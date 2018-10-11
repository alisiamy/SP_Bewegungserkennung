using System;
using System.Linq;
using System.Collections.Generic;


namespace SP_Bewegungserkennung
{
    class Program
    {

        static void Main(string[] args)
        {
            int shapeNumber = 4;  // shape to work with

            //** Data for Clustering,FSM and Rcognition**

            dataReader d = new dataReader("C:/Users/Daria/source/repos/SP_Bewegungserkennung/SP_Bewegungserkennung/KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();

            d.scaleShapes(shapes);


            evaluation ev = new evaluation(shapes);

            ev.evaluate(5, new point(20,20));

            ev.saveEvaluation("C:/Users/Daria/Desktop/evRES.csv");


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

