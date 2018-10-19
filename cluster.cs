using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace SP_Bewegungserkennung
{
    /**
    *   Class Cluster, which describes a as a result of clustering created cluster
    */
    class Cluster
    {
        /**
         * \param points is a List of points in the cluster
         * \param variance defines variance of a cluster
         * \param mean is a mean value of the cluster
         * \param covariance is a covariace of a cluster
         * \param icvm is a inverse of a covariance of a cluster
         */
        public List<point> points { get; private set; }
        public point variance { get; private set; }
        public point mean { get; private set; }
        private double covariance;
        private double[] icvm;
        /**
         * Constructor which creaates new cluster with a given mean
         */

        public Cluster(List<point> points)
        {
            this.points = points;
            updateCluster();
        }
        /**
         * clearCluster function clears points of an exisiting cluster
         */
        public void clearCluster()
        {
            points = new List<point>();
        }
        /**
         * calculateMean function calculates a mean of a cluster
         */
        private void calculateMean()
        {
            mean = new point(0, 0);
            foreach (point p in points)
                mean.addition(p);

            mean.divide(points.Count);
        }
        /**
         * calculateVariance function clears points of an exisiting cluster
         */
        private void calculateVariance()
        {
            variance = new point(0, 0);
            foreach (point p in points)
            {
                variance.addition(point.substract(p, mean).power(2));
            }
            variance.divide(points.Count);
        }
        /**
        * calculateCovariance function calculates covariance of the cluster
        */
        private void calculateCovariance()
        {
            covariance = 0;

            foreach (point p in points)
            {
                point tmp = point.substract(p, mean);
                covariance += tmp.x * tmp.y / points.Count;
            }
        }
        /**
        * CVmatrixinverse function calculates an inversed covariance matrix
        */
        private void CVmatrixinverse()
        {
            this.icvm = new double[4];

            double a = variance.x;
            double bc = covariance;
            double d = variance.y;
            Debug.Assert(!Double.IsNaN(a) && !Double.IsNaN(bc) && !Double.IsNaN(d));

            double divider = a * d - Math.Pow(bc, 2);

            Debug.Assert(divider != 0);

            this.icvm[0] = d / divider;
            this.icvm[1] = -bc / divider;
            this.icvm[2] = this.icvm[1];
            this.icvm[3] = a / divider;
        }
        /**
        * updateCluster function updates attributes of a cluster
        */
        public void updateCluster()
        {
            this.calculateMean();
            this.calculateVariance();            
        }
        /**
        * addToCluster function adds a point to the cluster
        */
        public void addToCluster(point ipt)
        {
            points.Add(ipt);
        }
        /**
        * split function splits clusters for clustering
        */
        public Cluster split()
        {
            Cluster other = new Cluster(points.GetRange(0, points.Count / 2));
            points.RemoveRange(0, points.Count / 2);
            updateCluster();
            return other;
        }
        /**
        * euclideanDist function calculates a Euclidean distance between apoint and a cluster
        */
        public double euclideanDist(point p)
        {
            return mean.distance(p);
        }
        /**
        * Mahalanobis distance between a point and a cluster
        */
        public double mahalanobisDist(point p)
        {
            point tmp = point.substract(p, mean);

            point p1im = new point(tmp.x * icvm[0] + tmp.y * icvm[2], tmp.x * icvm[1] + tmp.y * icvm[3]);

            Debug.Assert(tmp.x * p1im.x + tmp.y * p1im.y >= 0);
            return Math.Sqrt(tmp.x * p1im.x + tmp.y * p1im.y);
        }

    }
}