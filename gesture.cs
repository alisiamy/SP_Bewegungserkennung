using System;
using System.Collections;
using System.Collections.Generic;

namespace Bewegungserkennung 
{

    public class Gesture 
    {
        public List<point> Points {get; private set; }
        public int gestureID {get; private set; }

        public Gesture(int gestureID, List<point> p) 
        {
            this.gestureID = gestureID;
            Points = p;
        }

        public void Add(point p) 
        {
            Points.Add(p);
        }
    }
}