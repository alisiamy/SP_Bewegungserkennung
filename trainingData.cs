using System;
using System.Collections.Generic;
using System.Diagnostics;

using gD = gestureData;
/*
    structure of training Data
    allows processing of the data, zB verschiedene Gesten (unterschiedliche Zeit) zusammen packen
    ->Shapes als Array zu genauen adressierung
    bei Rest Reihenfolge egal -> Liste
 */



namespace gestureData {

public class trainingData {
    private int _numShapes = -1;
    private Shape[] _shapes;

    public trainingData() {
        _numShapes = 30;
        _shapes = new Shape[_numShapes];
    }

    public void setShape(int num, Shape s) {
        _shapes[num] = s;
    }

    public Shape getShape(int num) {
        if(num <= _numShapes && _shapes[num]!=null) return _shapes[num];
        Debug.WriteLine("There is no shape named '" + num + "'");
        //oder: _shapes einfach grösser werden lassen
        return null;
    }

    public void mergeShapes(int from, int into) {
        Console.WriteLine("Methode mergeShapes fehlt");
    }



    public class Shape {
        private List<Gesture> _gestures;
        private int _shapeID = -1;

        //for 2D/3D handling:
        static private Boolean _is2D = false;               
        static private Boolean dimensionDecided = false;

        public Shape(int shapeID, Boolean is2D) {
            _shapeID = shapeID;
            _gestures = new List<Gesture>();
            _is2D = is2D;
        } 

        public void addGesture(Gesture newGesture) {
            _gestures.Add(newGesture);
        }

        public List<Gesture> getGestures() {
            return _gestures;
        }

        
        public class Gesture {
            private List<Point> _Points;
            private int _gestureID = -1;

            public Gesture(int gestureID) {
                _gestureID = gestureID;
                _Points = new List<Point>();
            }

            public void addPoint(Point p) {
                _Points.Add(p);
            }

            public List<Point> getPoints() {
                return _Points;
            }

            public int getGestureID() {
                return _gestureID;
            }

            public class Point {
             private int _pointID = -1;
             private float _xPos = -1;
             private float _yPos = -1;
             private float _zPos = -1;

             public Point(int id,float x, float y) {
                 if(!dimensionDecided) {
                      dimensionDecided = true;
                      _is2D = true;
                  }

                  if(_is2D) {
                     _pointID = id;
                     _xPos = x;
                     _yPos = y;
                  } else {
                      Console.WriteLine("Couldn't create 2D Point for a 3D Shape");
                  }
                }
             public Point(int id, float x, float y, float z) {
                 if(!dimensionDecided) {
                      dimensionDecided = true;
                      _is2D = false;
                  }

                 if(!_is2D) {
                     _pointID = id;
                     _xPos = x;
                     _yPos = y;
                     _zPos = z;
                 } else {
                      Console.WriteLine("Couldn't create 3D Point for a 2D Shape");
                 }
             }

             /*public tryCluster.point toClusterPoint() {
                 return new tryCluster.point(this._xPos, this._yPos);
             }*/

             new public String ToString() {
                 String name = "Point ID=" + _pointID + ", x=" + _xPos + ", y=" + _yPos;
                 return name;
             }

             public int ID => _pointID;
             public float X => _xPos;
             public float Y => _yPos;
             public float Z => _zPos;
             }
            }
    }
}

}