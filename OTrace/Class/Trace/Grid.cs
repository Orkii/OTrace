using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTrace.Class.Trace {
    /// <summary>
    /// Плата как куча клеточек
    /// </summary>
    internal class Grid {
        /// <summary>
        /// 
        /// </summary>
        public bool[,] padGrid;
        public double cellSize;
        Plate plate;

        public int height { get => padGrid.GetLength(0); }
        public int width  { get => padGrid.GetLength(1); }

        public Grid(Plate plate_) {
            plate = plate_;
            makeGrid();

        }

        public void makeGrid(double cellSize_ = 0.1) {//////////////
            cellSize = cellSize_;
            padGrid = new bool[(int)Math.Round(plate.board.size.X / cellSize), (int)Math.Round(plate.board.size.Y / cellSize)];

            foreach (Component component in plate.components) {
                Log.log(component + " Fill grid");
                component.fillGrid(this);

            }

        }

        public void paint(object sender, PaintEventArgs e, Vector3 panelOffset) {
            int x = padGrid.GetLength(0);
            int y = padGrid.GetLength(1);

            Pen pen = new Pen(Color.Gray, 1);

            //for (int i = 0; i < y + 1; i++) {   // + 1 Чтоб закрыть последнюю клеточку
            //    e.Graphics.DrawLine(pen,
            //        0 + panelOffset.X,
            //        (float)(-i * cellSize) * panelOffset.Z + panelOffset.Y + ((Panel)sender).Size.Height,
            //        (float)( x * cellSize) * panelOffset.Z + panelOffset.X,
            //        (float)(-i * cellSize) * panelOffset.Z + panelOffset.Y + ((Panel)sender).Size.Height);
            //}
            //
            //for (int i = 0; i < x + 1; i++) {   // + 1 Чтоб закрыть последнюю клеточку
            //    e.Graphics.DrawLine(pen,
            //        (float)(i * cellSize) * panelOffset.Z + panelOffset.X,
            //        ((Panel)sender).Size.Height + panelOffset.Y,
            //        (float)(i * cellSize) * panelOffset.Z + panelOffset.X,
            //        (float)(-y * cellSize) * panelOffset.Z + panelOffset.Y + ((Panel)sender).Size.Height);
            //}

            pen = new Pen(Color.Red, 1);
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (padGrid[i, j] == true) {
                        e.Graphics.DrawEllipse(pen,
                            (float)(i * cellSize) * panelOffset.Z + panelOffset.X,
                            (float)(-j * cellSize) * panelOffset.Z + panelOffset.Y + ((Panel)sender).Size.Height,
                            (float)(cellSize)     * panelOffset.Z   ,
                            (float)(cellSize)     * panelOffset.Z   );
                    }

                }

            }
        }
    }
}
