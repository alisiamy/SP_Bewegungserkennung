using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SP_Bewegungserkennung
{

    /**
     * Class point, which describes an 2-dimentional vector, and it's operations
     */
    [DataContract]
    public class point : IComparable<point>, IEquatable<point>
    {
        [DataMember]
        /**
         * \param x is a x-coordinate of a 2-dimensional vector
         * \param y is a y-coordinate of a 2-dimensional vector
         * \param time is a time stamp of a point
         */
        public double x { get; private set; }
        [DataMember]
        public double y { get; private set; } 
        [DataMember]
        public long time { get; private set; }

        /**
         * Constructor, which defines a point with x and y coordinates and a default time variable
         */
        public point(double px, double py)
        {
            x = px;
            y = py;
            time = 0;
        }
        /**
         * Constructor, which defines new point from an exsiting point
         */
        public point(point p)
        {
            x = p.x;
            y = p.y;
            time = p.time;
        }
        /**
         * Constructor, which defines a point with x and y coordinates and a time variable
         */
        public point(double px, double py, long t)
        {
            x = px;
            y = py;
            time = t;

        }
        /**
         * Addition of a point on to existing one
         */
        public void addition(point ipt)
        {
            this.x += ipt.x;
            this.y += ipt.y;
        }
        /**
         * Subsraction of one point from another
         */
        public static point substract(point p1, point p2)
        {
            return new point(p1.x - p2.x, p1.y - p2.y);
        }
        /**
         * Division of a point with a variable 
         */
        public void divide(int c)
        {
            this.x /= c;
            this.y /= c;
        }
        /**
         * Mutiplicatoin of a point with a variable 
         */
        public point mult(double m)
        {
            return new point(this.x * m, this.y * m);
        }
        /**
         * Mutiplicatoin of an existing point with a given point 
         */
        public void multiply(point p) {
            this.x *= p.x;
            this.y *= p.y;
        }
        /**
         * Exponentianlion function of a point as a base and a variable n as an exponent
         */
        public point power(double n)
        {
            return new point(Math.Pow(this.x, n), Math.Pow(this.y, n));
        }
        /**
         * Square root function of a point
         */
        public point sqroot()
        {
            return new point(Math.Sqrt(this.x), Math.Sqrt(this.y));
        }
        /**
        * An absolte value function of a given point
        */
        public static point abs(point p)
        {
            return new point(Math.Abs(p.x), Math.Abs(p.y));
        }
        /**
        * Comparison of two points, according to x and y
        */
        public bool compareXY(point p)
        {
            return this.x <= p.x && this.y <= p.y;

        }
        /**
        * Comparison of two points, according to x
        */
        public int CompareTo(point comparepoint)
        {
            if (comparepoint == null)
                return 1;

            if (this.x == comparepoint.x)
                return this.y.CompareTo(comparepoint.y);

            return this.x.CompareTo(comparepoint.x);
        }
        /**
        *Euclidean distance function between an existing point and a given one
        */
        public double distance(point p)
        {
            return Math.Sqrt(Math.Pow(this.x - p.x, 2) + Math.Pow(this.y - p.y, 2));
        }
        /**
        * Equality function  between an existing point and a given one
        */
        public bool Equals(point eqpoint)
        {
            if (this.x == eqpoint.x && this.y == eqpoint.y)
                return true;

            return false;
        }
        /**
        *  ToString function, which returns a string of x and y coordinates of a point in format (x,y)
        */
        public override String ToString()
        {
            return "(" + this.x.ToString() + "," + this.y.ToString() + ")";
        }
    }
    /**
     *Comparer class, which compares points with one another   
    */
    public class pointComparer : IComparer<point>
    {
        public int Compare(point p1, point p2)
        {
            if (p1.time > p2.time)
                return 1;
            else if (p1.time < p2.time)
                return -1;
            else
                return 0;
        }
    }
}