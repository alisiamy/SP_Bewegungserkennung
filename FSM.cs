using System;
using System.Linq;
using System.Collections.Generic;

namespace Bewegungserkennung {
    class FSM {
        List<State> stateList;
        int currentState = -1;
        int currentStatePointCount = 0;
        

        public FSM(KMclustering k, Shape s) {
            int numberstateList = k.numberClusters();
            List<Cluster> clusterList = k.getClusterList();
            stateList = new List<State>();

            //first state
            Gesture orderGesture = s.getGesture(1);
            int clusterNumber = findPointInClusters(orderGesture.Points[0], clusterList);
            stateList.Add(new State(clusterList[clusterNumber], clusterNumber));

            //get state order
            foreach(point p in orderGesture.Points) {
                int currentCluster = findPointInClusters(p, clusterList);
                if(currentCluster != clusterNumber) {
                    stateList.Add(new State(clusterList[currentCluster], currentCluster));
                    clusterNumber = currentCluster;
                }
            }


            //add tMin and tMax for each state
            foreach(Gesture g in s.getGestures()) {
                int pointCounter = 0;
                int stateNumber = 0;

                foreach(point p in g.Points) {
                    Boolean stillInState = findPointInState(p, stateList[stateNumber]);
                    if(stillInState) {
                        pointCounter++;
                    }
                    else {
                        if(stateList[stateNumber].tMax<pointCounter) stateList[stateNumber].tMax=pointCounter;
                        if(stateList[stateNumber].tMin>pointCounter) stateList[stateNumber].tMin=pointCounter;
                        pointCounter = 1;
                        stateNumber++;
                    }
                }
            }
        }

        //for gesture recognition
        public Boolean tick(point livePoint) {
        if(currentState == -2)  return false; //"FSM is eliminated from consideration"

            if(isInRegion(livePoint, currentState+1)) {
                if(currentStatePointCount > stateList[currentState].tMax) {
                    currentState++;
                } else if( livePoint.distance(stateList[currentState].center) < 
                            livePoint.distance(stateList[currentState].center) &&
                            currentStatePointCount > stateList[currentState].tMin) {
                                currentState++;
                } else if(!isInRegion(livePoint, currentState)) {
                    currentState++;
                }
            }
            else { //"FSM is eliminated from consideration"
                currentState = -2;
            }

            if(currentState == stateList.Count-1) return true;

            return false;
        }

        private Boolean isInRegion(point p, int stateID) {
            /* checks, if Distance(point, stateCenter) < stateTreshold
                => if point is in the region of the state */

            if(stateID < 0) return false;

            
            //TODO: implement

            return false;
        }

        private int findPointInClusters(point pointWeLookFor, List<Cluster> c) {
            int l = c.Count;
            for(int i=0; i<l; i++) {
                foreach(point clusterPoint in c[i].getPoints()) {
                    if(pointWeLookFor.CompareTo(clusterPoint) == 0) {
                        return i; //return number of cluster where we found the point
                    }
                }
            }
            
            return -1;
        }

        private Boolean findPointInState(point pointWeLookFor, State s) {
            foreach(point clusterPoint in s.pointList) {
                if(pointWeLookFor.CompareTo(clusterPoint) == 0) {
                    return true; //return number of cluster where we found the point
                }
            }
            return false;
        }
    }

    class State {
        public point center;
        public point variance;
        //double treshold;
        public int tMin;
        public int tMax;
        public List<point> pointList;
        public int clusterID;

        public State(Cluster c, int clusterNumber) {
            center = c.mean;
            variance = c.variance;
            pointList = c.getPoints();
            tMin = int.MaxValue;
            tMax = int.MinValue;
            clusterID = clusterNumber;
        }
    }
}