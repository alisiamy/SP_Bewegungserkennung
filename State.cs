using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Bewegungserkennung {

    [DataContract]
    class State {
        [DataMember]
        private point center;
        [DataMember]
        private point treshold; 
        [DataMember]
        public int tMin{get; private set;}
        [DataMember]
        public int tMax{get; private set;}
        public State(Cluster c, int k, Shape s) {
            center = c.mean;
            treshold = c.variance.sqroot().mult(k);
            calculateTMinMax(c,s);
        }

        public bool pointInState(point p){
            point normalized = new point(p.x - center.x, p.y - center.y);

            return ((double)(normalized.x * normalized.x) / (treshold.x * treshold.x)) +
                ((double)(normalized.y * normalized.y) / (treshold.y * treshold.y)) <= 1.0;
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