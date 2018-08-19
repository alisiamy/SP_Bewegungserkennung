
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

            Gesture g;
            removeOneGesture(shapes[1], out g);


            KMclustering cluster1 = new KMclustering(shapes[1], new point(1.0e+17,1.0e+17),2,0.1);
            cluster1.clustering();
            FSM machine = new FSM(cluster1, shapes[1],3);

            KMclustering cluster2 = new KMclustering(shapes[2], new point(1.0e+17,1.0e+17),2,0.1);
            cluster2.clustering();
            FSM machine2 = new FSM(cluster2, shapes[2],3);


            Console.WriteLine("Clustering fertig, testen?");
            String input = Console.ReadLine();
            if(input!="ja") return;

            List<point> points = new List<point>();
            foreach(point p in points) {
                if(machine.tick(p)) {
                    Console.WriteLine("Geste gehört zu shape 1");
                    return;
                }

                if(machine.tick(p)) {
                    Console.WriteLine("Geste gehört zu shape 2");
                    return;
                }
            }

            Console.WriteLine("Geste konnte keinem shape zugeordnet werden");


            return;
        }

        private static void removeOneGesture(Shape shape, out Gesture g)
        {
            int numberOfGestures = shape.getGestures().Count;
            Random r = new Random();
            int removeNumber = r.Next(0,numberOfGestures+1);
            int removeID = shape.gestureIDList[removeNumber];

            g = shape.getGesture(removeID);
            if(shape.removeGesture(removeID)) {
                Console.WriteLine("Gesture(ID " + removeID + ") removed");
            } else {
                Console.WriteLine("Gesture(ID " + removeID + ") couldn't be removed");
            }

        }
    }
}