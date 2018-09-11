using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace Bewegungserkennung {

[DataContract]
class FSM {
        [DataMember]
        private List<State> stateList;
        private int currentState = 0;
        private int time = 0;

         public enum Status{
            RECOGNIZED,
            FAILED,
            RECOGNIZING
            }       

        public FSM(KMclustering cl, Shape s, int k) {
            stateList = clusterToState(orderClusters(cl.CLlist,s),k, s);            
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

        //serialization

        public static void serialize(FSM f, string filepath){

            DataContractSerializer ser = new DataContractSerializer(typeof(FSM));
            using(FileStream fs = File.Create(filepath)){
            using(XmlWriter xw = XmlWriter.Create(fs)){
                ser.WriteObject(xw, f);
            }
            }
        }

        public static FSM deserialize(string filepath){

            DataContractSerializer ser = new DataContractSerializer(typeof(FSM));

            using(FileStream fs = new FileStream(filepath, FileMode.Open)){

                XmlDictionaryReader dr = XmlDictionaryReader.CreateTextReader(fs,  new XmlDictionaryReaderQuotas());
                FSM f = (FSM)ser.ReadObject(dr, true);
                return f;
            }
        } 


        //for gesture recognition
        public FSM.Status tick(point livePoint) {
            if(currentState == stateList.Count-1){
                Console.WriteLine("Gesture is recognized");

                return Status.RECOGNIZED;
                
            }
            if(!stateList[currentState].pointInState(livePoint) && currentState == 0 && time == 0){ //FSM ist nicht aktiviert

                return Status.FAILED;
            }else if(stateList[currentState].pointInState(livePoint)){
                if (time>= stateList[currentState].tMin && time<= stateList[currentState].tMax){ //point ist im State
                     
                    return Status.RECOGNIZING;
                }else if( time> stateList[currentState].tMax){ //Zeit wird ueberschritten

                    currentState = 0;
                    time = 1;
                    return Status.FAILED;
                }else{                                         //if( time< stateList[currentState].tMin)

                    time++;
                    return Status.FAILED;
                }
            }else if(stateList[currentState+1].pointInState(livePoint)){ // point befindet sich im naechsten State
                if(time>= stateList[currentState].tMin && time<= stateList[currentState].tMax){

                    currentState++;
                    time = 1;
                    return Status.RECOGNIZING;
                }else if(time> stateList[currentState].tMax) {

                    currentState = 0;
                    time = 0;
                    return Status.FAILED;
                }else {                                          //if(time< stateList[currentState].tMin)

                    currentState = 0;
                    time = 0;
                    return Status.FAILED;
                }
            }else{

                return Status.FAILED;
            }

        }

        public void reset(){
            currentState =0;
            time = 0;
        }

        public bool recognize(Gesture g){

            Status s = Status.FAILED;

                foreach(point p in g.Points){
                     s = tick(p);
                }

            reset();

            return s == Status.RECOGNIZED;
        }

}
}