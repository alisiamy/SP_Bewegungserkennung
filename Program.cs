
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
//using System.Drawing;
using System.Windows.Media;
using System.Threading;


namespace Bewegungserkennung {
    class Program {
        static void Main(string[] args) {
            /*Boolean tessst = false;
            List<point> l = new List<point>();

            Thread WorkerThread;
            WorkerThread = new Thread(new ThreadStart(() => SWPvisualization.visualize(l))); //lambda expression to start thread with parameter
            WorkerThread.SetApartmentState(ApartmentState.STA);
            WorkerThread.Start();

           


            if (tessst){*/
            dataReader d = new dataReader("KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();

            
            //Punkte skalieren: Reichweite 0..100
            shapes = d.scaleShapes(shapes);


            int shapeID = 4;
            var id = shapes[shapeID].gestureIDList[2];
            //SWPvisualization.visualizeGesture(shapes[shapeID].getGesture(id).Points);
            SWPvisualization.visualizeShape(shapes[shapeID]);


            

            Gesture g;
            removeOneGesture(shapes[2], out g);


            KMclustering cluster1 = new KMclustering(shapes[1], new point(1.0e+17, 1.0e+17), 2, 0.1);
            cluster1.clustering();
            FSM machine = new FSM(cluster1, shapes[1], 3);

            KMclustering cluster2 = new KMclustering(shapes[2], new point(1.0e+17, 1.0e+17), 2, 0.1);
            cluster2.clustering();
            FSM machine2 = new FSM(cluster2, shapes[2], 3);


            Console.WriteLine("Clustering fertig, testen?");
            String input = Console.ReadLine();
            if (input != "ja") return;

            Boolean machineFound = false;
            List<point> points = g.Points;
            foreach (point p in points) {
                if (machine.tick(p)) {
                    Console.WriteLine("Geste gehört zu shape 1");
                    machineFound = true;
                    //break;
                }
                Console.WriteLine("1");
                if (machine.tick(p)) {
                    Console.WriteLine("Geste gehört zu shape 2");
                    machineFound = true;
                    //break;
                }
                Console.WriteLine("2");
            }

            if (!machineFound) Console.WriteLine("Geste konnte keinem shape zugeordnet werden");

            Console.ReadLine();
            //}// tessst ende
            return;
        }

        private static void removeOneGesture(Shape shape, out Gesture g) {
            int numberOfGestures = shape.getGestures().Count;
            Random r = new Random();
            int removeNumber = r.Next(0, numberOfGestures + 1);
            int removeID = shape.gestureIDList[removeNumber];

            g = shape.getGesture(removeID);
            if (shape.removeGesture(removeID)) {
                Console.WriteLine("Gesture(ID " + removeID + ") removed");
            } else {
                Console.WriteLine("Gesture(ID " + removeID + ") couldn't be removed");
            }

        }
    }
}