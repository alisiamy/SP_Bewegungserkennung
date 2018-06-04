using System;
using System.Collections.Generic;

namespace tryCluster{     

    class KMclustering{
        public int k = 1; // Konstruktor fuer k?
        public LinkedList<Cluster> CLlist;  
        public double mDistance;      
        
        public void clustering(point[] ipt){
            // Initial centroids for clusters according to k
            HashSet <int> iSet = new HashSet<int>();
            while(iSet.Count < k){
            Random ran = new Random();
            int idx = ran.Next(1,ipt.Length+1);
                if(!iSet.Contains(idx)){
                    iSet.Add(idx);
                }
            }
            foreach(int i in iSet){
                CLlist.AddLast(new Cluster(ipt[i]));
            }

            //do {

            //}while();
        }

        //Mahalanobis distance between two points
        public static double mahalanobisDist(point p1, point p2, Cluster c){
            double [] imx = c.KVmatrixinverse();

            point p1im = new point(p1.y*imx[0]+p1.y*imx[1],p1.x*imx[2]+p1.x*imx[3]); // transpone
            
            return Math.Sqrt(p2.x*p1im.x+p2.y*p1im.y);
        }

        //Mahalanobis distance between a point and a cluster
        public static double mahalanobisDist(point p, Cluster c){
            double [] imx = c.KVmatrixinverse();
 
            point tmp = p;
            tmp.subtraction(c.getMean());
            point p1im = new point(tmp.y*imx[0]+tmp.y*imx[1],tmp.x*imx[2]+tmp.x*imx[3]); // transpone
            
            return Math.Sqrt(tmp.x*p1im.x+tmp.y*p1im.y);
        }

    }
    }
