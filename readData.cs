using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;


namespace Bewegungserkennung 
{

    /*
        parsing data line per line with line format:
        FrameID;X;Y;Z;GestureID;SqlTime;Alpha;Beta;Velocity;ShapeId;UID
    */

   enum LineFormat {FrameID,Y,X,Z,GestureID,SqlTime,Alpha,Beta,Velocity,ShapeID,UID};

    //TODO: file validity checking and exception handling
    public class dataReader 
    {
        string file;
        private Dictionary<int,Shape> shapes;
        private Dictionary<int,Shape> scaleDic;
        const long FRAME = 30; 

        public dataReader(string file)
        {
            this.file = file;
            this.shapes = new Dictionary<int,Shape>();
        }

        public List<Shape> readData()
        {
            foreach (string line in File.ReadLines(file))
            {
                string[] properties = line.Trim().Split(';');
                int shapeID = Int32.Parse(properties[(int)LineFormat.ShapeID]);

                if (shapes.ContainsKey(shapeID))
                {
                    Shape s;
                    shapes.TryGetValue(shapeID,out s);
                    
                    int gestureID = Int32.Parse(properties[(int)LineFormat.GestureID]);
                    if (s.ContainsGesture(gestureID))
                    {
                        Gesture g = s.getGesture(gestureID);
                        g.Add(new point(Double.Parse(properties[(int)LineFormat.X], CultureInfo.GetCultureInfo("de-DE")),
                                        Double.Parse(properties[(int)LineFormat.Y], CultureInfo.GetCultureInfo("de-DE")),
                                         Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME));
                    }
                    else
                    {
                        s.Add(gestureID,new Gesture(gestureID,
                                    new List<point>(){new point(Double.Parse(properties[(int)LineFormat.X],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                Double.Parse(properties[(int)LineFormat.Y],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                    Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME)}));
                    }
                }
                else
                {
                    shapes.Add(shapeID,new Shape(shapeID,new Gesture(Int32.Parse(properties[(int)LineFormat.GestureID]),
                                    new List<point>(){new point(Double.Parse(properties[(int)LineFormat.X],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                Double.Parse(properties[(int)LineFormat.Y],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                    Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME)})));
                }
            }
            return shapes.Values.ToList();
        }

        public Shape getShape(int num) 
        {
            Shape ret = null;
            shapes.TryGetValue(num, out ret);
            return ret;
        }

        public List<Shape> getShapes()
        {
            return shapes.Values.ToList();
        }

        //Scales all Gestures of one shape to fit in the range 1 to 100
        public List<Shape> scaleShapes(List<Shape> shapeList) {
            scaleDic = new Dictionary<int, Shape>();

            foreach (Shape shape in shapeList) {
                foreach (Gesture g in shape.getGestures()) {
                    List<point> points = g.Points;
                    List<point> temp = new List<point>();

                    double minX = int.MaxValue;
                    double maxX = int.MinValue;
                    double minY = int.MaxValue;
                    double maxY = int.MinValue;

                    foreach (point p in points) {
                        if (p.x > maxX) maxX = p.x;
                        if (p.x < minX) minX = p.x;
                        if (p.y > maxY) maxY = p.y;
                        if (p.y < minY) minY = p.y;
                    }

                    double moveX = 0 - minX;
                    double moveY = 0 - minY;
                    double scaleX = 100 / (maxX + moveX);
                    double scaleY = 100 / (maxY + moveY);

                    foreach (point p in points) {
                        double x = (p.x + moveX) * scaleX;
                        double y = (p.y + moveY) * scaleY;

                        if (x < 0 || y < 0) Console.WriteLine("X:" + x + " ,Y:" + y);

                        temp.Add(new point(x, y));
                    }

                    Gesture tempGesture = new Gesture(g.gestureID, temp);


                    //neues Liste erstellt, da der set-accessor der point Klasse private ist 
                    //-> Punkte k�nnen nicht manipuliert werden

                    
                    if (scaleDic.ContainsKey(shape.shapeID)) {
                        Shape s;
                        scaleDic.TryGetValue(shape.shapeID, out s);

                        if(s.ContainsGesture(g.gestureID)) {
                            Console.WriteLine("sollte nicht passieren");
                        } else {
                            s.Add(g.gestureID, tempGesture);
                        }

                        

                    } else { // neues shape erstellen -> neue geste einf�gen
                        scaleDic.Add(shape.shapeID, new Shape(shape.shapeID, tempGesture));
                    }

                }
            }

            return scaleDic.Values.ToList();
        }

        public List<Shape> scaleGesture(List<Shape> shapeList) {

        }

    }
}