using System;
using System.Collections.Generic;

namespace tryCluster{
    public class point: IComparable<point>, IEquatable<point>{
        private double x, y;
        public point(double px, double py){
            x = px;
            y = py;
        }
        public void addition(point ipt){
            this.x += ipt.x;
            this.y += ipt.y;
        }
        public void divide(int c){
            this.x /= c;
            this.y /= c;
        }
        public int CompareTo(point comparepoint)
        {
        if (comparepoint == null){
            return 1;
        }
        else{
                if(this.x == comparepoint.x){
                    return this.y.CompareTo(comparepoint.y);
                }else{
                    return this.x.CompareTo(comparepoint.x);
                }
        }
    }
    }
    class Cluster{

        LinkedList<point> CL;
        point mean;
        public Cluster(point ipt){
            mean = ipt;
            addToCluster(CL, ipt);
        }

        public void calclulateEV(){
            point tmpSum = new point(0,0);
            foreach(point n in CL){
                tmpSum.addition(n);
            }
            tmpSum.divide(CL.Count);
        }
        public void calculateWeight(){

        }
        public void addToCluster(LinkedList<point> cl, point ipt){
            cl.AddLast(ipt);
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
