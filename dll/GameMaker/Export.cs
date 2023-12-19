using SharpPath.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPath.GameMaker
{
    public static class Export
    {
        static readonly object dsMapLock = new object();
        static readonly object activePathsLock = new object();
        static readonly object activeGridsLock = new object();

        public static GMExtension GMExtension;
        public static List<Grid> Grids = new List<Grid>();
        public static List<int> ActivePaths = new List<int>();
        public static List<int> ActiveGrids = new List<int>();
        
        [DllExport("RegisterCallbacks", CallingConvention.Cdecl)]
        public unsafe static double RegisterCallbacks(char* arg0, char* arg1, char* arg2, char* arg3)
        {
            GMExtension = new GMExtension(arg0, arg1, arg2, arg3);

            return 0;
        }

        [DllExport("SpGridCreate", CallingConvention.Cdecl)]
        public static double SpGridCreate(double width, double height)
        {
            Grids.Add(new Grid((int)width, (int)height));

            return Grids.Count-1;
        }

        [DllExport("SpGridCreateFromBuffer", CallingConvention.Cdecl)]
        public static unsafe double SpGridCreateFromBuffer(double width, char* bufferBase64)
        {
            byte[] buffer;

            buffer = Convert.FromBase64String(GMExtension.StringFromCharPointer(bufferBase64));

            Grids.Add(new Grid(buffer, (int)width));

            return Grids.Count-1;
        }

        [DllExport("SpGridFindPath", CallingConvention.Cdecl)]
        public static double SpGridFindPath(double gridId, double pathId, double xStart, double yStart, double xDestination, double yDestination, double searchDirections, double includeStart)
        {
            lock (activePathsLock)
            {
                if (ActivePaths.Contains((int)pathId))
                {
                    return 0;
                }

                ActivePaths.Add((int)pathId);
            }

            lock (activeGridsLock) ActiveGrids.Add((int)gridId);

            Task.Run(() =>
            {
                Path path;
                string pathBase64;
                Grid grid;
                Dictionary<string, string> dsMapItems;

                grid = GetGrid(gridId);
                dsMapItems = new Dictionary<string, string>();

                try
                {
                    IPathFinder pathFinder;

                    switch ((SearchDirections)(int)searchDirections)
                    {
                        case SearchDirections.Four:
                            pathFinder = new PathFinder4(grid, (int)includeStart != 0);
                            break;

                        default:
                            pathFinder = new PathFinder8(grid, true, (int)includeStart != 0);
                            break;
                    }

                    path = pathFinder.Run((int)xStart, (int)yStart, (int)xDestination, (int)yDestination);
                    pathBase64 = Convert.ToBase64String(path.ToByteArray());

                    dsMapItems.Add($"sp_path64_{pathId}", pathBase64);
                }
                catch (PathNotFoundException)
                {
                    lock (activePathsLock) ActivePaths.RemoveAll(item => item == pathId);
                    return;
                }
                catch (GridPositionOutOfBoundsException)
                {
                    lock (activePathsLock) ActivePaths.RemoveAll(item => item == pathId);
                    return;
                }
                catch (Exception ex)
                {
                    dsMapItems.Add($"sp_error_{pathId}", ex.Message);
                }

                lock (dsMapLock)
                {
                    int map;

                    map = GMExtension.DsMapCreate();
                    GMExtension.DsMapAdd(map, dsMapItems);

                    GMExtension.EventPerformAsync(map, EventType.EVENT_OTHER_SYSTEM_EVENT);
                    
                }

                lock (activePathsLock) ActivePaths.RemoveAll(item => item == pathId);

                lock (activeGridsLock)
                {
                    ActiveGrids.Remove((int)gridId);
                    Monitor.Pulse(activeGridsLock);
                }
            });

            return 1;
        }

        [DllExport("SpGridDelete", CallingConvention.Cdecl)]
        public static double SpGridDelete(double gridId)
        {
            Task.Run(() =>
            {
                lock(activeGridsLock)
                {
                    while (ActiveGrids.Contains((int)gridId))
                    {
                        Monitor.Wait(activeGridsLock);
                    }

                    GetGrid(gridId);

                    Grids[(int)gridId] = null;
                }
            });

            return 0;
        }

        [DllExport("SpGridSetObstacle", CallingConvention.Cdecl)]
        public static double SpGridSetObstacle(double gridId, double x, double y, double isObstacle)
        {
            Grid grid;

            Task.Run(() =>
            {
                lock (activeGridsLock)
                {
                    while (ActiveGrids.Contains((int)gridId))
                    {
                        Monitor.Wait(activeGridsLock);
                    }

                    grid = GetGrid(gridId);

                    grid.Nodes[(int)x, (int)y].IsObstacle = (int)isObstacle != 0;
                };
            });

            return 0;
        }

        [DllExport("SpGridClearObstacles", CallingConvention.Cdecl)]
        public static double SpGridClearObstacles(double gridId)
        {
            Grid grid;

            Task.Run(() =>
            {
                lock (activeGridsLock)
                {
                    while (ActiveGrids.Contains((int)gridId))
                    {
                        Monitor.Wait(activeGridsLock);
                    }

                    grid = GetGrid((int)gridId);

                    foreach (Node node in grid.Nodes)
                    {
                        node.IsObstacle = false;
                    }
                }
                
            });

            return 0;
        }

        static Grid GetGrid(double gridId)
        {
            if (gridId >= Grids.Count || Grids[(int)gridId] is null)
            {
                throw new ArgumentException($"Grid {gridId} was not found.", "gridId");
            }

            return Grids[(int)gridId];
        }
    }
}
