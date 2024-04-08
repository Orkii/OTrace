using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OTrace.Class.Trace {
    internal class StashGrid {
        public Grid grid;
        public List<RouteGrid> routeGrids;

        public StashGrid(Grid grid_) {
            grid = grid_;
            routeGrids = new List<RouteGrid>();
        }
        public void addRouteGrid(RouteGrid routeGrid_) {
            routeGrids.Add(routeGrid_);
        }

        public bool isOccupied(Point p ) {
            return isOccupied(p.X, p.Y);
        }

        public bool isOccupied(int x, int y) {
            try {
                if (grid.padGrid[x, y] == true) return true;
                foreach (RouteGrid a in routeGrids) {
                    if (a.routeGrid[x, y] == true) return true;
                }
                return false;
            }
            catch {
                return true;
            }
        }
    }
}
