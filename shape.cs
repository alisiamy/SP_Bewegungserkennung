using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace SP_Bewegungserkennung
{

    public class Shape
    {
        private Dictionary<int, Gesture> gestures;
        public int shapeID { get; private set; }

        public Shape(int shapeID, Gesture g)
        {
            this.shapeID = shapeID;
            this.gestures = new Dictionary<int, Gesture>();
            this.gestures.Add(g.gestureID, g);
        }

        public void Add(int gestureID, Gesture newGesture)
        {
            gestures.Add(gestureID, newGesture);
        }

        public Gesture getGesture(int gestureID)
        {
            Gesture g;
            gestures.TryGetValue(gestureID, out g);
            return g;
        }

        public List<int> getIDList()
        {

            return gestures.Keys.ToList();
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