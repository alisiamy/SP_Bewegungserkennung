using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace SP_Bewegungserkennung {
    class evaluation {
        readonly float[] percentage = { 0.25f, 0.5f, 0.75f, 1 };
        readonly string format = "percentage;shapeID;recognitionRate;FalsePositiveRates";
        public string result { get; private set; }
        List<Shape> shapeList;


        public evaluation(List<Shape> sl) {
            this.shapeList = sl;
            result = "";
        }

        private double evaluateRecognitionRate(FSM machine, List<Gesture> gesturelist) {

            int counter = 0;
            foreach (Gesture g in gesturelist) {
                if (machine.recognize(g)) {
                    ++counter;
                }
            }
            return counter / (double)gesturelist.Count;
        }

        private List<double> evaluateFalsePositives(FSM machine, int currentShape) {
            List<double> resList = new List<double>();

            for (int i = 0; i < shapeList.Count; ++i) {
                if (i == currentShape) {
                    continue;
                }
                resList.Add(evaluateRecognitionRate(machine, shapeList[i].getGestures()));
            }
            return resList;
        }

        public string evaluate(int tresholdMultiplier, point variance) {

            StringBuilder sb = new StringBuilder(format);
            sb.Append('\n');

            foreach (float percent in percentage) {
                for (int i = 1; i < shapeList.Count; ++i) {

                    List<Gesture> traininggestures = shapeList[i].getGestures(); // data for FSM
                    List<Gesture> testinggestures = new List<Gesture>();
                    if (percent < 1) {
                        int num_elems = (int)Math.Ceiling(traininggestures.Count * percent);

                        Random r = new Random(0);
                        for (int j = 0; j < num_elems; ++j) {
                            int index = r.Next(0, traininggestures.Count);
                            testinggestures.Add(traininggestures[index]);
                            traininggestures.RemoveAt(index);
                        }

                    }
                    if (percent == 1) {
                        testinggestures = traininggestures;
                    }
                    Shape tmpShape = new Shape(shapeList[i].shapeID, traininggestures);
                    KMclustering km = new KMclustering(shapeList[i], variance);

                    km.clustering();

                    FSM machine = new FSM(km, shapeList[i], tresholdMultiplier);

                    double rate = evaluateRecognitionRate(machine, testinggestures);
                    List<double> falsepositive = evaluateFalsePositives(machine, i);

                    sb.Append(percent * 100).Append(";").Append(shapeList[i].shapeID).Append(";").Append(rate).Append(";").Append(string.Join(",", falsepositive.ToArray())).Append('\n');
                }
            }
            result = sb.ToString();
            return result;
        }

        public void saveEvaluation(string path) {

            FileInfo fileInfo = new FileInfo(path);

            using (FileStream fs = fileInfo.OpenWrite()) {
                byte[] res = Encoding.ASCII.GetBytes(result);
                fs.Write(res, 0, res.Length);
            }


        }

        public string evaluate2(int tresholdMultiplier, point variance) {

            StringBuilder sb = new StringBuilder();
            //format
            for (int q = 0; q < 23; q++) {
                sb.Append(q+1).Append(";");
            }
            sb.Append("totalRecognised").Append(";").Append("totalTested").Append(";");
            sb.Append("\n");

            List<FSM> machineList = new List<FSM>();

            //percent = 100
            for (int i = 0; i < shapeList.Count; ++i) {

                List<Gesture> traininggestures = shapeList[i].getGestures(); // data for FSM
                List<Gesture> testinggestures = new List<Gesture>();

                testinggestures = traininggestures;

                Shape tmpShape = new Shape(shapeList[i].shapeID, traininggestures);
                KMclustering km = new KMclustering(shapeList[i], variance);

                km.clustering();

                FSM machine = new FSM(km, shapeList[i], tresholdMultiplier);
                machineList.Add(machine);
            }



            //matrix that stores the number of all positive responses of each machine per shape, int[shape][machine]
            int[][] matrix = new int[shapeList.Count][];

            for (int i = 0; i < shapeList.Count; i++) {
                //Console.WriteLine("Teste Shape " + i + "  mit " + shapeList[i].getGestures().Count + " Gesten, " + machineList.Count + " Maschinen");
                matrix[i] = evaluateShape(shapeList[i], machineList, sb);
            }



            //calculate precision and recall
            float[] precision = new float[machineList.Count]; //truePositive / (truePositive + falsePositive)
            float[] recall = new float[machineList.Count];    //truePositive / (truePositive + falseNegative)

            for(int i = 0; i < machineList.Count; i++) {
                float truePositive = matrix[i][i];
                float falseNegative = shapeList[i].getGestures().Count - truePositive;
                float falsePositive = 0;

                for(int fp = 0; fp < shapeList.Count; fp++) {
                    if (fp == i) continue;
                    falsePositive += matrix[fp][i];
                }
                falsePositive /= shapeList.Count;  //rough scaling because gestures not belonging to the tested machine were much more

                precision[i] = (truePositive + falsePositive) > 0 ? truePositive / (truePositive + falsePositive) : 0;
                recall[i] = truePositive / (truePositive + falseNegative);
            }

            sb.Append(string.Join(";", precision)).Append(";").Append("= precision").Append(";").Append("\n");
            sb.Append(string.Join(";", recall)).Append(";").Append("= recall").Append(";").Append("\n");


            //get average of precision and recall
            float avgPrec = 0;
            float avgRec = 0;
            for(int i = 0; i < shapeList.Count; i++) {
                avgPrec += precision[i];
                avgRec += recall[i];
            }
            avgPrec /= shapeList.Count;
            avgRec /= shapeList.Count;
            Console.WriteLine("Average Precision: " + avgPrec + " ,Average Recall: " + avgRec);


            result = sb.ToString();
            return result;
        }

        private int[] evaluateShape(Shape shape, List<FSM> machineList, StringBuilder sb) {
            int[] totalRecognised = new int[machineList.Count]; //array with the final result, will be one row in the later matrix
            int totalGesturesTested = shape.getGestures().Count;
            int totalGesturesRecognised = 0;

            foreach (Gesture g in shape.getGestures()) {
                int recognisedMachineID = -1;
                int[] recognizedMachinesInGesture = new int[machineList.Count]; //to see wich machines are in conflict, remebers machine that return this for this gesture
                bool recogInThisIteration = false;

                //check every machine if it recognises the current gesture
                for (int m = 0; m < machineList.Count; m++) {
                    if (machineList[m].recognize(g)) {
                        if (recogInThisIteration) {
                            //Console.WriteLine("Problem: multiple machines recognised same gesture at the same time");
                            continue;
                        } else {
                            recognisedMachineID = m;
                            totalGesturesRecognised++;
                        }

                        recognizedMachinesInGesture[m]++;
                        recogInThisIteration = true;
                    }
                }

                //no machine recognized
                if (recognisedMachineID < 0) {
                    continue;
                }

                //Handle machines that recognize Gesture at the same time:
                //look for machine with shortest summed up distance between all clusters
                if (recogInThisIteration) {
                    double minDist = Double.MaxValue;
                    int minMachine = -1;
                    for (int i = 0; i < recognizedMachinesInGesture.Length; i++) {
                        if (recognizedMachinesInGesture[i] != 0) {
                            if (machineList[i].getSumStateDistances() < minDist) {
                                minMachine = i;
                                minDist = machineList[i].getSumStateDistances();
                            }
                        }
                    }

                    recognisedMachineID = minMachine;
                }

                totalRecognised[recognisedMachineID]++;
            } //end of foreach(Gesture) loop

            //build string for each shape
            for (int i = 0; i < totalRecognised.Length; i++) {
                sb.Append(totalRecognised[i]).Append(";");
            }
            sb.Append(totalGesturesRecognised).Append(";").Append(totalGesturesTested).Append(";").Append("\n");

            return totalRecognised;
        }
    }
}
