using System;
using System.Linq;
using System.Collections.Generic;


namespace SP_Bewegungserkennung
{
    class Program
    {

        static void Main(string[] args)
        {
            int shapeNumber = 2; //max 22
            dataReader d = new dataReader("C:/Users/Daria/source/repos/SP_Bewegungserkennung/SP_Bewegungserkennung/KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();

            d.scaleShapes(shapes);


            KMclustering km = new KMclustering(shapes[shapeNumber], new point(50, 50), 2, Double.Epsilon);
            km.clustering();


            //Visualisierung
           //visualisation.visualizeShape2(shapes[shapeNumber], km.CLlist);

            //int k = Convert.ToInt32(Math.Ceiling(Math.Sqrt(10)));
            FSM machine = new FSM(km, shapes[shapeNumber], 2);

            //FSM.serialize(machine, "testMachine.xml");
            //FSM f2 = FSM.deserialize("testMachine.xml");


            foreach (Gesture g in shapes[shapeNumber].getGestures())
            {
               machine.recognize(g);
               Console.WriteLine("done");
            }

            Console.ReadLine();
            return;
        }
    }
}

