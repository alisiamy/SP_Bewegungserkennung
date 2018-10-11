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
            for (int q = 1; q <= 23; q++) {
                sb.Append(q).Append(";");
            }
            sb.Append("totalRecognised").Append(";").Append("totalTested").Append(";");
            sb.Append("\n");

            //percent = 100
            for (int i = 1; i < shapeList.Count; ++i) {

                List<Gesture> traininggestures = shapeList[i].getGestures(); // data for FSM
                List<Gesture> testinggestures = new List<Gesture>();

                testinggestures = traininggestures;

                Shape tmpShape = new Shape(shapeList[i].shapeID, traininggestures);
                KMclustering km = new KMclustering(shapeList[i], variance);

                km.clustering();

                FSM machine = new FSM(km, shapeList[i], tresholdMultiplier);



                int totalRecognised = 0;
                int totalTested = 0;
                for (int c = 0; c < 23; c++) { //for each row
                    int rec = evaluateMachine(machine, shapeList[c].getGestures());
                    sb.Append(rec).Append(";");

                    totalRecognised += rec;
                    totalTested += shapeList[c].getGestures().Count;
                }
                sb.Append(totalRecognised).Append(";").Append(totalTested).Append(";").Append('\n');
            }

            result = sb.ToString();
            return result;
        }

        private int evaluateMachine(FSM machine, List<Gesture> gestureList) {
            int counter = 0;
            foreach (Gesture g in gestureList) {
                if (machine.recognize(g)) {
                    ++counter;
                }
            }
            return counter;
        }
    }
}
