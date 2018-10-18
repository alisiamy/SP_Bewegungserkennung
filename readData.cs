using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;


namespace SP_Bewegungserkennung {

    /*
        parsing data line per line with line format:
        FrameID;X;Y;Z;GestureID;SqlTime;Alpha;Beta;Velocity;ShapeId;UID
    */

    enum LineFormat { FrameID, Y, X, Z, GestureID, SqlTime, Alpha, Beta, Velocity, ShapeID, UID };


    public class dataReader {
        string file;
        private Dictionary<int, Shape> shapes;

        //TODO: USER INTERACTION
        const long FRAME = 30; //can change with different data 

        public dataReader(string file) {
            this.file = file;
            this.shapes = new Dictionary<int, Shape>();
        }

        public List<Shape> readData() //list of shapes contains list of gestures, contains list of points
        {
            foreach (string line in File.ReadLines(file)) {
                string[] properties = line.Trim().Split(';');
                int shapeID = Int32.Parse(properties[(int)LineFormat.ShapeID]);

                if (shapes.ContainsKey(shapeID)) {
                    Shape s;
                    shapes.TryGetValue(shapeID, out s);

                    int gestureID = Int32.Parse(properties[(int)LineFormat.GestureID]);
                    if (s.ContainsGesture(gestureID)) {
                        Gesture g = s.getGesture(gestureID);
                        //point contains x,y value
                        g.Add(new point(Double.Parse(properties[(int)LineFormat.X], CultureInfo.GetCultureInfo("de-DE")),
                                        Double.Parse(properties[(int)LineFormat.Y], CultureInfo.GetCultureInfo("de-DE")),
                                         Int64.Parse(properties[(int)LineFormat.FrameID]) * FRAME));
                    } else {
                        s.Add(gestureID, new Gesture(gestureID,
                                    new List<point>(){new point(Double.Parse(properties[(int)LineFormat.X],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                Double.Parse(properties[(int)LineFormat.Y],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                    Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME)}));
                    }
                } else {
                    shapes.Add(shapeID, new Shape(shapeID, new Gesture(Int32.Parse(properties[(int)LineFormat.GestureID]),
                                    new List<point>(){new point(Double.Parse(properties[(int)LineFormat.X],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                Double.Parse(properties[(int)LineFormat.Y],
                                                                    CultureInfo.GetCultureInfo("de-DE")),
                                                                    Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME)})));
                }
            }
            return shapes.Values.ToList();
        }

        public Shape getShape(int num) {
            Shape ret = null;
            shapes.TryGetValue(num, out ret);
            return ret;
        }

        public List<Shape> getShapes() {
            return shapes.Values.ToList();
        }

        //Scale all Gestures of one shape to fit in the range 1 to 100
        //without consideration of bad exceptions
        public void scaleShapes(List<Shape> shapeList) {

            foreach (Shape shape in shapeList) {
                foreach (Gesture g in shape.getGestures()) {

                    double minX = double.MaxValue;
                    double maxX = double.MinValue;
                    double minY = double.MaxValue;
                    double maxY = double.MinValue;

                    foreach (point p in g.Points) {
                        if (p.x > maxX) maxX = p.x;
                        if (p.x < minX) minX = p.x;
                        if (p.y > maxY) maxY = p.y;
                        if (p.y < minY) minY = p.y;
                    }

                    point move = new point(0 - minX, 0 - minY);
                    point scale = new point(100 / (maxX + move.x), 100 / (maxY + move.y));

                    for (int i = 0; i < g.Points.Count; ++i) {
                        g.Points[i].addition(move);
                        g.Points[i].multiply(scale);

                    }
                }
            }
        }
    }
}