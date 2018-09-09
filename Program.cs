
using System;
using System.Linq;
using System.Collections.Generic;


namespace Bewegungserkennung
{
    class Program
    {
        static void Main(string[] args)
        {
            dataReader d = new dataReader("KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();
            d.modifyShapes(shapes);

            KMclustering km = new KMclustering(shapes[5], new point(500,500),2,0.1);            
            km.clustering();

            int k = Convert.ToInt32(Math.Ceiling(Math.Sqrt(10)));
            FSM machine = new FSM(km, shapes[5], k);

            //FSM.serialize(machine, "testMachine.xml");
            //FSM f2 = FSM.deserialize("testMachine.xml");

            foreach(Gesture g in shapes[5].getGestures()){
            machine.recognize(g);
            Console.WriteLine("done");
            }

            return;
        }
    }
}

