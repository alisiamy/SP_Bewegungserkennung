using System;
using System.Collections.Generic;

namespace tryCluster{
        class Cluster{

        List<point> CL;
        point variance;
        point mean;
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
                variance.addition(point.substract(variance, mean));
        }

        }

        public void addToCluster(List<point> cl, point ipt){ //umschreiben
            cl.Add(ipt);
        }

    }

    class KMclustering{
        public int k = 1; // Konstruktor fuer k?
        public LinkedList<Cluster> CLlist;
        
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
        public void mahalanobisDist(){

        }

    }
    }
