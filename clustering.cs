using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace tryCluster{     

    class KMclustering{
        private int k; 
        private List<Cluster> CLlist;
        private List<point> PList;  
        private point sigmaNull;
        private double epsilon;

        public KMclustering(List<point> pl, point sNull, int k=2, double epsilon = 0.5){
                this.k = k;
                this.sigmaNull = sNull;
                this.epsilon = epsilon;
                this.PList = pl;
                CLlist = new List<Cluster>();
                for (int i = 0; i < k; ++i)
                {
                        CLlist.Add(new Cluster(pl.GetRange(i*pl.Count/k, Math.Min(pl.Count/k, pl.Count - i*pl.Count/k)))); 
                }
        } 

        private point maxVariance()
        {
            point maxVariance = new point(0,0);
            foreach (Cluster c in CLlist)
            {
                if (c.getVariance().CompareTo(maxVariance) > 0)
                    maxVariance = c.getVariance();
            }
            return maxVariance;
        }
        
        public void clustering(){

            point curVariance = maxVariance();

            do{
                bool change;

                //k-means
                do {

                    foreach(Cluster c in CLlist){
                        c.clearCluster();
                    }

                    change = false;

                    foreach(point p in PList){

                        //sometimes the mahalanobis distance becomes NaN, why is that?
                        double minDist = CLlist[0].mahalanobisDist(p);
                        Debug.Assert(!Double.IsNaN(minDist));
                        int DistX = 0;

                        for(int j = 1; j < CLlist.Count; ++j){

                            double tmpDist = CLlist[j].mahalanobisDist(p);
                            Debug.Assert(!Double.IsNaN(tmpDist));
                            
                            if(tmpDist < minDist){
                                minDist = tmpDist;
                                DistX = j;
                            }
                        }
                        CLlist[DistX].addToCluster(p);
                    }

                    foreach(Cluster c in CLlist){
                        Debug.Assert(c.getPoints().Count > 0);
                        point oldMean = new point(c.getMean());
                        c.updateCluster();
                        change |= !(c.getMean().distance(oldMean) < epsilon);
                    }
                }while(change);
                
                curVariance = maxVariance();

                if (sigmaNull.CompareTo(curVariance) < 0)
                {
                    k++;
                    int i=0;
                    while(!CLlist[i].getVariance().Equals(curVariance))
                        ++i;
                    
                    CLlist.Add(CLlist[i].split());
                }

            } while(sigmaNull.CompareTo(curVariance)<0);
        }

    }
}