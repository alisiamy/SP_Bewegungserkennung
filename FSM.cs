using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace SP_Bewegungserkennung
{

    [DataContract]
    class FSM
    {
        [DataMember]
        public List<State> stateList { get; private set; }
        private int currentState = 0;
        private int time = 0;

        public enum Status
        {
            RECOGNIZED,
            FAILED,
            RECOGNIZING
        }

        //one FSM for each shape, cluster are states
        public FSM(KMclustering cl, Shape s, int k)
        {
            //cluster in time order -> list of states which build the FSM
            stateList = clusterToState(orderClusters(cl.CLlist, s), k, s);
        }

        //calculate for each point of each gesture the matching cluster (minDis)
        private List<Cluster> orderClusters(List<Cluster> CList, Shape s)
        {
            //list contains the order of clusters as number (index) for each gesture of one shape as a list
            List<List<int>> gestureClusterIdx = new List<List<int>>();
            
            foreach (Gesture g in s.getGestures())
            {
                //list contains the index of CList in temporal order
                List<int> idxList = new List<int>();
               
                foreach (point p in g.Points)
                {
                    double minDist = CList[0].euclideanDist(p); //could be mahalanobis distance, minimal Distance to one Cluster
                    int idx = 0; //index of CList
                    for (int i = 1; i < CList.Count; i++)
                    {
                        double tmpDist = CList[i].euclideanDist(p);
                        if (tmpDist < minDist) 
                        {
                            minDist = tmpDist;
                            idx = i; //Cluster i
                        }
                    }
                    //first entry
                    if (idxList.Count == 0)
                        idxList.Add(idx);
                    //if point matches to next cluster -> add next index 
                    else if (idx != idxList[idxList.Count - 1])
                        idxList.Add(idx);
                }
                gestureClusterIdx.Add(idxList);  
            }
            //list result contains the order of the cluster with temporal information
            List<Cluster> result = new List<Cluster>();
            for (int i = 0; i < gestureClusterIdx[0].Count; i++)
            {
                int[] voting = new int[CList.Count];
                foreach (List<int> l in gestureClusterIdx)
                {
                    //workaround for different gestures with different lengths
                    try
                    {
                        ++voting[l[i]];
                    }
                    catch (System.ArgumentOutOfRangeException e)
                    {
                    };
                }
                int maxIdx = 0; 
                int maxfame = voting[0]; 
                for (int j = 1; j < voting.Length; ++j)
                {
                    if (maxfame < voting[j])
                    {
                        maxfame = voting[j];
                        maxIdx = j; 
                    }
                }
                result.Add(CList[maxIdx]);
            }

            return result;
        }

        //list of states for one shape, each state contains a Cluster, k, shape
        public List<State> clusterToState(List<Cluster> CList, int k, Shape s)
        {
            List<State> result = new List<State>();
            result.Add(new State(CList[0], k, s, 1, Int32.MaxValue));

            for (int i = 1; i < CList.Count - 2; ++i) {
                result.Add(new State(CList[i], k, s));
            }

            result.Add(new State(CList[CList.Count-1], k, s, 1, Int32.MaxValue));
            State.calculateTMinMax(s, result);
            return result;
        }


        //serialization
        public static void serialize(FSM f, string filepath)
        {

            DataContractSerializer ser = new DataContractSerializer(typeof(FSM));
            using (FileStream fs = File.Create(filepath))
            {
                using (XmlWriter xw = XmlWriter.Create(fs))
                {
                    ser.WriteObject(xw, f);
                }
            }
        }

        public static FSM deserialize(string filepath)
        {

            DataContractSerializer ser = new DataContractSerializer(typeof(FSM));

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {

                XmlDictionaryReader dr = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                FSM f = (FSM)ser.ReadObject(dr, true);
                return f;
            }
        }


        //for gesture recognition
        public FSM.Status tick(point livePoint)
        {
            if (currentState == stateList.Count - 1)
            {
                Console.WriteLine("Gesture is recognized"); //each state is recognized

                return Status.RECOGNIZED;

            }
            if (!stateList[currentState].pointInState(livePoint) && currentState == 0 && time == 0)
            { //FSM was not activated

                return Status.FAILED;
            }
            else if (stateList[currentState + 1].pointInState(livePoint))
            { // point is in next state
                if (time >= stateList[currentState].tMin && time <= stateList[currentState].tMax)
                {

                    currentState++;
                    time = 1;
                    return Status.RECOGNIZING; //next state
                }
                else if (time > stateList[currentState].tMax)
                {

                    currentState = 0;
                    time = 0;
                    return Status.FAILED; //duration in one state was too long
                }
                else
                {                                          //if(time< stateList[currentState].tMin)

                    currentState = 0; 
                    time = 0;
                    return Status.FAILED; //set time and state to 0
                }
            }
            else if (stateList[currentState].pointInState(livePoint))
            {
                if (time <= stateList[currentState].tMax)
                { //point is in State

                    ++time;
                    return Status.RECOGNIZING;
                }
                else if (time > stateList[currentState].tMax)
                { //gesture needs more time in state than "allowed"

                    currentState = 0;
                    time = 1;
                    return Status.FAILED;
                }
                else
                {                                         //if( time< stateList[currentState].tMin)

                    time++;
                    return Status.FAILED;
                }
            }
            else
            {

                return Status.FAILED;
            }

        }

        public void reset()
        {
            currentState = 0;
            time = 0;
        }

        public bool recognize(Gesture g)
        {

            Status s = Status.FAILED;

            foreach (point p in g.Points)
            {
                s = tick(p);
            }

            reset();

            return s == Status.RECOGNIZED;
        }

    }
}