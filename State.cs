using System;
using System.Collections.Generic;

namespace Bewegungserkennung {
    class State {
        private point center;
        private point treshold; //double treshold;
        public int tMin{get; private set;}
        public int tMax{get; private set;}
        public State(Cluster c, int k, Shape s) {
            center = c.mean;
            treshold = c.variance.sqroot().mult(k);
            calculateTMinMax(c,s);
        }

        public bool pointInState(point p){
            return point.abs(point.substract(center,p)).CompareTo(treshold) <= 0;
        }
        private void calculateTMinMax(Cluster c, Shape s){
            tMin = Int32.MaxValue;
            tMax = 0;
            foreach(Gesture g in s.getGestures()){
                int counter = 0;
                foreach(point p in g.Points){
                    if(pointInState(p)){
                        ++counter;  //cpu-zyklus gerettet!
                    }
                }
                if(counter < tMin){
                    tMin = counter;
                    continue;
                }
                if(counter> tMax){
                    tMax = counter;
                }
            }
            if (tMin == 0)
                ++tMin;
        }
        public double distance(point p){
            return center.distance(p);
        }

    }
}