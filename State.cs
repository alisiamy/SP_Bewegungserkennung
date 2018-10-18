using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace SP_Bewegungserkennung {

    [DataContract]
    class State {
        [DataMember]
        public point center { get; private set; }
        [DataMember]
        public point treshold { get; private set; }
        [DataMember]
        public int tMin { get; private set; }
        [DataMember]
        public int tMax { get; private set; }


        public State(Cluster c, int k, Shape s) {
            center = c.mean;
            treshold = c.variance.sqroot().mult(k);
        }

        public State(Cluster c, int k, Shape s, int tmi, int tma) {
            center = c.mean;
            treshold = c.variance.sqroot().mult(k);
            tMin = tmi;
            tMax = tma;
        }

        public bool pointInState(point p) {
            return point.abs(point.substract(p, center)).compareXY(treshold);
        }

        public static void calculateTMinMax(Shape s, List<State> SL) //for each state
        {

            foreach (State st in SL) {
                st.tMin = Int32.MaxValue;
                st.tMax = 0;
            }

            HashSet<point>[] tmpSet = new HashSet<point>[SL.Count];

            for (int h = 0; h < tmpSet.Length; ++h) {
                tmpSet[h] = new HashSet<point>();
            }

            foreach (Gesture g in s.getGestures()) {
                foreach (point p in g.Points) {
                    for (int i = 0; i < SL.Count; ++i) {
                        if (SL[i].pointInState(p)) {
                            tmpSet[i].Add(p); //count points in each state
                        }
                    }
                }
                for (int j = 0; j < SL.Count; ++j) {
                    HashSet<point> hstmp = new HashSet<point>(tmpSet[j]);
                    SL[j].tMax = SL[j].tMax > hstmp.Count ? SL[j].tMax : hstmp.Count;
                    for (int k = 0; k < SL.Count; ++k) {
                        if (k != j)
                            hstmp.ExceptWith(tmpSet[k]);
                    }

                    SL[j].tMin = SL[j].tMin < hstmp.Count ? SL[j].tMin : hstmp.Count; //tmin has to be >0
                }
            }
        }

        public double distance(point p) //distance from point to cluster center
        {
            return center.distance(p);
        }

    }
}