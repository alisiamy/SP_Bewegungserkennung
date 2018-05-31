using System;
using System.Collections.Generic;

namespace tryCluster{
        class Cluster{

        List<point> CL;
        point mean;
        public Cluster(point ipt){
            mean = ipt;
            addToCluster(CL, ipt);
        }

        public void calculateEV(){
            CL.Sort();
            for(int i = 0; i < CL.Count;++i){ // 1 cpu zyklus weniger!
                point tmp = CL[i];
                int counter = 1;
                    while(i+1 <= CL.Count && CL[i+1]==tmp){
                    counter++;
                    ++i;
                    }
                mean.addition(tmp.mult(counter));
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
