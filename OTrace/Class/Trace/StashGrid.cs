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
        bool[,] routeGrid;


        public StashGrid(Grid grid_) {
            grid = grid_;
            routeGrids = new List<RouteGrid>();
            routeGrid = new bool[grid.height, grid.width];
            for (int i = 0; i < grid.padGrid.GetLength(0); i++) {
                for (int j = 0; j < grid.padGrid.GetLength(1); j++) {
                    routeGrid[i, j] = grid.padGrid[i, j];
                }
            }
        }


        private void addRouteGridInBypassing(RouteGrid routeGrid_) {
            routeGrids.Add(routeGrid_);
        }

        public void addRouteGrid(RouteGrid routeGrid_) {
            routeGrids.Add(routeGrid_);

            for (int i = 0; i < grid.padGrid.GetLength(0); i++) {
                for (int j = 0; j < grid.padGrid.GetLength(1); j++) {
                    routeGrid[i, j] = routeGrid_.routeGrid[i, j] || routeGrid[i, j];///
                }
            }


        }

        public bool isOccupied(Point p ) {
            return isOccupied(p.X, p.Y);
        }

        public StashGrid clone() {
            StashGrid sg = new StashGrid(grid);
            foreach (RouteGrid a in routeGrids) {
                sg.addRouteGridInBypassing(a);
            }
            sg.routeGrid = (bool[,])routeGrid.Clone();
            //sg.routeGrids = new List<RouteGrid>(routeGrids);
            return sg;
        }
        public bool isOccupiedInRadius(int x, int y, double radius) {
            int cellOccupiedRadius = (int)(Math.Floor(radius / grid.cellSize / 2));
            int cellOccupiedDiametr = cellOccupiedRadius * 2;

            for (int i = -cellOccupiedRadius; i > cellOccupiedRadius; i++) {
                for (int j = -cellOccupiedRadius; j > cellOccupiedRadius; j++) {
                    if (isOccupied(x + i, y + j) == true) return true;
                }
            }

            return false;
        }
        public bool isOccupied(int x, int y) {
            /*
            
            if ((y < 0) || (y > grid.padGrid.GetLength(1) - 2)) return false;

            if (grid.padGrid[x, y] == true) return true;
            foreach (RouteGrid a in routeGrids) {
                if (a.routeGrid[x, y] == true) return true;
            }
            return false;
            */
            if ((x < 0) || (x >= grid.padGrid.GetLength(0))) return true;
            if ((y < 0) || (y >= grid.padGrid.GetLength(1))) return true;
            //try {

            return routeGrid[x, y];

            if (grid.padGrid[x, y] == true) return true;
            foreach (RouteGrid a in routeGrids) {
                if (a.routeGrid[x, y] == true) return true;
            }
            return false;
            //}
            //catch {
            //    return true;
            //}
        }
    }
}
