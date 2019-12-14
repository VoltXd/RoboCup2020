﻿using AdvancedTimers;
using EventArgsLibrary;
using HeatMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Utilities;
using WorldMap;

namespace WayPointGenerator
{
    public class WaypointGenerator
    {
        string robotName;

        //Timer timerWayPointGeneration;

        Location destinationLocation;
        GlobalWorldMap globalWorldMap;
        //double[,] strategyManagerHeatMap = new double[0, 0];

        //double heatMapCellsize = 2; //doit être la même que celle du strategy manager
        //double fieldLength = 22;
        //double fieldHeight = 14;

        Heatmap StrategyHeatmap; 

        public WaypointGenerator(string name)
        {
            robotName = name;
            //timerWayPointGeneration = new Timer(100);
            //timerWayPointGeneration.Elapsed += TimerWayPointGeneration_Elapsed;
            //timerWayPointGeneration.Start();
        }

        public void SetNextWayPoint(Location waypointLocation)
        {
            OnWaypoint(robotName, waypointLocation);
        }

        public void OnDestinationReceived(object sender, EventArgsLibrary.LocationArgs e)
        {
            if (e.RobotName == robotName)
            {
                destinationLocation = e.Location;
            }
        }

        public void OnStrategyHeatMapReceived(object sender, EventArgsLibrary.HeatMapArgs e)
        {            
            if (robotName == e.RobotName)
            {
                StrategyHeatmap = e.HeatMap;
                CalculateOptimalWayPoint();
            }
        }
        public void OnGlobalWorldMapReceived(object sender, GlobalWorldMapArgs e)
        {
            globalWorldMap = e.GlobalWorldMap;
        }

        private void CalculateOptimalWayPoint()
        {
            Heatmap WaypointGeneratorHeatmap = StrategyHeatmap.Copy();

            //Attention, le remplissage de la HeatMap se fait avec une inversion des coordonnées
            //for(int y = 0; y < nbCellInHeatMapHeight; y++)
            //{
            //    for (int x = 0; x < nbCellInHeatMapWidth; x++)
            //    {
            //        //Prise en compte de la destination
            //        if (destinationLocation != null)
            //        {
            //            data[y, x] = strategyManagerHeatMap[y,x];
            //        }
            //    }
            //}

            //if (globalWorldMap != null)
            //{
            //    if (globalWorldMap.robotLocationDictionary.ContainsKey(robotName))
            //    {
            //        Location robotLocation = globalWorldMap.robotLocationDictionary[robotName];
            //        double angleDestination = Math.Atan2(destinationLocation.Y - robotLocation.Y, destinationLocation.X - robotLocation.X);

            //        //On veut éviter de taper les autres robots
            //        for(int i=0; i<globalWorldMap.robotLocationDictionary.Count;i++)
            //        {
            //            string competitorName = globalWorldMap.robotLocationDictionary.Keys.ElementAt(i);
            //            Location competitorLocation = globalWorldMap.robotLocationDictionary.Values.ElementAt(i);
            //            //On itère sur tous les robots sauf celui-ci
            //            if (competitorName != robotName)
            //            {
            //                double angleRobotAdverse = Math.Atan2(competitorLocation.Y - robotLocation.Y, competitorLocation.X - robotLocation.X);
            //                double distanceRobotAdverse = Toolbox.Distance(competitorLocation.X, competitorLocation.Y, robotLocation.X, robotLocation.Y);

            //                for (int y = 0; y < nbCellInHeatMapHeight; y++)
            //                    for (int x = 0; x < nbCellInHeatMapWidth; x++)
            //                    {
            //                        PointD ptCourant = GetFieldPosFromHeatMapCoordinates(x, y);
            //                        double distancePt = Toolbox.Distance(ptCourant.X, ptCourant.Y, robotLocation.X, robotLocation.Y);
            //                        double anglePtCourant = Math.Atan2(ptCourant.Y - robotLocation.Y, ptCourant.X - robotLocation.X);

            //                        if (Math.Abs(distanceRobotAdverse * (anglePtCourant - angleRobotAdverse)) < 2.0 && distancePt > distanceRobotAdverse - 3)
            //                            data[y, x] -= 1;// Math.Max(0, 1 - Math.Abs(anglePtCourant - angleRobotAdverse) *10.0);
            //                    }
            //            }
            //        }
            //    }
            //}
            //for (int y = 0; y < nbCellInHeatMapHeight; y++)
            //{
            //    for (int x = 0; x < nbCellInHeatMapWidth; x++)
            //    {
            //        if (data[y, x] > max)
            //        {
            //            max = data[y, x];
            //            maxPosX = x;
            //            maxPosY = y;
            //        }
            //    }
            //}
            //PointD OptimalPosition = GetFieldPosFromHeatMapCoordinates(maxPosX, maxPosY);

            ; 
            PointD OptimalPosition = WaypointGeneratorHeatmap.GetMaxPositionInBaseHeatMap();

            OnHeatMap(robotName, WaypointGeneratorHeatmap);
            SetNextWayPoint(new Location((float)OptimalPosition.X, (float)OptimalPosition.Y, 0, 0, 0, 0));
        }


        //private PointD GetFieldPosFromHeatMapCoordinates(int x, int y)
        //{
        //    return new PointD(-fieldLength / 2 + x * heatMapCellsize, -fieldHeight / 2 + y * heatMapCellsize);
        //}

        public delegate void NewWayPointEventHandler(object sender, LocationArgs e);
        public event EventHandler<LocationArgs> OnWaypointEvent;
        public virtual void OnWaypoint(string name, Location wayPointlocation)
        {
            var handler = OnWaypointEvent;
            if (handler != null)
            {
                handler(this, new LocationArgs { RobotName = name, Location = wayPointlocation});
            }
        }


        public delegate void HeatMapEventHandler(object sender, HeatMapArgs e);
        public event EventHandler<HeatMapArgs> OnHeatMapEvent;
        public virtual void OnHeatMap(string name, Heatmap heatMap)
        {
            var handler = OnHeatMapEvent;
            if (handler != null)
            {
                handler(this, new HeatMapArgs { RobotName = name, HeatMap = heatMap });
            }
        }
    }
}
