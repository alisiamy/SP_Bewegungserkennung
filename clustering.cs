using System;
using System.Collections.Generic;

namespace tryCluster{     

    class KMclustering{
        private int k; 
        private List<Cluster> CLlist;  
        private point sigmaNull;
        private List<point> PList;

            public KMclustering(int k, List<point> pointL, point sNull){
                this.k = k;
                this.PList = pointL;
                this.sigmaNull = sNull;
                CLlist = new List<Cluster>();
        } 
        
        public void clustering(point[] ipt){

            point maxVariance = new point(0,0);

            do{
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
                    CLlist.Add(new Cluster(ipt[i]));
                    CLlist[CLlist.Count-1].updateCluster();
                }

                bool change;

                do {

                    foreach(Cluster c in CLlist){
                    c.clearCluster();
                    }

                    change = false;

                    foreach(point p in ipt){

                        double minDist = CLlist[0].mahalanobisDist(p);
                        int DistX = 0;

                        for(int j = 1; j < CLlist.Count; ++j){

                            double tmpDist = CLlist[j].mahalanobisDist(p);

                            if(tmpDist < minDist){
                                minDist = tmpDist;
                                DistX = j;
                            }
                        }
                    CLlist[DistX].addToCluster(p);
                    }
                foreach(Cluster c in CLlist){
                    point oldMean = c.getMean();
                    c.updateCluster();
                    change |= !oldMean.Equals(c.getMean());
                }
                }while(change);
                
                k++;

                foreach(Cluster c in CLlist){

                    if(c.getVariance().CompareTo(maxVariance)>0){
                        maxVariance = c.getVariance();
                    }
                }

            }while(sigmaNull.CompareTo(maxVariance)<0);
        }

    }
    }