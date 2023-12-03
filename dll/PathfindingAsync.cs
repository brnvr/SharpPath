using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SharpPath
{
    public static class PathfindingAsync
    {
        public static GMExtension GMExtension;
        public static Dictionary<int, Grid> Grids = new Dictionary<int, Grid>();

        [DllExport("RegisterCallbacks", CallingConvention.Cdecl)]
        public unsafe static double RegisterCallbacks(char* arg0, char* arg1, char* arg2, char* arg3)
        {
            GMExtension = new GMExtension(arg0, arg1, arg2, arg3);

            return 0;
        }

        [DllExport("SpGridAdd", CallingConvention.Cdecl)]
        public static unsafe double SpGridAdd(double id, double width, char* bufferBase64)
        {
            byte[] buffer;

            buffer = Convert.FromBase64String(GMExtension.StringFromCharPointer(bufferBase64));

            Grids.Add((int)id, new Grid(buffer, (int)width));

            return 0;
        }

        [DllExport("SpGridFindPath", CallingConvention.Cdecl)]
        public static double SpGridFindPath(double id, double xStart, double yStart, double xDestination, double yDestination, double searchDirections, double includeStart)
        {
            Grid grid;

            grid = GetGrid(id);

            Task.Run(() =>
            {
                int map;
                Path path;
                string pathBase64;

                map = GMExtension.DsMapCreate();

                try
                {
                    path = grid.FindPath((int)xStart, (int)yStart, (int)xDestination, (int)yDestination, (SearchDirections)(int)searchDirections, (int)includeStart != 0);
                    pathBase64 = Convert.ToBase64String(path.ToByteArray());
                    

                    GMExtension.DsMapAddString(map, "sp_path64", pathBase64);
                }
                catch (Exception ex)
                {
                    GMExtension.DsMapAddString(map, "sp_error", ex.Message);

                }

                GMExtension.EventPerformAsync(map, EventType.EVENT_OTHER_SYSTEM_EVENT);
            });

            return 0;
        }

        [DllExport("SpGridRemove", CallingConvention.Cdecl)]
        public static double SpGridRemove(double id)
        {
            try
            {
                Grids.Remove((int)id);
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentException($"Grid {id} was not found.", "id", ex);
            }

            return 0;
        }

        [DllExport("SpGridSetObstacle", CallingConvention.Cdecl)]
        public static double SpGridSetObstacle(double id,  double x, double y, double isObstacle)
        {
            Grid grid;

            grid = GetGrid(id);

            grid.Nodes[(int)x, (int)y].IsObstacle = (int)isObstacle != 0;

            return 0;
        }

        static Grid GetGrid(double id)
        {
            try
            {
                return Grids[(int)id];
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentException($"Grid {id} was not found.", "id", ex);
            }
        }
    }
}
