using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Bewegungserkennung
{
    class evaluation
    {
        readonly float[] percentage = {0.25f, 0.5f, 0.75f, 1};
        readonly string format = "percentage;shapeID;recognitionRate;FalsePositiveRates";
        public string result { get; private set;}
        List<Shape> shapeList;
               

        public evaluation(List<Shape> sl)
        {
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
            return counter/(double)gesturelist.Count;
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

        public void evaluate(int tresholdMultiplier, point variance) {

            foreach (float percent in percentage) {
                for (int i = 1; i < shapeList.Count; ++i) {

                    List<Gesture> traininggestures = shapeList[i].getGestures();
                    List<Gesture> testinggestures = shapeList[i].getGestures();
                    if (percent < 1)
                    {
                        int num_elems = (int)Math.Ceiling(traininggestures.Count * percent);
                        traininggestures.RemoveRange(0,num_elems);
                        testinggestures.RemoveRange(num_elems, testinggestures.Count - num_elems);
                    }
                    Shape tmpShape = new Shape(shapeList[i].shapeID, traininggestures);
                    KMclustering km = new KMclustering(shapeList[i], variance);

                    km.clustering();

                    FSM machine = new FSM(km, shapeList[i], tresholdMultiplier);

                    double rate = evaluateRecognitionRate(machine, testinggestures);
                    List<double> falsepositive = evaluateFalsePositives(machine, i);
                    
                }
            }
        }


    }
}
