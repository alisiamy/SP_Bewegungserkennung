﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace SP_Bewegungserkennung
{
    /**
    *   Class Geture, which describes a recoreded gesture consiting of points
    */
    public class Gesture
    {
        public List<point> Points { get; set; }
        public int gestureID { get; private set; }

        public Gesture(int gestureID, List<point> p)
        {
            this.gestureID = gestureID;
            Points = p;
            Points.Sort(new pointComparer());
        }

        public void Add(point p)
        {
            int idx = Points.BinarySearch(p, new pointComparer());
            if (idx < 0)
                idx = ~idx;
            Points.Insert(idx, p);
        }
    }
}