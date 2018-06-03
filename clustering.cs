using System;
using System.Collections.Generic;

namespace tryCluster{
        class Cluster{
        
        static List<point> CL;
        static point variance;
        static point mean;
		static double covariance;
        public Cluster(point ipt){
            mean = ipt;
            addToCluster(CL, ipt);
        }
        private void calculateEV(){
            mean = new point(0,0); // TODO: moegliche Optimierung
            CL.Sort();
            for(int i = 0; i < CL.Count;++i){ // 1 cpu zyklus weniger!
                point tmp = CL[i];
                int counter = 1;
                // heufigkeit
                    while(i+1 <= CL.Count && CL[i+1]==tmp){ 
                    counter++;
                    ++i;
                    }
                //EV
                mean.addition(tmp.mult(counter));
                }
        }
            private void calculateVariance(){
            variance = new point(0,0);
             for(int i = 0; i < CL.Count;++i){
                point tmp = CL[i];
                int counter = 1;
                     // heufigkeit
                    while(i+1 <= CL.Count && CL[i+1]==tmp){ 
                    counter++;
                    ++i;
                    }
                //varianz
				variance.addition(((point.substract(tmp, mean)).power(2.0)).mult(counter));
        }
        }
		private void calculateCV(){
			covariance = 0;
			for (int i = 0; i < CL.Count; ++i){
				covariance += (CL[i].x - mean.x) * (CL[i].y - mean.y);
			}
			covariance /= CL.Count;
         }
         public static double[] KVmatrixinverse(){
            double[] matrix = new double[4];

            double a = (1/CL.Count)*(variance.x-mean.x);
            double bc = (1/CL.Count)*covariance;
            double d = (1/CL.Count)*(variance.y-mean.y);

            double divider = 1/(a*d - bc*bc);

             matrix[0] = divider*d;
             matrix[1] = divider*(bc*(-1.0));  
             matrix[2] = matrix[1];
             matrix[3] = divider*a; 

             return matrix;
         }

        public void addToCluster(List<point> cl, point ipt){ //umschreiben
            cl.Add(ipt);
        }

    }

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
        public void mahalanobisDist(point p1, point p2){
            double [] imx = Cluster.KVmatrixinverse();

            point p1im = new point(p1.y*imx[0]+p1.y*imx[1],p1.x*imx[2]+p1.x*imx[3]); // transpone
            
            mDistance = Math.Sqrt(p2.x*p1im.x+p2.y*p1im.y);
        }

    }
    }
