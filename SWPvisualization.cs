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

        public static void visualizeShape2(Shape s, List<Cluster> clusterList) {
            Thread visThread;
            visThread = new Thread(new ThreadStart(() => visualizeShapeThread2(s, clusterList))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            visThread.Start();
            visThread.Join();
        }

        private static void visualizeShapeThread2(Shape s, List<Cluster> clusterList) {
            Console.WriteLine("starte VIS");

            // Create the application's main window
            var mainWindow = new Window();
            mainWindow.Height = windowSize + 100;
            mainWindow.Width = windowSize + 100;
            mainWindow.DragMove();
            // Create a canvas sized to fill the window
            Canvas myCanvas = new Canvas();

            Cluster clus = clusterList[3];
            double KKK = 3;

            //Clustervisualization
            if (clusterList.Count != 0) {
                foreach (Cluster c in clusterList) {
                    
                    point threshold = c.variance.sqroot().mult(KKK); //radius
                    point center = c.mean;

                    Ellipse myEllipse = new Ellipse();
                    myEllipse.Stroke = System.Windows.Media.Brushes.Black;
                    if (clus.mean.Equals(c.mean)) {
                        myEllipse.Fill = System.Windows.Media.Brushes.Yellow;
                        Console.WriteLine("Gelbes Cluster, Mean: x=" + c.mean.x + ", y=" + c.mean.y);
                    } else {
                        myEllipse.Fill = System.Windows.Media.Brushes.DarkBlue;
                    }
                    myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
                    myEllipse.VerticalAlignment = VerticalAlignment.Center;
                    //myEllipse.Width = threshold.x * scale / 2; myEllipse.Height = threshold.y * scale / 2; //rumgespielte Größe
                    myEllipse.Width = threshold.x * scale * 2; myEllipse.Height = threshold.y * scale * 2; //richtige grösse | *2 wegen radius -> durchmesser
                    //myEllipse.Width = 30; myEllipse.Height = 30; //nur zentren visualisieren
                    myCanvas.Children.Add(myEllipse);
                    Canvas.SetBottom(myEllipse, center.y * scale - myEllipse.Height / 2 + 20);
                    Canvas.SetLeft(myEllipse, center.x * scale - myEllipse.Width / 2 + 40);
                }
            }

            
            //Visualization of the points
            foreach (Gesture g in s.getGestures()) {
                List<point> points = g.Points;
                foreach (point p in points) {
                    if (pointInCluster3(p, clus.mean, clus.variance.sqroot().mult(KKK))) { 
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

            }

            mainWindow.Content = myCanvas;
            mainWindow.Title = "Canvas Sample";
            mainWindow.Show();


            //Thread.Sleep(2000);

            Console.ReadLine();

            mainWindow.Close();
        }

        private static bool pointInCluster(point p, point center, point treshold) {
            return point.abs(point.substract(center, p)).CompareTo(treshold) <= 0;
        }

        private static bool pointInCluster2(point p, point center, point treshold) {
            Double meanTresh = ( treshold.x + treshold.y ) / 2;

            return center.distance(p) < meanTresh ? true : false;
        }

        private static bool pointInCluster3(point p, point center, point treshold) {
            point normalized = new point(p.x - center.x, p.y - center.y);

            return ((double)(normalized.x * normalized.x) / (treshold.x * treshold.x)) + 
                ((double)(normalized.y * normalized.y) / (treshold.y * treshold.y)) <= 1.0;
        }

        /*
         *  public bool Contains(Ellipse Ellipse, Point location)
        {
            Point center = new Point(
                  Canvas.GetLeft(Ellipse) + (Ellipse.Width / 2),
                  Canvas.GetTop(Ellipse) + (Ellipse.Height / 2));

            double _xRadius = Ellipse.Width / 2;
            double _yRadius = Ellipse.Height / 2;


            if (_xRadius <= 0.0 || _yRadius <= 0.0)
                return false;
            /* This is a more general form of the circle equation
             *
             * X^2/a^2 + Y^2/b^2 <= 1
             *

        Point normalized = new Point(location.X - center.X,
                                     location.Y - center.Y);

            return ((double)(normalized.X* normalized.X)
                     / (_xRadius* _xRadius)) + ((double)(normalized.Y* normalized.Y) / (_yRadius* _yRadius))
                <= 1.0;
        } */

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

            mainWindow.Close();
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

            mainWindow.Close();
        }
    }
}
