using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;


namespace SP_Bewegungserkennung
{
    class Program
    {

        static void Main(string[] args)
        {

            int shapeNumber = 2; //max 22
            dataReader d = new dataReader("KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();

            //variable to determine the size of the clusters
            int k = 3;

            d.scaleShapes(shapes);



            List<KMclustering> clusterlist = new List<KMclustering>();
            for(int i=1; i<=22; i++) {
                KMclustering km = new KMclustering(shapes[i], new point(50, 50), 2, Double.Epsilon);
                km.clustering();
                clusterlist.Add(km);
            }
            Console.WriteLine("Clusters finished");

            //Visualisierung
            //visualisation.visualizeShape2(shapes[shapeNumber], km.CLlist, k);

            int temp = 1;
            List<FSM> fsmList = new List<FSM>();
            for(int i=1; i<=22; i++) {
                FSM machine = new FSM(clusterlist[i-1], shapes[i], k);
                fsmList.Add(machine);
            }
            //previous loops start at 1 because the very first shape is uninteresting
            Console.WriteLine("Fsm's trained");


            //FSM.serialize(machine, "testMachine.xml");
            //FSM f2 = FSM.deserialize("testMachine.xml");

            testFalsePositives(shapes, fsmList, k);
            testTruePositives(shapes, fsmList, k);

            /*
            foreach (Gesture g in shapes[shapeNumber].getGestures())
            {
               machine.recognize(g);
               Console.WriteLine("done");
            }*/

            Console.ReadLine();

            visualisation.closeVisualisation();

            Console.ReadLine();

            return;
        }

        static void testFalsePositives(List<Shape> shapes, List<FSM> fsmList, int k) {
            int totalGesturesTested = 0;
            List<int> FPlist = new List<int>(); //false Positives per machine


            //test, if a gesture was recognized mistakenly
            for(int fsmIndex = 0; fsmIndex < 22; fsmIndex++) { //each machine
                FSM machine = fsmList[fsmIndex];
                for(int shapeIndex=1; shapeIndex<=22; shapeIndex++) { //each shape
                    //skip shapes that trained the machine
                    if (fsmIndex == shapeIndex - 1) continue;

                    List<Gesture> gestureList = shapes[shapeIndex].getGestures();

                    int fp = 0;
                    foreach(Gesture g in gestureList) {
                        totalGesturesTested++;
                        if (machine.recognize(g)) fp++;
                    }
                    FPlist.Add(fp);
                }
            }


            int sumFP = 0;
            int minFP = int.MaxValue;
            int maxFP = 0;
            //evaluation
            foreach(int fp in FPlist) {
                sumFP += fp;
                if (fp < minFP) minFP = fp;
                if (fp > maxFP) maxFP = fp;
            }

            Console.WriteLine("Testing ended with " + sumFP + " false Positives of " + totalGesturesTested +
                " Gestures tested per machine. minimal FP:" + minFP + " ,maximal FP:" + maxFP);
        }



        static void testTruePositives(List<Shape> shapes, List<FSM> fsmList, int k) {
            int totalGesturesTested = 0;
            List<int> TPlist = new List<int>(); //false Positives per machine


            //test, if a gesture was recognized
            for (int index = 0; index < 22; index++) { //each machine
                FSM machine = fsmList[index];

                //and the corresponding shape
                List<Gesture> gestureList = shapes[index].getGestures();

                int tp = 0;
                foreach (Gesture g in gestureList) {
                    totalGesturesTested++;
                    if (machine.recognize(g)) tp++;
                }
                TPlist.Add(tp);
                
            }


            int sumTP = 0;
            
            //evaluation
            foreach (int fp in TPlist) {
                sumTP += fp;
            }

            Console.WriteLine("Testing ended with " + sumTP + " true Positives of " + totalGesturesTested +
                " Gestures tested per machine.");
        }
    }
}

