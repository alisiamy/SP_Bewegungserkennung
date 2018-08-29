using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;


namespace Bewegungserkennung 
{

    /*
        parsing data line per line with line format:
        FrameID;X;Y;Z;GestureID;SqlTime;Alpha;Beta;Velocity;ShapeId;UID
    */

    enum LineFormat {FrameID,X,Y,Z,GestureID,SqlTime,Alpha,Beta,Velocity,ShapeID,UID};

    //TODO: file validity checking and exception handling
    public class dataReader 
    {
        string file;
        private Dictionary<int,Shape> shapes;
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
                        g.Add(new point(Double.Parse(properties[(int)LineFormat.X], System.Globalization.CultureInfo.InstalledUICulture),
                                        Double.Parse(properties[(int)LineFormat.Y], System.Globalization.CultureInfo.InstalledUICulture),
                                        Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME));
                    }
                    else
                    {
                        s.Add(gestureID,new Gesture(gestureID,
                                    new List<point>(){new point(Double.Parse(properties[(int)LineFormat.X],
                                                                    System.Globalization.CultureInfo.InstalledUICulture),
                                                                Double.Parse(properties[(int)LineFormat.Y],
                                                                    System.Globalization.CultureInfo.InstalledUICulture),
                                                                    Int64.Parse(properties[(int)LineFormat.FrameID])*FRAME)}));
                    }
                }
                else
                {
                    shapes.Add(shapeID,new Shape(shapeID,new Gesture(Int32.Parse(properties[(int)LineFormat.GestureID]),
                                    new List<point>(){new point(Double.Parse(properties[(int)LineFormat.X],
                                                                    System.Globalization.CultureInfo.InstalledUICulture),
                                                                Double.Parse(properties[(int)LineFormat.Y],
                                                                    System.Globalization.CultureInfo.InstalledUICulture),
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
    }

}