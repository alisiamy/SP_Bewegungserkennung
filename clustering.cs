using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace SP_Bewegungserkennung
{
    /**
    *   Class KmeansClustering, which preforms a k-means Clustering on a shape
    */
    class KmeansClustering
    {
        public int k { get; private set; }
        public List<Cluster> ClusterList { get; private set; }
        private List<point> PointList;
        private point sigmaNull;
        private double epsilon;

       /**
       * Constructor of a Clustering with a set gestures saved in Shape and the desired variance sNull 
       */
        public KmeansClustering(Shape s, point sNull, int k = 2, double epsilon = Double.Epsilon)
        {
            this.k = k;
            this.sigmaNull = sNull;
            this.epsilon = epsilon;

            PointList = new List<point>();

            foreach (Gesture g in s.getGestures())
                PointList.AddRange(g.Points);

            ClusterList = new List<Cluster>();
            for (int i = 0; i < k; ++i)
            {
                ClusterList.Add(new Cluster(PointList.GetRange(i * PointList.Count / k, Math.Min(PointList.Count / k, PointList.Count - i * PointList.Count / k))));
            }
        }
        private point maxVariance()
        {
            point maxVariance = new point(0, 0);
            foreach (Cluster c in ClusterList)
            {
                if (c.variance.CompareTo(maxVariance) > 0)
                    maxVariance = c.variance;
            }
            return maxVariance;
        }
        /**
       *  Clustering preforms a dynamic k-means clustering in which value sNull is the termination close.
       *  For determination which point lands in which cluster euclidean distance is used
       *  New cluster is added when the mean of the clusters is not changing
       */
        public void clustering()
        {

            point curVariance = maxVariance();

            do
            {
                bool change;
                do
                {
                    foreach (Cluster c in ClusterList)
                    {
                        c.clearCluster();
                    }

                    change = false;

                    foreach (point p in PointList)
                    {
                        double minDist = ClusterList[0].euclideanDist(p);
                        int DistX = 0;

                        for (int j = 1; j < ClusterList.Count; ++j)
                        {
                            double tmpDist = ClusterList[j].euclideanDist(p);

                            if (tmpDist < minDist)
                            {
                                minDist = tmpDist;
                                DistX = j;
                            }
                        }
                        ClusterList[DistX].addToCluster(p);
                    }

                    foreach (Cluster c in ClusterList)
                    {
                        point oldMean = new point(c.mean);
                        c.updateCluster();
                        change |= !(c.mean.distance(oldMean) < epsilon);
                    }
                } while (change);

                curVariance = maxVariance();

                if (sigmaNull.CompareTo(curVariance) < 0)
                {
                    k++;
                    int i = 0;
                    while (!ClusterList[i].variance.Equals(curVariance))
                        ++i;

                    ClusterList.Add(ClusterList[i].split());
                }

            } while (sigmaNull.CompareTo(curVariance) < 0);
        }
    }
}