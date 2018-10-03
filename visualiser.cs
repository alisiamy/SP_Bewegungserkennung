using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

namespace SP_Bewegungserkennung
{
    class visualiser
    {
        Process visualize;

        List<point> plist;
        List<State> slist;
        List<Cluster> clist;
        public enum ClusterOrState
        {
            VISCLUSTER,
            VISSTATE
        }

        public ClusterOrState flag;


        public visualiser(List<Cluster> cl) {
            clist = cl;
            visualize = new Process();
            flag = ClusterOrState.VISCLUSTER;
        }

        public visualiser(List<State> sl, List<point> pl) {
            plist = pl;
            slist = sl;
            visualize = new Process();
            flag = ClusterOrState.VISSTATE;
        }

        public void runVisualiser() {

  
            string fileName = Path.GetTempFileName();
            FileInfo fileInfo = new FileInfo(fileName);
            fileInfo.Attributes = FileAttributes.Temporary;

            using (FileStream fs = fileInfo.OpenWrite())
            {
                if (flag == ClusterOrState.VISCLUSTER)
                {
                    foreach (Cluster c in clist) {
                        foreach(point p in c.points){
                            byte[] xy = Encoding.ASCII.GetBytes(p.ToString() + ";");
                            fs.Write(xy, 0, xy.Length);
                        }
                        byte [] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                        fs.Write(newline, 0, newline.Length);
                    }
                    foreach (Cluster c in clist) {
                        byte[] mean = Encoding.ASCII.GetBytes(c.mean.ToString() + ";");
                        fs.Write(mean, 0, mean.Length);
                    }
                    byte[] nl = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(nl, 0, nl.Length);

                    visualize.StartInfo.FileName = "python.exe";
                    visualize.StartInfo.RedirectStandardOutput = true;
                    visualize.StartInfo.UseShellExecute = false;

                    visualize.StartInfo.Arguments = "plot_clusters.py " + fileName;

                    visualize.Start();
                }
                else
                {
                    foreach (State s in slist) {
                        byte[] cntr = Encoding.ASCII.GetBytes(s.center.ToString() + ";");
                        fs.Write(cntr, 0, cntr.Length);
                    }
                    byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(newline, 0, newline.Length);

                    foreach (State s in slist){
                        byte[] tsh = Encoding.ASCII.GetBytes(s.treshold.ToString() + ";");
                        fs.Write(tsh, 0, tsh.Length);
                    }
                    fs.Write(newline, 0, newline.Length);

                    List<List<point>> printList = new List<List<point>>();

                    for (int j = 0; j < slist.Count; ++j) {
                        printList.Add(new List<point>());
                    }

                    foreach (point p in plist) {
                        for (int i = 0; i < printList.Count; ++i) {
                            if (slist[i].pointInState(p)) {
                                printList[i].Add(p);
                            }
                        }
                    }

                    foreach (List <point> lp in printList) {

                        foreach (point pt in lp) {
                            byte[] xy = Encoding.ASCII.GetBytes(pt.ToString() + ";");
                            fs.Write(xy, 0, xy.Length);
                        }
                        fs.Write(newline, 0, newline.Length);
                    }

                    visualize.StartInfo.FileName = "python.exe";
                    visualize.StartInfo.RedirectStandardOutput = true;
                    visualize.StartInfo.UseShellExecute = false;

                    visualize.StartInfo.Arguments = "plot_states.py " + fileName;

                    visualize.Start();
                }
            }


        }
    }
}
