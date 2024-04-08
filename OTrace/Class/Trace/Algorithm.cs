using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTrace.Class.Trace {
    internal class Algorithm {
        Color[] colors = {
            Color.FromArgb(240,248,255),
            Color.FromArgb(255,160,122),
            Color.FromArgb(250,235,215),
            Color.FromArgb(32,178,170 ),
            Color.FromArgb(0,255,255  ),
            Color.FromArgb(135,206,250),
            Color.FromArgb(127,255,212),
            Color.FromArgb(119,136,153),
            Color.FromArgb(240,255,255),
            Color.FromArgb(176,196,222),
            Color.FromArgb(245,245,220),
            Color.FromArgb(255,255,224),
            Color.FromArgb(255,228,196),
            Color.FromArgb(0,255,0    ),
            Color.FromArgb(0,0,0      ),
            Color.FromArgb(50,205,50  ),
            Color.FromArgb(255,255,205),
            Color.FromArgb(250,240,230),
            Color.FromArgb(0,0,255    ),
            Color.FromArgb(255,0,255  )
        }; 
        public AlgoritmSettins settings;

        public Plate plate;
        public Grid grid;


        List<Net> netList;

        Panel toDraw;

        public Algorithm(Plate plate_) {
            plate = plate_;
            grid = new Grid(plate);
            settings = new AlgoritmSettins();

            initNetList();
        }

        public void paint(object sender, PaintEventArgs e, Vector3 panelOffset) {
            toDraw = (Panel)sender;
            if (settings.showPlate == true) {
                plate.paint(sender, e, panelOffset);
            }
            if (settings.showGrid == true) {
                grid.paint(sender, e, panelOffset);
            }

            foreach (Net net in netList) {
                if (net.routePoints.Count != 0) {
                    foreach(Point routePoint in net.routePoints)
                    e.Graphics.FillEllipse(new SolidBrush(Color.Blue),
                        (float)(panelOffset.X + (routePoint.X * grid.cellSize) * panelOffset.Z),
                        (float)(panelOffset.Y + ((Panel)sender).Size.Height - (routePoint.Y * grid.cellSize) * panelOffset.Z),
                        (float)grid.cellSize * panelOffset.Z,
                        (float)grid.cellSize * panelOffset.Z);
                    /*
                     e.Graphics.FillEllipse(new SolidBrush(Color.Blue),
                        (float)(panelOffset.X + (routePoint.X * grid.cellSize - grid.cellSize / 2) * panelOffset.Z),
                        (float)(panelOffset.Y + ((Panel)sender).Size.Height - (routePoint.Y * grid.cellSize + grid.cellSize / 2) * panelOffset.Z),
                        (float)grid.cellSize * panelOffset.Z,
                        (float)grid.cellSize * panelOffset.Z);
                     */
                }
                for (int i = 1; i < net.pads.Count; i++) {

                    Pad pad1 = net.pads[i - 1];
                    Pad pad2 = net.pads[i];

                    PointF p1 = new PointF(pad1.position.X, pad1.position.Y);
                    PointF p2 = new PointF(pad2.position.X, pad2.position.Y);
                    {
                        Vector2 newOffset = Vector2.Transform(pad1.offset, Matrix3x2.CreateRotation((float)pad1.angle__));
                        p1 = new PointF(
                            panelOffset.X + (pad1.position.X + newOffset.X) * panelOffset.Z,
                            panelOffset.Y + ((Panel)sender).Size.Height - (pad1.position.Y + newOffset.Y) * panelOffset.Z);
                    }
                    {
                        Vector2 newOffset = Vector2.Transform(pad2.offset, Matrix3x2.CreateRotation((float)pad2.angle__));
                        p2 = new PointF(
                            panelOffset.X + (pad2.position.X + newOffset.X) * panelOffset.Z,
                            panelOffset.Y + ((Panel)sender).Size.Height - (pad2.position.Y + newOffset.Y) * panelOffset.Z);
                    }
                    //p1.X = p1.X * panelOffset.Z + panelOffset.X;
                    //p1.Y = -p1.Y * panelOffset.Z + panelOffset.Y + ((Panel)sender).Size.Height;
                    //
                    //p2.X = p2.X * panelOffset.Z + panelOffset.X;
                    //p2.Y = -p1.Y * panelOffset.Z + panelOffset.Y + ((Panel)sender).Size.Height;

                    e.Graphics.DrawLine(new Pen(Color.Green), p1, p2);
                }
            }
            if (sortedList != null) {
                Point[] ppp = new Point[sortedList.alreadyUsed.Keys.Count*2];
                sortedList.alreadyUsed.Keys.CopyTo(ppp, 0);
                foreach (Point p in ppp) {
                    e.Graphics.FillEllipse(new SolidBrush(Color.Orange),
                        (float)(panelOffset.X + (p.X * grid.cellSize - grid.cellSize / 2) * panelOffset.Z),
                        (float)(panelOffset.Y + ((Panel)sender).Size.Height - (p.Y * grid.cellSize + grid.cellSize / 2) * panelOffset.Z),
                        (float)grid.cellSize * panelOffset.Z,
                        (float)grid.cellSize * panelOffset.Z);
                }
            }
            if (nowPoint != null) {
                e.Graphics.FillEllipse(new SolidBrush(Color.Orange),
                    (float)(panelOffset.X + (nowPoint.X * grid.cellSize - grid.cellSize / 2) * panelOffset.Z),
                    (float)(panelOffset.Y + ((Panel)sender).Size.Height - (nowPoint.Y * grid.cellSize + grid.cellSize / 2) * panelOffset.Z),
                    (float)grid.cellSize * panelOffset.Z,
                    (float)grid.cellSize * panelOffset.Z);
            }
        }

        public void initNetList() {
            netList = new List<Net>();
            foreach (Component comp in plate.components) {
                foreach (Pad pad in comp.patternStyle.padList) {
                    if (pad.netId != -1) {
                        pad.color = Color.Black;// colors[pad.netId];
                    }
                    bool isNetWithSameIdExist = false;
                    foreach (Net net in netList){
                        if (net.id == pad.netId) {
                            net.pads.Add(pad);
                            isNetWithSameIdExist = true;
                            break;
                        }
                    }
                    if (isNetWithSameIdExist == false) {
                        Net net = new Net(pad.netId);
                        net.pads.Add(pad);
                        netList.Add(net);
                    }
                }
            }

        }

        StashGrid stashGrid;
        SortedListP sortedList;
        Point nowPoint;
        public void alg(int netID = -1) {
            if (stashGrid == null) {
                stashGrid = new StashGrid(grid);
                stashGrid.grid = grid;
            }

            if (netID == -1) {
                foreach (Net net in netList) {
                    if (net.id != -1) {
                        findSingleRoute(net.id);
                    }
                }
            }
            else {
                findSingleRoute(netID);
            }
        }
        public bool findSingleRoute(int netID) {
            Net net = netList[netID];

            Pad pad1 = net.pads[0];
            Pad pad2 = net.pads[1];

            //RouteGrid routeGrid = new RouteGrid();

            sortedList = new SortedListP(pad2.cellUsageL[0]);
            foreach (Point a in pad1.cellUsageL) {
                sortedList.add(a, 0);
            }


            //sortedList.add(pad1.cellUsageL[0], 0);
            //sortedList.add(pad1.cellUsageL[0], 0);

            bool foundWay = false;
            PointWithDist lastCell = null;
            while (foundWay == false) {
                PointWithDist p = sortedList.pickD();

                if (p == null) {
                    return false;
                }
                nowPoint = p.point;


                Point[] ps = {
                    new Point(p.point.X + 1, p.point.Y),
                    new Point(p.point.X - 1, p.point.Y),
                    new Point(p.point.X, p.point.Y + 1),
                    new Point(p.point.X, p.point.Y - 1)
                };

                foreach (Point a in ps) {
                    if ((a.X == -1) || (a.Y == -1) || (a.Y >= grid.height) || (a.Y >= grid.width)) continue;

                    foreach (Point b in pad2.cellUsageL) {
                        if ((a.X == b.X) && (a.Y == b.Y)) {
                            Console.WriteLine("Found Way");

                            
                            int distance = Math.Abs(sortedList.destination.X - a.X) + Math.Abs(sortedList.destination.Y - a.Y);
                            lastCell = new PointWithDist(a, distance, p.routeLeng + 1, p);

                            foundWay = true;
                        }
                    }

                    if (stashGrid.isOccupied(a.X, a.Y) == false) {
                        if (sortedList.alreadyUsed.ContainsKey(a) == false) {
                            sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);
                        }
                        else {
                            PointWithDist usedDot = null;
                            sortedList.alreadyUsed.TryGetValue(a, out usedDot);
                            if (usedDot.routeLeng > p.routeLeng + 1) {
                                sortedList.remove(a);
                                sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);
                            }
                        }
                    }




                    //Thread.Sleep(10);
                }
                toDraw.Invalidate();
            }
            // Обратный поиск
            List<Point> routeList = new List<Point>();

            while (lastCell.prevPoint != null) {
                routeList.Add(lastCell.point);
                lastCell = lastCell.prevPoint;
            }
            net.routePoints = routeList;
            
            sortedList = null;

            RouteGrid gr = new RouteGrid(routeList, grid.padGrid.GetLength(0), grid.padGrid.GetLength(1));
            stashGrid.addRouteGrid(gr);

            toDraw.Invalidate();


            return true;
        }
    }
}
