using System;
using System.Collections.Generic;

namespace tryCluster{     

    class KMclustering{
        public int k = 1; // Konstruktor fuer k?
        public List<Cluster> CLlist;  
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
                CLlist.Add(new Cluster(ipt[i]));
            }

            bool change;

            do {
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
        }
    }
    }
