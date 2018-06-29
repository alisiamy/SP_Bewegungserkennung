﻿
using System;
using System.Linq;
using System.Collections.Generic;


namespace Bewegungserkennung
{
    class Program
    {
        static void Main(string[] args)
        {
            dataReader d = new dataReader("D:/VL/swGesten/KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();
            KMclustering k = new KMclustering(shapes[1], new point(100,10),2,1);
            k.clustering();
        }
    }
}

