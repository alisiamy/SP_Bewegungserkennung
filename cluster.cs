using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace SP_Bewegungserkennung
{

    class Cluster
    {
        public List<point> points { get; private set; }
        public point variance { get; private set; }
        public point mean { get; private set; }
        private double covariance;
        private double[] ikvm;

        public Cluster(point var, point me)
        {
            variance = var;
            mean = me;
        }

        public Cluster(point ipt)
        {
            mean = ipt;
            points = new List<point>();
            addToCluster(ipt);
        }

        public Cluster(List<point> points)
        {
            this.points = points;
            updateCluster();
        }

        public void clearCluster()
        {
            points = new List<point>();
        }

        private void calculateEV()
        {
            mean = new point(0, 0); // TODO: moegliche Optimierung
            foreach (point p in points)
                mean.addition(p);

            mean.divide(points.Count);
        }

        private void calculateVariance()
        {
            variance = new point(0, 0);
            foreach (point p in points)
                variance.addition(point.substract(p, mean).power(2));
            Debug.Assert(!Double.IsInfinity(variance.x) && !Double.IsInfinity(variance.y) && !Double.IsNaN(variance.x) && !Double.IsNaN(variance.y));
            variance.divide(points.Count);
        }

        private void calculateCV()
        {
            covariance = 0;

            Debug.Assert(points.Count != 0);
            foreach (point p in points)
            {
                point tmp = point.substract(p, mean);
                covariance += tmp.x * tmp.y / points.Count;
            }
        }

        private void KVmatrixinverse()
        {
            this.ikvm = new double[4];

            double a = variance.x;
            double bc = covariance;
            double d = variance.y;
            Debug.Assert(!Double.IsNaN(a) && !Double.IsNaN(bc) && !Double.IsNaN(d));

            double divider = a * d - Math.Pow(bc, 2);

            Debug.Assert(divider != 0);

            this.ikvm[0] = d / divider;
            this.ikvm[1] = -bc / divider;
            this.ikvm[2] = this.ikvm[1];
            this.ikvm[3] = a / divider;
        }

        public void updateCluster()
        {
            this.calculateEV();
            this.calculateVariance();
            //this.calculateCV();
            //this.KVmatrixinverse();

        }

        public void addToCluster(point ipt)
        {
            points.Add(ipt);
        }

        public Cluster split()
        {
            Cluster other = new Cluster(points.GetRange(0, points.Count / 2));
            points.RemoveRange(0, points.Count / 2);
            updateCluster();
            return other;
        }

        public double euclideanDist(point p)
        {
            return mean.distance(p);
        }

        //Mahalanobis distance between a point and a cluster
        public double mahalanobisDist(point p)
        {
            point tmp = point.substract(p, mean);

            point p1im = new point(tmp.x * ikvm[0] + tmp.y * ikvm[2], tmp.x * ikvm[1] + tmp.y * ikvm[3]); // transpone

            Debug.Assert(tmp.x * p1im.x + tmp.y * p1im.y >= 0);
            return Math.Sqrt(tmp.x * p1im.x + tmp.y * p1im.y);
        }

    }
}