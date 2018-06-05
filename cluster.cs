using System;
using System.Collections.Generic;

namespace tryCluster{     
 class Cluster{
        
        private List<point> CL;
        private point variance;
        private point mean;
		private double covariance;

        public Cluster(point ipt){
            mean = ipt;
            CL = new List<point>();
            addToCluster(ipt);
        }

        public point getMean()
        {
            return this.mean;
        }
        public point getVariance(){
            return this.variance;
        }

        public void clearCluster(){
            CL.Clear();
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
         
         private double[] KVmatrixinverse(){
            double[] matrix = new double[4];

            double a = (1/CL.Count)*(variance.x-mean.x);
            double bc = (1/CL.Count)*covariance;
            double d = (1/CL.Count)*(variance.y-mean.y);

            double divider = a*d - bc*bc;

             matrix[0] = d/divider;
             matrix[1] = -bc/divider;  
             matrix[2] = matrix[1];
             matrix[3] = a/divider; 

             return matrix;
         }

         public void updateCluster(){
             this.calculateEV();
             this.calculateVariance(); 
             this.calculateCV();
             this.KVmatrixinverse();

         }
        public void addToCluster(point ipt){
            CL.Add(ipt);
        }
        //Mahalanobis distance between a point and a cluster
        public double mahalanobisDist(point p){
            double [] imx = this.KVmatrixinverse();
 
            point tmp = p;
            tmp.subtraction(this.getMean());
            point p1im = new point(tmp.y*imx[0]+tmp.y*imx[1],tmp.x*imx[2]+tmp.x*imx[3]); // transpone
            
            return Math.Sqrt(tmp.x*p1im.x+tmp.y*p1im.y);
        }

    }
}