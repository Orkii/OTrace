using System;
using System.Collections;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OTrace.Class.Trace {
    internal class Algorithm {
        Color[] colors = {
            Color.FromArgb(0  ,0  ,255),
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
            Color.FromArgb(255,0,255  )
        }; 
        public AlgoritmSettins settings;

        public RichTextBox infoRB;

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
        bool inWork = false; // Для отрисовки
        public void paint(object sender, PaintEventArgs e, Vector3 panelOffset) {
            toDraw = (Panel)sender;
            if (settings.showPlate == true) {
                plate.paint(sender, e, panelOffset);
            }
            if (settings.showGrid == true) {
                grid.paint(sender, e, panelOffset);
            }

            if (inWork == true) return;

            int c = 0; // Для разных цветов
            List<Net> netList_ = new List<Net>(netList);
            foreach (Net net in netList_) {
                if (net.routePoints.Count != 0) {
                    c++;  // Для разных цветов
                    foreach (Point routePoint in net.routePoints)
                    e.Graphics.FillRectangle(new SolidBrush(colors[c]),
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
            /* // Рисованеи Проверенных клеток
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
            */
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
            List<Net> temp = new List<Net>();
            foreach (Net net in netList) {
                if (net.pads.Count >= 2) {
                    temp.Add(net);
                } 
            
            }
            netList = temp;
        }

        //StashGrid stashGrid;
        
        Point nowPoint;
        public void alg(int netID = -1) {
            clearInfoText();

            writeInfoText("общее количество сетей = " + netList.Count);

            StashGrid stashGrid = new StashGrid(grid);
            stashGrid.grid = grid;
            bool able = true;

            foreach (Net net in netList) {
            }

            foreach (Net net in netList) {       // Проверка возможности каждой отдельной
                if (net.id != -1) {
                    if (findMultyRoute(stashGrid.clone(), net) == false) {
                        able = false;
                        writeInfoText("Невозможно провести путь " + net.id);
                    }
                    net.routePoints.Clear();
                    //findSingleRoute(stashGrid, net.id);
                }
            }
            if (able == false) return;
            else writeInfoText("Поиск пути");
            inWork = true;
            if (netID == -1) {
                recursiveRouteFinder(stashGrid, netList);
                /*
                foreach (Net net in netList) {
                    if (net.id != -1) {
                        findMultyRoute(stashGrid, net);
                        //findSingleRoute(stashGrid, net.id);
                    }
                }
                */
            }
            else {
                foreach (Net net in netList) {
                    if (net.id == netID) {
                        findMultyRoute(stashGrid, net);
                        toDraw.Invalidate();
                    }
                }
                //findSingleRoute(netID);
            }
            Console.WriteLine("DONE");
            inWork = false;
            toDraw.Invalidate();
        }
        int deep = 0;
        private bool recursiveRouteFinder(StashGrid stashGrid, List<Net> nets) {
            //clearInfoText();
            if (nets.Count == 0) return true;

            writeInfoText(deep.ToString() + " ");
            deep++;
            List<Net> wasTry = new List<Net>(); // Эти сети были попробываны 
            wasTry.Sort();

            Dictionary<Net, List<Point>> temp1 = new Dictionary<Net, List<Point>>();
            Dictionary<Net, StashGrid>   temp2 = new Dictionary<Net, StashGrid>();

            foreach (Net item in nets) {
                StashGrid sg = stashGrid.clone();
                if (findMultyRoute(sg, item) == false) {
                    deep--;
                    return false;
                }
                else {
                    temp1.Add(item, item.routePoints);
                    temp2.Add(item, sg);
                }
            }

            nets.Sort();

            Net nowNet; // Сеть которая сейчас в рекурсии

            while (nets.Count != 0) {
                nowNet = nets.First(); 
                nets.Remove(nowNet);

                StashGrid stashGridNewState;
                temp2.TryGetValue(nowNet, out stashGridNewState);
                temp1.TryGetValue(nowNet, out nowNet.routePoints);

                List<Net> tempList = new List<Net>();
                tempList.AddRange(nets);
                tempList.AddRange(wasTry);

                if (recursiveRouteFinder(stashGridNewState, tempList) == true) {
                    deep--;
                    return true;
                }

                nowNet.routePoints.Clear();
                wasTry.Add(nowNet);

                /*
                StashGrid stashGridNewState = stashGrid.clone();
                if (findMultyRoute(stashGridNewState, nowNet) == false) {
                    deep--;
                    return false;
                }
                else {
                    List<Net> tempList = new List<Net>();
                    tempList.AddRange(nets);
                    tempList.AddRange(wasTry);
                    if (recursiveRouteFinder(stashGridNewState, tempList) == true) {
                        deep--;
                        return true;
                    }
                    nowNet.routePoints.Clear();
                    wasTry.Add(nowNet);
                }
                */

            }
            deep--;
            return false;
        }

        /// <summary>
        /// находит путь для всей сети
        /// </summary>
        /// <param name="netID"></param>
        /// <returns></returns>
        public bool findMultyRoute(StashGrid stashGrid, Net net) {
            List<Point> ps = net.pads[0].cellUsageL;
            for (int i = 1; i < net.pads.Count; i++) {
                List<Point>  newPs = findWayFromArrToArr(stashGrid, ps, net.pads[i].cellUsageL, net);
                if (newPs == null) return false;
                else ps = newPs;
            }

            net.routePoints = ps;
            RouteGrid gr = new RouteGrid(ps, grid.padGrid.GetLength(0), grid.padGrid.GetLength(1));
            stashGrid.addRouteGrid(gr);
            
            //toDraw.Invalidate();
            return true;
        }
        /// <summary>
        /// Находит путь от кучи точек до другой. stashGrid - Поле на котором проводится, 
        /// </summary>
        /// <param name="netID"></param>
        /// <returns>null = Пути нет.</returns>
        public List<Point> findWayFromArrToArr(StashGrid stashGrid, List<Point> ps1, List<Point> ps2, Net net) {
            SortedListP sortedList = new SortedListP(ps2);

            foreach (Point a in ps1) {
                sortedList.add(a, 0);
            }
            bool foundWay = false;
            PointWithDist lastCell = null;

            while (foundWay == false) {
                PointWithDist p = sortedList.pickD();
                if (p == null) {
                    return null;
                }

                Point[] ps = {// Соседние клетки
                    new Point(p.point.X + 1, p.point.Y),
                    new Point(p.point.X - 1, p.point.Y),
                    new Point(p.point.X, p.point.Y + 1),
                    new Point(p.point.X, p.point.Y - 1)
                };
                foreach (Point a in ps) {
                    if ((a.X == -1) || (a.Y == -1) || (a.Y >= grid.height) || (a.Y >= grid.width)) continue; // Выходят за границы

                    foreach (Point b in ps2) {
                        if ((a.X == b.X) && (a.Y == b.Y)) {
                            Console.WriteLine("Found Way");                             // Нашли путь
                            int distance = Math.Abs(sortedList.destination[0].X - a.X) + Math.Abs(sortedList.destination[0].Y - a.Y);
                            lastCell = new PointWithDist(a, distance, p.routeLeng + 1, p);
                            foundWay = true;
                        }
                    }


                    if (stashGrid.isOccupied(a.X, a.Y) == false) {                      // Если в клетке ничего нет
                        if (sortedList.alreadyUsed.ContainsKey(a) == false) {           // И дорожек текущего шага тоже
                            sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);    // Добовляем возможный путь
                        }
                        else {
                            PointWithDist usedDot = null;
                            sortedList.alreadyUsed.TryGetValue(a, out usedDot);         // Иначе
                            if (usedDot.routeLeng > p.routeLeng + 1) {                  // Если текущий путь короче
                                sortedList.remove(a);                                   // Заменяем
                                sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);
                            }
                        }
                    }
                    /*

                    if (stashGrid.isOccupied(a.X, a.Y) == false) {                      // Если в клетке ничего нет
                        if (sortedList.alreadyUsed.ContainsKey(a) == false) {           // И дорожек текущего шага тоже
                            sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);    // Добовляем возможный путь
                        }
                        else {
                            PointWithDist usedDot = null;
                            sortedList.alreadyUsed.TryGetValue(a, out usedDot);         // Иначе
                            if (usedDot.routeLeng > p.routeLeng + 1) {                  // Если текущий путь короче
                                sortedList.remove(a);                                   // Заменяем
                                sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);
                            }
                        }
                    }
                    
                    */
                }
            }
            // Обратный поиск
            List<Point> routeList = new List<Point>();

            while (lastCell.prevPoint != null) {
                routeList.Add(lastCell.point);
                lastCell = lastCell.prevPoint;
            }
            routeList.AddRange(ps1);
            routeList.AddRange(ps2);
            return routeList;
        }
        public bool findSingleRoute(StashGrid stashGrid, int netID) {
            Net net = netList[netID];

            Pad pad1 = net.pads[0];
            Pad pad2 = net.pads[1];
             
            //RouteGrid routeGrid = new RouteGrid();
            SortedListP sortedList = new SortedListP(pad2.cellUsageL);
            foreach (Point a in pad1.cellUsageL) {
                sortedList.add(a, 0);
            }

            bool foundWay = false;
            PointWithDist lastCell = null;
            while (foundWay == false) {
                PointWithDist p = sortedList.pickD();

                if (p == null) {
                    return false;
                }
                nowPoint = p.point;


                Point[] ps = {// Соседние клетки
                    new Point(p.point.X + 1, p.point.Y),
                    new Point(p.point.X - 1, p.point.Y),
                    new Point(p.point.X, p.point.Y + 1),
                    new Point(p.point.X, p.point.Y - 1)
                };

                foreach (Point a in ps) {
                    if ((a.X == -1) || (a.Y == -1) || (a.Y >= grid.height) || (a.Y >= grid.width)) continue; // Выходят за границы

                    foreach (Point b in pad2.cellUsageL) {
                        if ((a.X == b.X) && (a.Y == b.Y)) {
                            Console.WriteLine("Found Way");                             // Нашли путь


                            int distance = Math.Abs(sortedList.destination[0].X - a.X) + Math.Abs(sortedList.destination[0].Y - a.Y);
                            lastCell = new PointWithDist(a, distance, p.routeLeng + 1, p);

                            foundWay = true;
                        }
                    }

                    if (stashGrid.isOccupiedInRadius(a.X, a.Y, net.routeWidth) == false) {                      // Если в клетке ничего нет

                        if (sortedList.alreadyUsed.ContainsKey(a) == false) {           // И дорожек текущего шага тоже
                            sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);    // Добовляем возможный путь
                        }
                        else {
                            PointWithDist usedDot = null;
                            sortedList.alreadyUsed.TryGetValue(a, out usedDot);         // Иначе
                            if (usedDot.routeLeng > p.routeLeng + 1) {                  // Если текущий путь короче
                                sortedList.remove(a);                                   // Заменяем
                                sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);
                            }
                        }
                    }


                    /*
                    if (stashGrid.isOccupied(a.X, a.Y) == false) {                      // Если в клетке ничего нет

                        if (sortedList.alreadyUsed.ContainsKey(a) == false) {           // И дорожек текущего шага тоже
                            sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);    // Добовляем возможный путь
                        }
                        else {
                            PointWithDist usedDot = null;
                            sortedList.alreadyUsed.TryGetValue(a, out usedDot);         // Иначе
                            if (usedDot.routeLeng > p.routeLeng + 1) {                  // Если текущий путь короче
                                sortedList.remove(a);                                   // Заменяем
                                sortedList.add(new Point(a.X, a.Y), p.routeLeng + 1, p);
                            }
                        }
                    }
                    */



                    //Thread.Sleep(10);
                }
                toDraw.Invalidate();
            }
            // Обратный поиск
            List<Point> routeList = new List<Point>();

            while (lastCell.prevPoint != null) {
                routeList.Add(lastCell.point);



                int cellOccupiedRadius = (int)(Math.Floor(net.routeWidth / grid.cellSize / 2));
                int cellOccupiedDiametr = cellOccupiedRadius * 2;

                for (int i = -cellOccupiedRadius; i < cellOccupiedRadius; i++) {
                    for (int j = -cellOccupiedRadius; j < cellOccupiedRadius; j++) {
                        routeList.Add(new Point(lastCell.point.X + i, lastCell.point.Y + j));
                    }
                }


                lastCell = lastCell.prevPoint;
            }
            net.routePoints = routeList;
            
            sortedList = null;

            RouteGrid gr = new RouteGrid(routeList, grid.padGrid.GetLength(0), grid.padGrid.GetLength(1));
            stashGrid.addRouteGrid(gr);

            toDraw.Invalidate();


            return true;
        }
        public void writeInfoText(string text) {
            if (infoRB.InvokeRequired) {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { writeInfoText($"{text}"); };
                infoRB.Invoke(safeWrite);
            }
            else {
                infoRB.AppendText(text);
                infoRB.ScrollToCaret();
            }
        }
        public void clearInfoText() {
            if (infoRB.InvokeRequired) {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { clearInfoText(); };
                infoRB.Invoke(safeWrite);
            }
            else
                infoRB.Clear();
        }
    }






}
