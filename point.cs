using System;
using System.Collections.Generic;


namespace Bewegungserkennung 
{

    public class point: IComparable<point>, IEquatable<point>
    {
        public double x {get; private set;}
        public double y {get; private set;} // sollte private sein
        public long time {get; private set;}

        public point(double px, double py)
        {
            x = px;
            y = py;
            time = 0;
        }

        public point(point p)
        {
            x = p.x;
            y = p.y;
            time = p.time;
        }

        public point(double px, double py, long t){
            x = px;
            y = py;
            time = t;

        }

        public void addition(point ipt)
        {
            this.x += ipt.x;
            this.y += ipt.y;
        }

        public void subtraction(point ipt)
        {
            this.x -= ipt.x;
            this.y -= ipt.y;
        }

        public static point substract(point p1, point p2)
        {
            return new point(p1.x - p2.x, p1.y - p2.y);
        }

        public void divide(int c)
        {
            this.x /= c;
            this.y /= c;
        }

        public point mult(double m)
        {
            return new point(this.x *m,this.y *m);
        }

		public point power(double n){
			return new point(Math.Pow(this.x, n), Math.Pow(this.y, n));
		}

        public point sqroot(){
            return new point(Math.Sqrt(this.x),Math.Sqrt(this.y));
        }

        public bool pround(point old)
        {
                if(this.Equals(old))
                    return true;

                if (Double.IsInfinity(this.x) || Double.IsNaN(old.x) || Double.IsInfinity(this.y) || Double.IsNaN(old.y))
                    return false;

                if (Double.IsInfinity(old.x) || Double.IsNaN(this.x) || Double.IsInfinity(old.y) || Double.IsNaN(this.y))
                    return false;

                return Math.Abs(this.x - old.x) <= 0.000000001 && Math.Abs(this.y - old.y) <= 0.000000001;

        }

        public void abs()
        {
            this.x = Math.Abs(this.x);
            this.y = Math.Abs(this.y);
        }
        
        public static point abs(point p)
        {
            return new point(Math.Abs(p.x),Math.Abs(p.y));
        }

        public int CompareTo(point comparepoint)
        {
            if (comparepoint == null)
                return 1;

            if(this.x == comparepoint.x)
                return this.y.CompareTo(comparepoint.y);

            return this.x.CompareTo(comparepoint.x); 
        }

        public double distance(point p)
        {
            return Math.Sqrt(Math.Pow(this.x-p.x,2)+Math.Pow(this.y-p.y,2));
        }

        public bool Equals(point eqpoint)
        {
            if(this.x == eqpoint.x && this.y == eqpoint.y)
                return true;

            return false;
        }

        public override String ToString()
        {
            return "("+this.x.ToString()+","+this.y.ToString()+")";
        }
    }

    public class pointComparer : IComparer<point>{
        public int Compare(point p1, point p2){
            if(p1.time > p2.time)
                return 1;
            else if(p1.time < p2.time)
                return -1;
            else
                return 0;
        }
    }
}