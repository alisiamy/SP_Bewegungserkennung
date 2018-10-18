using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SP_Bewegungserkennung {
    [DataContract]
    public class point : IComparable<point>, IEquatable<point> {
        [DataMember]
        public double x { get; private set; }
        [DataMember]
        public double y { get; private set; }
        [DataMember]
        public long time { get; private set; }

        // Main constructor
        public point(double px, double py) {
            x = px;
            y = py;
            time = 0;
        }
        // --
        public point(point p) {
            x = p.x;
            y = p.y;
            time = p.time;
        }
        // ---
        public point(double px, double py, long t) {
            x = px;
            y = py;
            time = t;

        }
        // Add a point on to existing one
        public void addition(point ipt) {
            this.x += ipt.x;
            this.y += ipt.y;
        }
        //Substract a point from an extisting one 
        public void subtraction(point ipt) {
            this.x -= ipt.x;
            this.y -= ipt.y;
        }
        //Subsract one point from another
        public static point substract(point p1, point p2) {
            return new point(p1.x - p2.x, p1.y - p2.y);
        }

        // Divide a point with a variable
        public void divide(int c) {
            this.x /= c;
            this.y /= c;
        }
        // Multiply a point with a variable
        public point mult(double m) {
            return new point(this.x * m, this.y * m);
        }
        //Multiply a point with a variable
        public void multiply(point p) {
            this.x *= p.x;
            this.y *= p.y;
        }
        //Power functions with point as a base and a n as an exponent
        public point power(double n) {
            return new point(Math.Pow(this.x, n), Math.Pow(this.y, n));
        }
        //Square root of a point
        public point sqroot() {
            return new point(Math.Sqrt(this.x), Math.Sqrt(this.y));
        }
        //Modulus of a point
        public static point abs(point p) {
            return new point(Math.Abs(p.x), Math.Abs(p.y));
        }
        //Compare a point to an existing one 
        public bool compareXY(point p) {
            return this.x <= p.x && this.y <= p.y;

        }
        public int CompareTo(point comparepoint) {
            if (comparepoint == null)
                return 1;

            if (this.x == comparepoint.x)
                return this.y.CompareTo(comparepoint.y);

            return this.x.CompareTo(comparepoint.x);
        }

        public double distance(point p) {
            return Math.Sqrt(Math.Pow(this.x - p.x, 2) + Math.Pow(this.y - p.y, 2));
        }

        public bool Equals(point eqpoint) {
            if (this.x == eqpoint.x && this.y == eqpoint.y)
                return true;

            return false;
        }

        public override String ToString() {
            return "(" + this.x.ToString() + "," + this.y.ToString() + ")";
        }
    }

    public class pointComparer : IComparer<point> {
        public int Compare(point p1, point p2) {
            if (p1.time > p2.time)
                return 1;
            else if (p1.time < p2.time)
                return -1;
            else
                return 0;
        }
    }
}