using System;
using System.Linq;
using System.Collections.Generic;


namespace SP_Bewegungserkennung {
    class Program {

        static void Main(string[] args) {
            int shapeNumber = 4;  // shape to work with

            //** Data for Clustering,FSM and Rcognition**

            dataReader d = new dataReader("C:/Users/Mori/source/repos/ConsoleApp1/ConsoleApp1/bin/Release/KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();
            //int[] badShapes = { 23, 20, 16, 15, 12, 10, 2, 1};
            int[] badShapes = { 23, 22, 21, 20, 19, 18, 17, 11, 9, 8, 7, 6, 3, 2, 1 };

            for (int i = 0; i < badShapes.Length; i++) {
                shapes.RemoveAt(badShapes[i]-1); //indizes beginnen ab 0
            } 


            d.scaleShapes(shapes);

            evaluation ev = new evaluation(shapes);

            int k = 5;
            double variance = 30;
            ev.evaluate2(k, new point(variance, variance));

            ev.saveEvaluation("C:/Users/Mori/Desktop/evRES_5_30_verySlim.csv");

            Console.WriteLine("FERTIG!");
            Console.ReadLine();

            /*

           //**Run Clustering**

           KMclustering km = new KMclustering(shapes[shapeNumber], new point(50, 50), 2, Double.Epsilon);
           km.clustering();

           // **Visualize Clusters**
           // visualisation.visualizeShape2(shapes[shapeNumber], km.CLlist, 3);

           visualiser vsl = new visualiser(km.CLlist);
           vsl.runVisualiser();


           // **Create FSM**

           FSM machine = new FSM(km, shapes[shapeNumber], 3);

           // **Serialise FSM**

           //FSM.serialize(machine, "testMachine.xml");
           //FSM f2 = FSM.deserialize("testMachine.xml");


           // **Visualize States**

           List<point> pvis = new List<point>();

           foreach (Gesture g in shapes[shapeNumber].getGestures()) {
               foreach (point p in g.Points) {
                   pvis.Add(p);
               }
           }

           visualiser vsl2 = new visualiser(machine.stateList,pvis);
           vsl2.runVisualiser();

           // **Recognition**

           foreach (Gesture g in shapes[shapeNumber].getGestures())
           {
                machine.recognize(g);
                            
           }


           Console.ReadLine();
           return;//*/

        }
    }
}

