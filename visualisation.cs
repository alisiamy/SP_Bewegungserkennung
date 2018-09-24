using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
//using System.Drawing;


namespace SP_Bewegungserkennung
{
    class visualisation {
        //Point range in x and y direction is 1..100
        private static readonly int windowSize = 800; //+30 damit Rechtecke am Rand auch gezeigt werden
        private static readonly int rectSize = 6;
        private static readonly int scale = windowSize / 100;
        private static Thread visThread;
        private static ManualResetEvent stopEvent = new ManualResetEvent(false);
        private static Cursor visCursor = new Cursor(Cursor.Current.Handle);

        /*public static void visualizeGesture(List<point> points)
        {
            Thread visThread;
            visThread = new Thread(new ThreadStart(() => visualizeGestureThread(points))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            visThread.Start();
        } */


        public static void visualizeShape2(Shape s, List<Cluster> clusterList, int k) {
            visThread = new Thread(new ThreadStart(() => visualisationThreads.visualizeShapeThread(s, clusterList, k))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            stopEvent.Reset();
            visThread.Start();
        }

        public static void closeVisualisation() {
            visualisationThreads.closeWindow();
            visThread.Abort();
        }

        class visualisationThreads {
            private static Window mainWindow = new Window();
            private bool draggingWindow = false;
            private System.Drawing.Point lm = new System.Drawing.Point();

            public static void closeWindow() {
                stopEvent.Set();
            }

            public static void visualizeShapeThread(Shape s, List<Cluster> clusterList, int k) {
                Console.WriteLine("starte VIS");

                // Create the application's main window
                //var mainWindow = new Window();
                mainWindow.Height = windowSize + 100;
                mainWindow.Width = windowSize + 100;
                mainWindow.DragMove();
                // Create a canvas sized to fill the window
                Canvas myCanvas = new Canvas();

                Cluster clus = clusterList[3];

                //Clustervisualization
                if (clusterList.Count != 0) {
                    foreach (Cluster c in clusterList) {

                        point threshold = c.variance.sqroot().mult(k); //radius
                        point center = c.mean;

                        Ellipse myEllipse = new Ellipse();
                        myEllipse.Stroke = System.Windows.Media.Brushes.Black;
                        if (clus.mean.Equals(c.mean)) {
                            myEllipse.Fill = System.Windows.Media.Brushes.Yellow;
                            Console.WriteLine("Gelbes Cluster, Mean: x=" + c.mean.x + ", y=" + c.mean.y);
                        } else {
                            myEllipse.Fill = System.Windows.Media.Brushes.DarkBlue;
                        }
                        myEllipse.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
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
                        if (pointInCluster2(p, clus.mean, clus.variance.sqroot().mult(k))) {
                            Rectangle myRect = new Rectangle();
                            myRect.Stroke = Brushes.Black;
                            myRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
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
                mainWindow.Title = "Cluster Visualisation";
                mainWindow.Show();

                stopEvent.WaitOne();

                mainWindow.Close();
            }

            //Function calculating if point is in cluster (not really working)
            private static bool pointInCluster(point p, point center, point treshold) {
                return point.abs(point.substract(center, p)).CompareTo(treshold) <= 0;
            }

            //Function calculating if point is in ellipse
            private static bool pointInCluster2(point p, point center, point treshold) {
                point normalized = new point(p.x - center.x, p.y - center.y);

                return ((double)(normalized.x * normalized.x) / (treshold.x * treshold.x)) +
                    ((double)(normalized.y * normalized.y) / (treshold.y * treshold.y)) <= 1.0;
            }
 
            //Function to visualize just one gesture
            public static void visualizeGestureThread(List<point> points) {
                Console.WriteLine("starte VIS");

                // Create the application's main window
                var mainWindow = new Window();
                mainWindow.Height = windowSize;
                mainWindow.Width = windowSize;

                // Create a canvas sized to fill the window
                Canvas myCanvas = new Canvas();


                foreach (point p in points) {
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
}
