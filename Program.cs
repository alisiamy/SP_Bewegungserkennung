using System;
using System.Linq;
using System.Collections.Generic;


namespace SP_Bewegungserkennung {
    class Program {

        static void Main(string[] args) {
            //Parameters
            point variance = new point(30, 30);
            int tresholdMultiplier = 5;


            //Data for Clustering,FSM and Recognition
            dataReader d = new dataReader("C:/Users/Mori/source/repos/ConsoleApp1/ConsoleApp1/bin/Release/KinectDaten_Pascal.csv");
            List<Shape> shapeList = d.readData();


            //Preprocess Data
            d.scaleShapes(shapeList);


            //Gesture to recognize
            Gesture g = shapeList[7].getGestures()[15];     //some randome Gesture


            //Create Finite State Machine for each Shape
            List<KMclustering> clusterList = new List<KMclustering>();
            List<FSM> machineList = new List<FSM>();
            for (int i = 0; i < shapeList.Count; ++i) {
                KMclustering km = new KMclustering(shapeList[i], variance);

                km.clustering();

                FSM machine = new FSM(km, shapeList[i], tresholdMultiplier);
                machineList.Add(machine);
            }


            //Possibility to serialise FSM
            //FSM.serialize(machine, "testMachine.xml");
            //FSM fsm = FSM.deserialize("testMachine.xml");


            //Possibilty to visualize a shape or a gesture
            //int shapeToVisualize = 2;
            //visualisation.visualizeShape(shapeList[shapeToVisualize], clusterList[shapeToVisualize].CLlist, tresholdMultiplier);
            //visualisation.visualizeGesture(g.Points);
            /* But for visualisation the following assemby references are required;
             * System.Windows;
             * System.Windows.Controls;
             * System.Windows.Media;
             * System.Windows.Shapes;
             * System.Windows.Forms;
             * System.Threading;
             */


            //Necessary to determine wich machines are important to compare if ambiguity exists
            int recognisedMachineID = -1;
            int[] recognizedMachinesInGesture = new int[machineList.Count]; //to see wich machines are in conflict, remebers machine that return this for this gesture
            bool recognised = false;


            //Check every machine if it recognises the gesture
            for (int m = 0; m < machineList.Count; m++) {
                if (machineList[m].recognize(g)) {
                    if (!recognised) {
                        recognisedMachineID = m;
                    }

                    recognizedMachinesInGesture[m]++;
                    recognised = true;
                }
            }


            //No machine recognized
            if (recognisedMachineID < 0) {
                Console.WriteLine("No Gesture recognized");
                return;
            }


            //Handle machines that recognize Gesture at the same time:
            //look for machine with shortest summed up distance between all points of the gesture and the corresponding State
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


            Console.WriteLine("Gesture recognized as instance of Shape " + recognisedMachineID);
            Console.ReadLine();


            //Evaluations
            //First evaluation is for the finite state machines, the second is for the whole program
            //The evaluation takes very long, because the clustering is done againwith different trainingsets
            //The evaluation can be done without the above program (except reading in and scaling the data)
            //evaluation ev1 = new evaluation(shapeList);
            //evaluation ev2 = new evaluation(shapeList);
            //ev1.evaluate(tresholdMultiplier, variance);
            //ev2.evaluate2(tresholdMultiplier, variance);
            //ev1.saveEvaluation("C:/Users/User/Desktop/FSMevaluation.csv");
            //ev2.saveEvaluation("C:/Users/User/Desktop/programEvaluation.csv");

            return;
        }
    }
}

