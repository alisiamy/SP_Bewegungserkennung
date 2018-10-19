using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace SP_Bewegungserkennung
{
    /**
    *Class FSM discribes a Finite State Machine created out of states and
    * the further recognition of the gestures
    */
    [DataContract]
    class FSM
    {
        [DataMember]
        public List<State> stateList { get; private set; }
        private int currentState = 0;
        private int time = 0;
        double sumPointStateDistances = 0;
        int pointsCounted = 0;

        /**
        * Enum Status describes three phases of the recognition
        */
        public enum Status
        {
            RECOGNIZED,
            FAILED,
            RECOGNIZING
        }
        /**
       * Constructor of the FSM with result of the clustering, shape for temporal information calculation and tresholdMultiplier wihich 
       * indirectly affects recognition rate
       */
        public FSM(KmeansClustering cl, Shape s, int tresholdMultuplier)
        {
            stateList = clusterToState(orderClusters(cl.ClusterList, s), tresholdMultuplier, s);
        }

        private List<Cluster> orderClusters(List<Cluster> CList, Shape s)
        {
            List<List<int>> gestureClusterIdx = new List<List<int>>();
            foreach (Gesture g in s.getGestures())
            {
                List<int> idxList = new List<int>();
                foreach (point p in g.Points)
                {
                    double minDist = CList[0].euclideanDist(p);
                    int idx = 0;
                    for (int i = 1; i < CList.Count; i++)
                    {
                        double tmpDist = CList[i].euclideanDist(p);
                        if (tmpDist < minDist)
                        {
                            minDist = tmpDist;
                            idx = i;
                        }
                    }
                    if (idxList.Count == 0)
                        idxList.Add(idx);
                    else if (idx != idxList[idxList.Count - 1])
                        idxList.Add(idx);
                }
                gestureClusterIdx.Add(idxList); 
            }
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


        /**
        * XML serialisation of a FSM
        */
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
        /**
         * Tick function is a function for the excution of a FSM
         * can be also used for live recognition
         */
        public FSM.Status tick(point livePoint)
        {
            if (currentState == stateList.Count - 1)
            {
                return Status.RECOGNIZED;

            }
            if (!stateList[currentState].pointInState(livePoint) && currentState == 0 && time == 0) //FSM is not active
            { 
                return Status.FAILED;
            }
            else if (stateList[currentState + 1].pointInState(livePoint)) // point is in the next state
            { 
                if (time >= stateList[currentState].tMin && time <= stateList[currentState].tMax)
                {
                    currentState++;
                    time = 1;
                    sumPointStateDistances += distancePointState(livePoint, currentState);
                    pointsCounted++;

                    return Status.RECOGNIZING;
                }
                else if (time > stateList[currentState].tMax)
                {

                    currentState = 0;
                    time = 0;
                    return Status.FAILED;
                }
                else
                {                                         

                    currentState = 0;
                    time = 0;
                    return Status.FAILED;
                }
            }
            else if (stateList[currentState].pointInState(livePoint)) //point is in state
            {
                sumPointStateDistances += distancePointState(livePoint, currentState);
                pointsCounted++;

                if (time <= stateList[currentState].tMax) 
                { 
                    ++time;
                    return Status.RECOGNIZING;
                }
                else if (time > stateList[currentState].tMax) //temporal parameters are exceeded
                { 
                    currentState = 0;
                    time = 1;
                    return Status.FAILED;
                }
                else
                {                                        
                    time++;
                    return Status.FAILED;
                }
            }
            else
            {

                return Status.FAILED;
            }

        }

        private double distancePointState(point point, int currentState) {
            return stateList[currentState].center.distance(point);
        }

        public double getSumStateDistances() {
            return sumPointStateDistances / pointsCounted;
        }

        public void reset()
        {
            currentState = 0;
            time = 0;
        }
        /**
         * Recognize function preformes a recognition offline
        */
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