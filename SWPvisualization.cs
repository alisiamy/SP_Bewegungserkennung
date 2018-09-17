using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Threading;



namespace Bewegungserkennung    
{
    class SWPvisualization
    {
        //Point range in x and y direction is 1..100
        private static readonly int windowSize = 800; //+30 damit Rechtecke am Rand auch gezeigt werden
        private static readonly int rectSize = 6;
        private static readonly int scale = windowSize / 100;

        //private static Window mainWindow;

        public static void visualizeGesture(List<point> points) {
            Thread visThread;
            visThread = new Thread(new ThreadStart(() => visualizeGestureThread(points))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            visThread.Start();
        }

        public static void visualizeShape(Shape s, List<Cluster> clusterList) {
            Thread visThread;
            visThread = new Thread(new ThreadStart(() => visualizeShapeThread(s, clusterList))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            visThread.Start();
        }

        private static void visualizeShapeThread(Shape s, List<Cluster> clusterList) {
            Console.WriteLine("starte VIS");

            // Create the application's main window
            var mainWindow = new Window();
            mainWindow.Height = windowSize+100;
            mainWindow.Width = windowSize+100;
            mainWindow.DragMove();
            // Create a canvas sized to fill the window
            Canvas myCanvas = new Canvas();


            //Clustervisualization
            if (clusterList.Count != 0) {
                foreach (Cluster c in clusterList) {
                    point threshold = c.variance.sqroot().mult(clusterList.Count); //radius
                    point center = c.mean;

                    Ellipse myEllipse = new Ellipse();
                    myEllipse.Stroke = System.Windows.Media.Brushes.Black;
                    myEllipse.Fill = System.Windows.Media.Brushes.DarkBlue;
                    myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
                    myEllipse.VerticalAlignment = VerticalAlignment.Center;
                    myEllipse.Width = threshold.x * scale / 2; myEllipse.Height =  threshold.y * scale / 2; //richtige grösse
                    //myEllipse.Width = 30; myEllipse.Height = 30; //nur zentren visualisieren
                    myCanvas.Children.Add(myEllipse);
                    Canvas.SetBottom(myEllipse, center.y * scale - myEllipse.Height/2 + 20);
                    Canvas.SetLeft(myEllipse, center.x * scale - myEllipse.Width/2 + 40);
                }
            }


            //Visualization of the points
            foreach (Gesture g in s.getGestures()) {
                List<point> points = g.Points;
                foreach (point p in points) {
                    Rectangle myRect = new Rectangle();
                    myRect.Stroke = Brushes.Black;
                    myRect.HorizontalAlignment = HorizontalAlignment.Left;
                    myRect.VerticalAlignment = VerticalAlignment.Bottom;
                    myRect.Fill = Brushes.SkyBlue;
                    myRect.Height = rectSize;
                    myRect.Width = rectSize;
                    myCanvas.Children.Add(myRect);
                    Canvas.SetBottom(myRect, p.y * scale + 20);
                    Canvas.SetLeft(myRect, p.x * scale + 40);
                }

            }

            mainWindow.Content = myCanvas;
            mainWindow.Title = "Canvas Sample";
            mainWindow.Show();


            //Thread.Sleep(2000);

            Console.ReadLine();
        }

        private static void visualizeGestureThread(List<point> points)    {
            Console.WriteLine("starte VIS");

            // Create the application's main window
            var mainWindow = new Window();
            mainWindow.Height = windowSize;
            mainWindow.Width = windowSize;
            
            // Create a canvas sized to fill the window
            Canvas myCanvas = new Canvas();


            foreach(point p in points) {
                Rectangle myRect = new Rectangle();
                myRect.Stroke = Brushes.Black;
                myRect.Fill = Brushes.SkyBlue;
                //myRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                //myRect.VerticalAlignment = VerticalAlignment.Center;
                myRect.Height = rectSize;
                myRect.Width = rectSize;
                myCanvas.Children.Add(myRect);
                Canvas.SetBottom(myRect, p.y * scale + 30);
                Canvas.SetLeft(myRect, p.x * scale + 50);
            }

            mainWindow.Content = myCanvas;
            mainWindow.Title = "Canvas Sample";
            mainWindow.Show();


            //Thread.Sleep(2000);

            Console.ReadLine();
        }
    }
}
