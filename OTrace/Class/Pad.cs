using System;
using OTrace.Class.Trace;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Linq;
using System.Collections;

namespace OTrace.Class {
    internal class Pad {
        public int id;
        //public string style;
        public PadStyle style;
        public Vector2 offset;
        public float angle;
        public int netId;

        public Vector2 position;
        public double angle__;

        public Hashtable cellUsage;
        public List<Point> cellUsageL;

        public XmlElement xml;

        public Color color = Color.Black;

        //List<PointF> points = new List<PointF>();

        public Pad(XmlElement xml_, List<PadStyle> padStyleList) {
            xml = xml_;
            id = int.Parse(xml.Attributes["Id"].InnerText);


            string padStyleStr = xml.Attributes["Style"].InnerText;

            foreach (PadStyle padStyle in padStyleList) {
                if (padStyle.name == padStyleStr) {
                    style = padStyle;
                    break;
                }
            }
            if (style == null) throw (new Exception());

            offset = new Vector2(
                float.Parse(xml.Attributes["X"].InnerText.Replace(".", ",")),
                float.Parse(xml.Attributes["Y"].InnerText.Replace(".", ",")));
            angle = float.Parse(xml.Attributes["Angle"].InnerText.Replace(".", ","));

            cellUsage = new Hashtable();
            //cellUsage["key"]
        }


        public TreeNode drawToTree() {
            TreeNode node = new TreeNode(id.ToString());
            node.Nodes.Add("Style: " + style);
            node.Nodes.Add("Offset:" + offset);
            node.Nodes.Add("Angle:" + angle);
            node.Nodes.Add("NetId:" + netId);
            TreeNode point = new TreeNode("Points");
            //foreach (PointF p in points) {
            //    point.Nodes.Add(p.ToString());
            //}
            node.Nodes.Add(point);
            node.Nodes.Add("NetId:" + netId);
            return node;
        }

        public void paint(object sender, PaintEventArgs e, Vector2 pos, double angle_, Vector3 panelOffset) {
            position = pos;
            angle__ = angle_;
            Pen pen = new Pen(color);
            Vector2 newOffset = Vector2.Transform(offset, Matrix3x2.CreateRotation((float)angle_));



            if (style.shape == "Ellipse") {
                //e.Graphics.DrawEllipse(pen,
                //    panelOffset.X +                               (pos.X - style.size.X / 2 + newOffset.X) * panelOffset.Z,
                //    panelOffset.Y + ((Panel)sender).Size.Height - (pos.Y + style.size.Y / 2 + newOffset.Y) * panelOffset.Z,
                //    style.size.X * panelOffset.Z,
                //    style.size.Y * panelOffset.Z);
                e.Graphics.FillEllipse(new SolidBrush(color),
                    panelOffset.X + (pos.X - style.size.X / 2 + newOffset.X) * panelOffset.Z,
                    panelOffset.Y + ((Panel)sender).Size.Height - (pos.Y + style.size.Y / 2 + newOffset.Y) * panelOffset.Z,
                    style.size.X * panelOffset.Z,
                    style.size.Y * panelOffset.Z);
            }
            else if ((style.shape == "Rectangle") || style.shape == "Obround") {
                Vector2[] vec = {
                    new Vector2( style.size.X/2,  style.size.Y/2),
                    new Vector2(-style.size.X/2,  style.size.Y/2),
                    new Vector2(-style.size.X/2, -style.size.Y/2),
                    new Vector2( style.size.X/2, -style.size.Y/2) };
                PointF[] pts = new PointF[4];

                double totalAngle = angle + angle_;

                for (int i = 0; i < pts.Length; i++) {
                    //Console.WriteLine("Angle = " + angle);
                    vec[i] = Vector2.Transform(vec[i], Matrix3x2.CreateRotation((float)totalAngle));
                    pts[i] = new PointF(vec[i].X, vec[i].Y);


                    pts[i].X *= panelOffset.Z;
                    pts[i].Y = pts[i].Y * panelOffset.Z + ((Panel)sender).Size.Height;

                    pts[i].X += panelOffset.X + (pos.X + newOffset.X) * panelOffset.Z;
                    pts[i].Y += panelOffset.Y - (pos.Y + newOffset.Y) * panelOffset.Z;
                }
                e.Graphics.FillPolygon(new SolidBrush(color), pts);
            }
            //foreach (PointF p in points) {
            //
            //    e.Graphics.DrawEllipse(new Pen(Color.Red),
            //        panelOffset.X +                               (p.X - 0.02f) * panelOffset.Z,
            //        panelOffset.Y + ((Panel)sender).Size.Height - (p.Y + 0.02f) * panelOffset.Z,
            //        0.1f * panelOffset.Z,
            //        0.1f * panelOffset.Z);
            //}

        }

        public void fillGrid(Grid grid, Vector2 pos, double angle_) {
            {
                double totalAngle = angle + angle_;
                Vector2 newOffset = Vector2.Transform(offset, Matrix3x2.CreateRotation((float)angle_));

                int aa1 = (int)(Math.Floor((pos.X + newOffset.X) / grid.cellSize));
                int aa2 = (int)(Math.Ceiling((pos.Y + newOffset.Y) / grid.cellSize));
                //grid.padGrid[aa1, aa2] = true;
            }

            if ((style.shape == "Rectangle") || style.shape == "Obround" || style.shape == "Ellipse") {

                Log.log(id + " Pad Fill grid");

                Vector2[] vec = {
                    new Vector2( style.size.X/2,  style.size.Y/2 ),
                    new Vector2(-style.size.X/2,  style.size.Y/2 ),
                    new Vector2(-style.size.X/2, -style.size.Y/2 ),
                    new Vector2( style.size.X/2, -style.size.Y/2 ),

                    new Vector2( style.size.X/2,  style.size.Y/2 )
                }; //Same as pts[0]

                PointF[] pts = new PointF[5];
                double totalAngle = angle + angle_;
                Vector2 newOffset = Vector2.Transform(offset, Matrix3x2.CreateRotation((float)angle_));

                for (int i = 0; i < pts.Length; i++) {
                    //Console.WriteLine("Angle = " + angle);
                    vec[i] = Vector2.Transform(vec[i], Matrix3x2.CreateRotation((float)totalAngle));
                    pts[i] = new PointF(vec[i].X, vec[i].Y);

                    pts[i].X += pos.X + newOffset.X;
                    pts[i].Y += pos.Y + newOffset.Y;
                    {
                        int aa1 = (int)(Math.Floor((pts[i].X) / grid.cellSize));
                        int aa2 = (int)(Math.Ceiling((pts[i].Y) / grid.cellSize));
                        //grid.padGrid[aa1, aa2] = true;
                    }
                }


                int x = grid.padGrid.GetLength(0);
                int y = grid.padGrid.GetLength(1);

                float eps = (float)grid.cellSize / 100000;

                Console.WriteLine(style.name);


                for (int k = 0; k < pts.Length - 1; k++) {

                    for (int i = 1; i < x - 1; i++) {
                        PointF p3 = new PointF((float)(i * grid.cellSize), 0);
                        PointF p4 = new PointF((float)(i * grid.cellSize), (float)(y * grid.cellSize));

                        PointF pCross;
                        bool cross = Interact.cross(pts[k], pts[k + 1], p4, p3, out pCross);

                        if (cross == true) {
                            //Log.log(pCross + " is cross");
                            //Console.WriteLine("pCross = " + pCross);
                            int aa1 = (int)(Math.Floor((pCross.X + grid.cellSize / 2) / grid.cellSize));
                            int aa2 = (int)(Math.Ceiling(pCross.Y / grid.cellSize));
                            //points.Add(pCross);
                            if (aa1 < x && aa2 < y && aa1 > 0 && aa2 > 0) {
                                grid.padGrid[aa1, aa2] = true;
                                grid.padGrid[aa1 - 1, aa2] = true;
                                if (cellUsage.ContainsKey((aa1).ToString() + ";" + (aa2).ToString()) == false) {
                                    cellUsage.Add(((aa1).ToString() + ";" + (aa2).ToString()), "True");
                                }

                                if (cellUsage.ContainsKey((aa1 - 1).ToString() + ";" + (aa2).ToString()) == false) {
                                    cellUsage.Add(((aa1 - 1).ToString() + ";" + (aa2).ToString()), "True");
                                }
                            }

                            if ((int)(Math.Ceiling((pCross.Y + eps) / grid.cellSize)) != aa2) {
                                grid.padGrid[aa1, aa2 + 1] = true;
                                grid.padGrid[aa1 - 1, aa2 + 1] = true;
                                if (cellUsage.ContainsKey(aa1.ToString() + ";" + (aa2 + 1).ToString()) == false) {
                                    cellUsage.Add((aa1.ToString() + ";" + (aa2 + 1).ToString()), "True");
                                }
                                if (cellUsage.ContainsKey((aa1 - 1).ToString() + ";" + (aa2 + 1).ToString()) == false) {
                                    cellUsage.Add(((aa1 - 1).ToString() + ";" + (aa2 + 1).ToString()), "True");
                                }
                            }
                            if ((int)(Math.Ceiling((pCross.Y - eps) / grid.cellSize)) != aa2) {
                                grid.padGrid[aa1, aa2 - 1] = true;
                                grid.padGrid[aa1 - 1, aa2 - 1] = true;

                                if (cellUsage.ContainsKey(aa1.ToString() + ";" + (aa2 - 1).ToString()) == false) {
                                    cellUsage.Add((aa1.ToString() + ";" + (aa2 - 1).ToString()), "True");
                                }
                                if (cellUsage.ContainsKey((aa1 - 1).ToString() + ";" + (aa2 - 1).ToString()) == false) {
                                    cellUsage.Add(((aa1 - 1).ToString() + ";" + (aa2 - 1).ToString()), "True");
                                }
                            }


                        }
                        //else {
                        //    Log.log("No cross");
                        //}
                    }


                    for (int i = 1; i < y - 1; i++) {
                        PointF p1 = new PointF(0, (float)(i * grid.cellSize));
                        PointF p2 = new PointF((float)(x * grid.cellSize), (float)(i * grid.cellSize));

                        PointF pCross;
                        bool cross = Interact.cross(pts[k], pts[k + 1], p1, p2, out pCross);

                        if (cross == true) {
                            //Log.log(pCross + " is cross");
                            //Console.WriteLine("pCross = " + pCross);
                            int aa1 = (int)(Math.Floor(pCross.X / grid.cellSize));
                            int aa2 = (int)(Math.Ceiling((pCross.Y + grid.cellSize / 2) / grid.cellSize));
                            //points.Add(pCross);
                            if (aa1 < x && aa2 < y && aa1 > 0 && aa2 > 0) {
                                grid.padGrid[aa1, aa2] = true;
                                grid.padGrid[aa1, aa2 - 1] = true;

                                if (cellUsage.ContainsKey((aa1).ToString() + ";" + (aa2).ToString()) == false) {
                                    cellUsage.Add(((aa1).ToString() + ";" + (aa2).ToString()), "True");
                                }

                                if (cellUsage.ContainsKey((aa1).ToString() + ";" + (aa2 - 1).ToString()) == false) {
                                    cellUsage.Add(((aa1).ToString() + ";" + (aa2 - 1).ToString()), "True");
                                }
                            }

                            if ((int)(Math.Floor((pCross.X + eps) / grid.cellSize)) != aa1) {
                                grid.padGrid[aa1 + 1, aa2] = true;
                                grid.padGrid[aa1 + 1, aa2 - 1] = true;

                                if (cellUsage.ContainsKey((aa1 + 1).ToString() + ";" + (aa2).ToString()) == false) {
                                    cellUsage.Add(((aa1 + 1).ToString() + ";" + (aa2).ToString()), "True");
                                }
                                if (cellUsage.ContainsKey((aa1 + 1).ToString() + ";" + (aa2 - 1).ToString()) == false) {
                                    cellUsage.Add(((aa1 + 1).ToString() + ";" + (aa2 - 1).ToString()), "True");
                                }
                            }
                            if ((int)(Math.Floor((pCross.X - eps) / grid.cellSize)) != aa1) {
                                grid.padGrid[aa1 - 1, aa2] = true;
                                grid.padGrid[aa1 - 1, aa2 - 1] = true;

                                if (cellUsage.ContainsKey((aa1 - 1).ToString() + ";" + (aa2).ToString()) == false) {
                                    cellUsage.Add(((aa1 - 1).ToString() + ";" + (aa2).ToString()), "True");
                                }
                                if (cellUsage.ContainsKey((aa1 - 1).ToString() + ";" + (aa2 - 1).ToString()) == false) {
                                    cellUsage.Add(((aa1 - 1).ToString() + ";" + (aa2 - 1).ToString()), "True");
                                }
                            }

                        }
                        //else {
                        //    Log.log("No cross");
                        //}
                    }

                }

            }
            cellUsageL = new List<Point>();
            foreach (string key in cellUsage.Keys) {
                string[] sp = key.Split(';');
                int p1 = int.Parse(sp[0]);
                int p2 = int.Parse(sp[1]);

                cellUsageL.Add(new Point(p1, p2));
            }

        }

    }
}

