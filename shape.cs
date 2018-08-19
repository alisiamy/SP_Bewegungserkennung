using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace Bewegungserkennung 
{

    public class Shape 
    {
        private Dictionary<int,Gesture> gestures;
        private int shapeID;
        public List<int> gestureIDList= new List<int>();

        public Shape(int shapeID, Gesture g) 
        {
            this.shapeID = shapeID;
            this.gestures = new Dictionary<int,Gesture>();
            this.gestures.Add(g.gestureID,g);
            gestureIDList.Add(g.gestureID);
        }

        public void Add(int gestureID, Gesture newGesture) 
        { 
            gestures.Add(gestureID, newGesture);
            gestureIDList.Add(gestureID);
        }

        public bool removeGesture(int gestureID) { //for testing
            if(!this.ContainsGesture(gestureID)) return false;
            return this.gestures.Remove(gestureID);
        }

        public Gesture getGesture(int gestureID)
        {
            Gesture g;
            gestures.TryGetValue(gestureID, out g);
            return g;
        }

        public List<Gesture> getGestures() 
        {
            return gestures.Values.ToList();
        }

        public bool ContainsGesture(int gestureID)
        {
            return gestures.ContainsKey(gestureID);
        }
    }
}