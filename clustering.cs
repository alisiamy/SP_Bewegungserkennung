using System;
using System.Collections.Generic;

namespace tryCluster{
    public struct point{
        public double x, y;
        public point(int px, int py){
            x = px;
            y = py;
        }
    }
    class Cluster{
        
    }

    class KMclustering{
        public int k = 1;
        
        public void createCluster(point ipt){
            HashSet <point> newCL = new HashSet<point>();
            addToCluster(newCL, ipt);
        }
        public void addToCluster(HashSet <point> cl, point ipt){
            cl.Add(ipt);
        }
        public void clustering(point[] ipt){
            HashSet <int> iSet = new HashSet<int>();
            while(iSet.Count < k){
            Random ran = new Random();
            int idx = ran.Next(1,ipt.Length+1);
            if(!iSet.Contains(idx)){
                 iSet.Add(idx);
            }
            }
            foreach(int i in iSet){
                createCluster(ipt[i]);
            }
        }

    }
    }
