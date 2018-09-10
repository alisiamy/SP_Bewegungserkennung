using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace Bewegungserkennung
{     

    class KMclustering
    {
        public int k {get; private set; }
        public List<Cluster> CLlist {get; private set;}
        private List<point> PList;  
        private point sigmaNull;
        private double epsilon;

        public KMclustering(Shape s, point sNull, int k=2, double epsilon = 0.5)
        {
            this.k = k;
            this.sigmaNull = sNull;
            this.epsilon = epsilon;

            PList = new List<point>(); 
            
            foreach(Gesture g in s.getGestures())
                PList.AddRange(g.Points);

            CLlist = new List<Cluster>();
            for (int i = 0; i < k; ++i)
            {
                CLlist.Add(new Cluster(PList.GetRange(i*PList.Count/k, Math.Min(PList.Count/k, PList.Count - i*PList.Count/k)))); 
            }
        } 

        private point maxVariance()
        {
            point maxVariance = new point(0,0);
            foreach (Cluster c in CLlist)
            {
                if (c.variance.CompareTo(maxVariance) > 0)
                    maxVariance = c.variance;
            }
            return maxVariance;
        }
        
        public void clustering()
        {

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
                        double minDist = CLlist[0].euclideanDist(p);
                        Debug.Assert(!Double.IsNaN(minDist));
                        int DistX = 0;

                        for(int j = 1; j < CLlist.Count; ++j)
                        {
                            double tmpDist = CLlist[j].euclideanDist(p);
                            Debug.Assert(!Double.IsNaN(tmpDist));
                            
                            if(tmpDist < minDist)
                            {
                                minDist = tmpDist;
                                DistX = j;
                            }
                        }
                        CLlist[DistX].addToCluster(p);
                    }

                    foreach(Cluster c in CLlist)
                    {
                        //Debug.Assert(c.getPoints().Count > 0);
                        point oldMean = new point(c.mean);
                        c.updateCluster();
                        change |= !(c.mean.distance(oldMean) < epsilon);
                    }
                }while(change);
                
                curVariance = maxVariance();

                if (sigmaNull.CompareTo(curVariance) < 0)
                {
                    k++;
                    int i=0;
                    while(!CLlist[i].variance.Equals(curVariance))
                        ++i;
                    
                    CLlist.Add(CLlist[i].split());
                }

            } while(sigmaNull.CompareTo(curVariance)<0);
        }
    }
}