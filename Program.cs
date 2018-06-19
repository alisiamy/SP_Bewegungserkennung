using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using gD = gestureData;

namespace Softwareprojekt
{
    class Program
    {
        

        static void noMain(string[] args) {
            String s =" 1 ";
            int a = Int32.Parse(s);
            Console.WriteLine(a);
        }
        static void Main(string[] args)
        {
            //tabKlein.csv oder KinectDaten_Pascal.csv
            //String path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/tabKlein.csv";
            String path;
            if(false) path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/tabKlein.csv"; else 
                path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/KinectDaten_Pascal.csv";
            trainDataReader TDReader = new trainDataReader(path);
            gestureData.trainingData trainData = TDReader.readData();

            //welche shapes sind da?
            for(int shapeNum = 0; shapeNum > 30; shapeNum++) {
                if(trainData.getShape(shapeNum) == null) Console.WriteLine(shapeNum + " gibts nicht");
                else Console.WriteLine(shapeNum + " gibts");
            }

            //Clustering
            tryCluster.KMclustering[] clusterArray = TDClustering(trainData);
            


            
        }

        private static tryCluster.KMclustering[] TDClustering(gestureData.trainingData td) {
            tryCluster.KMclustering[] clusterArray = new tryCluster.KMclustering[23]; //Array that will be returned
            List<tryCluster.point> shapePoints = new List<tryCluster.point>(); //All points of one shape

            for(int shapeNum = 1; shapeNum < 23; shapeNum++) { //0 nicht da kaputt
                List<gD.trainingData.Shape.Gesture> ges = td.getShape(shapeNum).getGestures();
            	foreach(gD.trainingData.Shape.Gesture singleGes in ges) {
                    List<gD.trainingData.Shape.Gesture.Point> poi = singleGes.getPoints();
                    foreach(gD.trainingData.Shape.Gesture.Point singlePoint in poi) {
                        shapePoints.Add(singlePoint.toClusterPoint());
                    }
                }

                //alle Gesten eines Shapes durchlaufen
                //-> alle deren Punkte Clustern
                tryCluster.point startPoint = new tryCluster.point(0,0); //Wozu? Besserer Punkt? Mitte der Punktwolke?
                tryCluster.KMclustering kmt = new tryCluster.KMclustering(shapePoints, 1, startPoint);
                kmt.clustering();
                clusterArray[shapeNum] = kmt;

                shapePoints = new List<tryCluster.point>();
            }
            return clusterArray;


            /*List <point> test = new List<point>(){
                
            };
            //Console.WriteLine("Hello World!");
            foreach(Tuple<double,double> t in read){
                test.Add(new point(t.Item1,t.Item2));
            }
            tryCluster.KMclustering kmt = new tryCluster.KMclustering(test, 1, new point(10,10));
            kmt.clustering();*/
        }
    }

        class trainDataReader {

        private String _path;

        public trainDataReader(String path) => _path = path;

        public gestureData.trainingData readData() {
            using(var reader = new StreamReader(@_path))
             {
                 //ab hier nochmal ändern -> für jede spalte eine Liste
                 //FrameID;X;Y;Z;GestureID;SqlTime;Alpha;Beta;Velocity;ShapeId;UID
                 // wichtig:ShapeID -> GestureID -> (FrameID, X, Y)
                 //Gestures sind alle zusammen, Punkte nie verteilt
                gestureData.trainingData td = new gestureData.trainingData();
                String head = reader.ReadLine();

                List<gD.trainingData.Shape> currentShape = new List<gD.trainingData.Shape>();
                List<gD.trainingData.Shape.Gesture> currentGesture = new List<gD.trainingData.Shape.Gesture>();

                int currentPointID;
                float currentPointX;
                float currentPointY;
                float currentPointZ;
                int currentGestureID;
                int currentShapeID;

                Boolean[] shapesCreated = new Boolean[23]; //merken welche shapes es schon gibt, eig unnötig
                //Boolean[] isShape3D = new Boolean[22+1]; //alle mit false initialisiert
                //isShape3D[14] = true; 
                //isShape3D[15] = true; //die Spiralen
                //Problem: in trainingData.cs variable is2D, hier aber is3D übergeben

                int prevShapeID = int.MinValue;
                int prevGestureID = int.MinValue;
                gD.trainingData.Shape.Gesture prevGesture = new gD.trainingData.Shape.Gesture(prevGestureID);

                do {
                    String line = reader.ReadLine();
                    string[] lineArray = line.Split(';');

                    currentPointID = Int32.Parse(lineArray[0]);
                    currentPointX = float.Parse(lineArray[1]);
                    currentPointY = float.Parse(lineArray[2]);
                    currentPointZ = float.Parse(lineArray[3]);
                    currentGestureID = Int32.Parse(lineArray[4]);
                    currentShapeID = Int32.Parse(lineArray[9]);

                    //falls neue Geste != der bisherigen und nicht der erste Durchlauf
                    if(prevGestureID != currentGestureID && prevGestureID!=int.MinValue) {
                        //prevGesture in zugehöriges shape einfügen
                        //neue Gesture anlegen
                        //prevGesture durch neue Gesture ersetzen

                        //falls noch kein neues Shape besteht -> anlegen
                        if(!shapesCreated[prevShapeID]) {//if(!shapesCreated[currentShapeID]) {
                            //Console.WriteLine(currentShapeID + " Shape hinzugefügt");
                            //td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, isShape3D[currentShapeID])); //für 3D, unfertig
                            td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, true));
                            shapesCreated[prevShapeID] = true;
                        }

                        td.getShape(prevShapeID).addGesture(prevGesture);

                        //prevGesture durch neue, leere gesture ersetzen
                        prevGesture = new gD.trainingData.Shape.Gesture(currentGestureID);
                        prevGestureID = currentGestureID;
                    } else if(prevGestureID==int.MinValue) {    //nur beim ersten durchlauf
                        //td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, isShape3D[currentShapeID])); //für 3D, unfertig
                        td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, true));
                        prevGesture = new gD.trainingData.Shape.Gesture(currentGestureID);
                        prevGestureID = currentGestureID;
                        //Console.WriteLine("Erste Geste erstellt " + currentShapeID);
                        shapesCreated[currentShapeID] = true;
                        prevShapeID = currentShapeID;
                    }

                    //Add Point to current Gesture
                    /*if(isShape3D[currentShapeID]) {       //Shape is 3D
                        prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY, currentPointZ));
                    } else {                                //Shape is 2D
                        prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY));
                    }*/
                    //nur 2D
                    prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY));
                    

                    //update prevVariable
                    prevShapeID = currentShapeID;
                    

            
                }while (!reader.EndOfStream);

                //add last Gesture
                td.getShape(prevShapeID).addGesture(prevGesture);

                return td;
             }
        }
    }

}