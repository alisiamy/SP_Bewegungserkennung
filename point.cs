using System;
using System.Collections.Generic;

namespace tryCluster{
public class point: IComparable<point>, IEquatable<point>{
        public double x, y; // sollte private sein
        public point(double px, double py){
            x = px;
            y = py;
        }
        public void addition(point ipt){
            this.x += ipt.x;
            this.y += ipt.y;
        }
        public void subtraction(point ipt){ // ???
            this.x -= ipt.x;
            this.y -= ipt.y;
        }
        public static point substract(point p1, point p2){
            return new point(p1.x - p2.x, p1.y - p2.y);
        }
        public void divide(int c){
            this.x /= c;
            this.y /= c;
        }
        public point mult(int m){
            return new point(this.x *m,this.y *m);
        }
		public point power(double n){
			return new point(Math.Pow(this.x, n), Math.Pow(this.y, n));
		}

        public bool pround(point old){
                if(this.Equals(old)){
                    return true;
                }
                if (Double.IsInfinity(this.x) || Double.IsNaN(old.x) || Double.IsInfinity(this.y) || Double.IsNaN(old.y)){
                    return false;
                }
                if (Double.IsInfinity(old.x) || Double.IsNaN(this.x) || Double.IsInfinity(old.y) || Double.IsNaN(this.y)){
                    return false;
                }
                 return Math.Abs(this.x - old.x) <= 0.000000001 && Math.Abs(this.y - old.y) <= 0.000000001;

            }
        public int CompareTo(point comparepoint)
        {
            if (comparepoint == null){
                return 1;
            }
            else{
                    if(this.x == comparepoint.x){
                        return this.y.CompareTo(comparepoint.y);
                    }else{
                        return this.x.CompareTo(comparepoint.x);                    }
            }
        }
        public bool Equals(point eqpoint){
            if(this.x == eqpoint.x && this.y == eqpoint.y){
            return true;
            }else{
            return false;
            }
        }
        public override String ToString(){
            return "("+this.x.ToString()+","+this.y.ToString()+")";
        }
    }
}