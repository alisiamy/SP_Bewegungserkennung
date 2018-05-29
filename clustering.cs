using System;
using System.Collections.Generic;

namespace tryCluster{
    public struct point{
        public double x, y;
        public point(double px, double py){
            x = px;
            y = py;
        }
        public void add(point ipt){
            this.x += ipt.x;
            this.y += ipt.y;
        }
        public void divide(int c){
            this.x /= c;
            this.y /= c;
        }
    }
    class Cluster{

        HashSet <point> CL;
        point mean;
        public Cluster(point ipt){
            mean = ipt;
            addToCluster(CL, ipt);
        }

        public void calclulateMean(){
            point tmpSum = new point(0,0);
            foreach(point n in CL){
                tmpSum.add(n);
            }
            tmpSum.divide(CL.Count);
        }
        public void addToCluster(HashSet <point> cl, point ipt){
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

            do {

            }while();
        }

    }
    }
