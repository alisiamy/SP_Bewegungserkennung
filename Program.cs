
using System;
using System.Linq;
using System.Collections.Generic;


namespace Bewegungserkennung
{
    class Program
    {
        static void Main(string[] args)
        {
            dataReader d = new dataReader("C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/KinectDaten_Pascal.csv");
            List<Shape> shapes = d.readData();
            int shapeNumber = 1;
            KMclustering k = new KMclustering(shapes[shapeNumber], new point(100,10),2,1);
            k.clustering();

            //zeitliche Information
            FSM stateMachine = new FSM(k, shapes[shapeNumber]);
        }
    }
}

