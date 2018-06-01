
using System;
using System.Collections.Generic;
using  System.Linq;


namespace tryCluster
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            tryCluster.point p = new tryCluster.point(3,4);
            tryCluster.point p2 = new tryCluster.point(4,9);
            tryCluster.point p3 = new tryCluster.point(3,18);
            tryCluster.point p4 = new tryCluster.point(2,1);
            tryCluster.point p5 = new tryCluster.point(29,7);
            
            point [] prup = {p, p2, p3, p4, p5};

            List <point> pset = new List <point>();
            pset.AddRange(prup);

            pset.Sort();

            foreach(point n in pset){
                Console.WriteLine("{0}", n.ToString());
            }

        }
    }
}
