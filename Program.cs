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
            List<Shape> shapes = d.scaleShapes(d.readData());

            KMclustering km = new KMclustering(shapes[1], new point(1000,1000),2,0.001);            
            km.clustering();

            int k = Convert.ToInt32(Math.Ceiling(Math.Sqrt(10)));
            FSM machine = new FSM(km, shapes[1], k);

            //FSM.serialize(machine, "testMachine.xml");
            //FSM f2 = FSM.deserialize("testMachine.xml");

            foreach(Gesture g in shapes[1].getGestures()){
            machine.recognize(g);
            Console.WriteLine("done");
            }

            return;
        }
    }
}

