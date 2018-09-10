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
        private static readonly int windowSize = 800 + 30; //+8 damit Rechtecke am Rand auch gezeigt werden
        private static readonly int rectSize = 8;

        public static void visualizeGesture(List<point> points) {
            Thread visThread;
            visThread = new Thread(new ThreadStart(() => visualizeGestureThread(points))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            visThread.Start();
        }

        public static void visualizeShape(Shape s) {
            Thread visThread;
            visThread = new Thread(new ThreadStart(() => visualizeShapeThread(s))); //lambda expression to start thread with parameter
            visThread.SetApartmentState(ApartmentState.STA);
            visThread.Start();
        }

        private static void visualizeShapeThread(Shape s) {
            Console.WriteLine("starte VIS");

            // Create the application's main window
            var mainWindow = new Window();
            mainWindow.Height = windowSize;
            mainWindow.Width = windowSize;

            // Create a canvas sized to fill the window
            Canvas myCanvas = new Canvas();

            foreach (Gesture g in s.getGestures()) {
                List<point> points = g.Points;
            foreach (point p in points) {
                Rectangle myRect = new Rectangle();
                myRect.Stroke = Brushes.Black;
                myRect.Fill = Brushes.SkyBlue;
                //myRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                //myRect.VerticalAlignment = VerticalAlignment.Center;
                myRect.Height = rectSize;
                myRect.Width = rectSize;
                myCanvas.Children.Add(myRect);
                Canvas.SetTop(myRect, windowSize - p.y * rectSize - 30);
                Canvas.SetLeft(myRect, p.x * rectSize);
            }

            }

            // Add a "Hello World!" text element to the Canvas
            /*TextBlock txt1 = new TextBlock();
            txt1.FontSize = 14;
            txt1.Text = "Hello World!";
            Canvas.SetTop(txt1, 100);
            Canvas.SetLeft(txt1, 10);
            myCanvas.Children.Add(txt1);*/




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
                Canvas.SetTop(myRect, windowSize - p.y * rectSize - 30);
                Canvas.SetLeft(myRect, p.x * rectSize);
            }

            // Add a "Hello World!" text element to the Canvas
            /*TextBlock txt1 = new TextBlock();
            txt1.FontSize = 14;
            txt1.Text = "Hello World!";
            Canvas.SetTop(txt1, 100);
            Canvas.SetLeft(txt1, 10);
            myCanvas.Children.Add(txt1);*/




            mainWindow.Content = myCanvas;
            mainWindow.Title = "Canvas Sample";
            mainWindow.Show();


            //Thread.Sleep(2000);

            Console.ReadLine();
        }
    }
}
