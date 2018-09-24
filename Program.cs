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

            d.scaleShapes(shapes);


            KMclustering km = new KMclustering(shapes[shapeNumber], new point(50, 50), 2, Double.Epsilon);
            km.clustering();

            //variable to determine the size of the clusters
            int k = 3; 

            //Visualisierung
            visualisation.visualizeShape2(shapes[shapeNumber], km.CLlist, k);

            //int k = Convert.ToInt32(Math.Ceiling(Math.Sqrt(10)));
            FSM machine = new FSM(km, shapes[shapeNumber], k);

            //FSM.serialize(machine, "testMachine.xml");
            //FSM f2 = FSM.deserialize("testMachine.xml");


            foreach (Gesture g in shapes[shapeNumber].getGestures())
            {
               machine.recognize(g);
               Console.WriteLine("done");
            }

            Console.ReadLine();

            visualisation.closeVisualisation();

            return;
        }
    }
}

