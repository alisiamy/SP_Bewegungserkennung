using System;
using System.Collections.Generic;

namespace tryCluster{
public class point: IComparable<point>, IEquatable<point>{
        private double x, y;
        public point(double px, double py){
            x = px;
            y = py;
        }
        public void addition(point ipt){
            this.x += ipt.x;
            this.y += ipt.y;
        }
        public void divide(int c){
            this.x /= c;
            this.y /= c;
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