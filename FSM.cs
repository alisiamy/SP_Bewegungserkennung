using System;
using System.Linq;
using System.Collections.Generic;

namespace Bewegungserkennung {
class FSM {
        List<State> stateList;
        int currentState = -1;
        int currentStatePointCount = 0;
        

        public FSM(KMclustering cl, Shape s, int k) {
            stateList = clusterToState(orderClusters(cl.CLlist,s),k, s);            
        }

        //for gesture recognition
        public Boolean tick(point livePoint) {
        if(currentState == -2)  return false; //"FSM is eliminated from consideration"

            if(this.stateList[currentState+1].pointInState(livePoint)) {
                if(currentStatePointCount > stateList[currentState].tMax) {
                    currentState++;
                } else if(stateList[currentState].distance(livePoint) < 
                            stateList[currentState].distance(livePoint) &&
                            currentStatePointCount > stateList[currentState].tMin) {
                                currentState++;
                } else if(!this.stateList[currentState].pointInState(livePoint)) {
                    currentState++;
                }
            }
            else { //"FSM is eliminated from consideration"
                currentState = -2;
            }

            if(currentState == stateList.Count-1) return true;

            return false;
        }

        private List<Cluster> orderClusters(List<Cluster> CList, Shape s)
        {
            List<List<int>> gestureClusterIdx = new List<List<int>>();
            foreach (Gesture g in s.getGestures()){
                List<int> idxList = new List<int>();
                foreach (point p in g.Points){
                    double minDist = CList[0].euclideanDist(p);
                    int idx = 0;
                    for(int i = 1; i < CList.Count; i++){
                        double tmpDist = CList[i].euclideanDist(p); 
                        if(tmpDist< minDist){
                            minDist = tmpDist;
                            idx = i;
                        }
                    }
                    if(idxList.Count == 0)
                        idxList.Add(idx);
                    else if(idx != idxList[idxList.Count - 1])
                        idxList.Add(idx);
                }
                gestureClusterIdx.Add(idxList);  //error??
            }
            List<Cluster> result = new List<Cluster>();
            for(int i=0; i < gestureClusterIdx[0].Count; i++){
                int[] voting = new int[CList.Count];
                foreach(List<int> l in gestureClusterIdx){
                    //workaround for different gestures with different lengths
                    try {
                        ++voting[l[i]];
                    }
                    catch(System.ArgumentOutOfRangeException e){
                    };
                }
                int maxIdx = 0;
                int maxfame = voting[0];
                for(int j = 1; j < voting.Length; ++j){
                    if(maxfame<voting[j]){
                        maxfame = voting[j];
                        maxIdx = j;
                    }
                } 
                result.Add(CList[maxIdx]);
            }
            return result;
        }

        public List<State> clusterToState(List<Cluster> CList, int k, Shape s){
            List <State> result = new List<State>();
            foreach(Cluster c in CList){ 
                result.Add(new State(c,k,s));
            }
            return result;
        }
    }
}