using System;
using System.Collections.Generic;

namespace tryCluster{     

    class KMclustering{
        private int k; 
        private List<Cluster> CLlist;
        private List<point> PList;  
        private point sigmaNull;

            public KMclustering(List<point> pl, int k, point sNull){
                this.k = k;
                this.sigmaNull = sNull;
                this.PList = pl;
                CLlist = new List<Cluster>();
        } 
        
        public void clustering(){

            point maxVariance = new point(0,0);

            do{
                // Initial centroids for clusters according to k

                HashSet <int> iSet = new HashSet<int>();
                while(iSet.Count < k){
                Random ran = new Random();
                int idx = ran.Next(1,PList.Count);
                    if(!iSet.Contains(idx)){
                        iSet.Add(idx);
                    }
                }
                foreach(int i in iSet){
                    CLlist.Add(new Cluster(PList[i]));
                    CLlist[CLlist.Count-1].updateCluster();
                }

                bool change;

                do {

                    foreach(Cluster c in CLlist){
                    c.clearCluster();
                    }

                    change = false;

                    foreach(point p in PList){

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
                    change = change || !point.pround(oldMean).Equals(point.pround(c.getMean()));
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