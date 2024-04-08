using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTrace.Class.Trace {
    internal class RouteGrid {
        public bool[,] routeGrid;

        public RouteGrid(List<Point> routelist, int sizeX, int sizeY) { 
            routeGrid = new bool[sizeX, sizeY];
            foreach (Point p in routelist) {
                routeGrid[p.X, p.Y] = true;
            }
        }

    }
}
