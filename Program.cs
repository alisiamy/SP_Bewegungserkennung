using System;
using System.Linq;
using System.Collections.Generic;


namespace Bewegungserkennung
{
    class Program
    {
        private static int shapeNumber = 2;

        static void Main(string[] args)
        {
            dataReader d = new dataReader("KinectDaten_Pascal.csv");
            List<Shape> shapess= d.readData();

            //SWPvisualization.visualizeShape(shapess[shapeNumber], new List<Cluster>());
            List<Shape> shapes = d.scaleShapes(shapess);

            //SWPvisualization.visualizeShape(shapes[shapeNumber], new List<Cluster>());

            
            //int shapeNumber = 4;
            KMclustering km = new KMclustering(shapes[shapeNumber], new point(1000,1000),2,0.001);            
            km.clustering();

            //km.CLlist.Add(new Cluster(new point(3, 3), new point(15, 15)));       //for testing

            //Visualisierung
            //new List<Cluster>();
            SWPvisualization.visualizeShape(shapes[shapeNumber], km.CLlist);
            Console.WriteLine(km.CLlist.Count);

            int k = Convert.ToInt32(Math.Ceiling(Math.Sqrt(10)));
            FSM machine = new FSM(km, shapes[shapeNumber], k);

            //FSM.serialize(machine, "testMachine.xml");
            //FSM f2 = FSM.deserialize("testMachine.xml");

            
            foreach(Gesture g in shapes[shapeNumber].getGestures()) {
                if(machine.recognize(g)) {
                    Console.WriteLine("Maschine erkannt");
                    break;
                }
                //Console.WriteLine("done");
            }

            Console.WriteLine("Alles durchgelaufen");
            Console.ReadLine();
            return;
        }
    }
}

