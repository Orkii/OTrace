using OTrace.Class.Trace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OTrace.Class {
    internal class PadPattern {
        public string patternStyle;
        public string name;
        public Vector2 origin;
        //public Vector2 size;
        public List<Pad> padList;
        public List<Hole> holeList;
        public XmlElement xml;



        public List<PadStyle> padStyleList;
        public PadPattern(XmlElement xml_, List<PadStyle> padStyleList_) {
            padStyleList = padStyleList_;
            xml = xml_;
            patternStyle = xml.Attributes["PatternStyle"].InnerText;
            {
                XmlNode a = xml.SelectSingleNode("Name");
                if (a != null) {
                    name = a.InnerText;
                }
                else {
                    name = patternStyle;
                }
            }
            {
                XmlNode a = xml.SelectSingleNode("Holes");
                if (a != null) {

                    XmlNodeList list = a.SelectNodes("Hole");
                    holeList = new List<Hole>();
                    foreach (XmlElement node in list) {
                        holeList.Add(new Hole(node));
                    }
                }
            }



            origin = new Vector2(
            float.Parse(xml.SelectSingleNode("Origin").Attributes["X"].InnerText.Replace(".", ",")),
            float.Parse(xml.SelectSingleNode("Origin").Attributes["Y"].InnerText.Replace(".", ",")));

            padList = new List<Pad>();
            foreach (XmlElement item in xml.SelectNodes("Pads/Pad")) {
                padList.Add(new Pad(item, padStyleList));
            }
            Log.log("Created " + patternStyle + " - " + name);
        }

        public PadPattern copy() {
            return new PadPattern(xml, padStyleList);
        }
    }
    class Hole {
        XmlElement xml;
        int id;
        Vector2 position;
        double diam;
        double holeDiam;
        //Id="1" Locked="N" X="0" Y="0" Diam="5" HoleDiam="3.2"
        public Hole(XmlElement xml_) {
            xml = xml_;
            id = int.Parse(xml.Attributes["Id"].InnerText);
            string x = xml.Attributes["X"].InnerText.Replace(".", ",");
            string y = xml.Attributes["Y"].InnerText.Replace(".", ",");
            position = new Vector2(float.Parse(x), float.Parse(y));
            diam = double.Parse(xml.Attributes["Diam"].InnerText.Replace(".", ","));
            holeDiam = double.Parse(xml.Attributes["HoleDiam"].InnerText.Replace(".", ","));
        }
        public void paint(object sender, PaintEventArgs e, Vector2 pos, double angle, Vector3 panelOffset) {
            Pen pen = new Pen(Color.Black);
            //Vector2 newOffset = Vector2.Transform(offset, Matrix3x2.CreateRotation((float)angle));
            e.Graphics.DrawEllipse(pen,
                panelOffset.X + (pos.X + position.X - (float)diam / 2) * panelOffset.Z,
                panelOffset.Y + ((Panel)sender).Size.Height - (pos.Y + position.Y + (float)diam / 2) * panelOffset.Z,
                (float)diam * panelOffset.Z,
                (float)diam * panelOffset.Z);
            e.Graphics.DrawEllipse(pen,
                panelOffset.X + (pos.X + position.X - (float)holeDiam / 2) * panelOffset.Z,
                panelOffset.Y + ((Panel)sender).Size.Height - (pos.Y + position.Y + (float)holeDiam / 2) * panelOffset.Z,
                (float)holeDiam * panelOffset.Z,
                (float)holeDiam * panelOffset.Z);
        }
        public void fillGrid(Grid grid, Vector2 pos, double angle_) {


            //double totalAngle = angle_;
            //Vector2 newOffset = Vector2.Transform(position, Matrix3x2.CreateRotation((float)angle_));

            //int aaa1 = (int)(Math.Floor((pos.X + newOffset.X) / grid.cellSize));
            //int aaa2 = (int)(Math.Ceiling((pos.Y + newOffset.Y) / grid.cellSize));
            //grid.padGrid[aa1, aa2] = true;





            Log.log(id + " Pad Fill grid");



            Vector2[] vec = {
                    new Vector2( (float)diam/2,  (float)diam/2 ),
                    new Vector2(-(float)diam/2,  (float)diam/2 ),
                    new Vector2(-(float)diam/2, -(float)diam/2 ),
                    new Vector2( (float)diam/2, -(float)diam/2 ),

                    new Vector2( (float)diam/2,  (float)diam/2 )}; //Same as pts[0]

            PointF[] pts = new PointF[5];
            double totalAngle = angle_;
            Vector2 newOffset = Vector2.Transform(position, Matrix3x2.CreateRotation((float)angle_));

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

            for (int k = 0; k < pts.Length - 1; k++) {

                for (int i = 1; i < x - 1; i++) {
                    PointF p3 = new PointF((float)(i * grid.cellSize), 0);
                    PointF p4 = new PointF((float)(i * grid.cellSize), (float)(y * grid.cellSize));

                    PointF pCross;
                    bool cross = Interact.cross(pts[k], pts[k + 1], p4, p3, out pCross);

                    if (cross == true) {
                        int aa1 = (int)(Math.Floor((pCross.X + grid.cellSize / 2) / grid.cellSize));
                        int aa2 = (int)(Math.Ceiling(pCross.Y / grid.cellSize));
                        //points.Add(pCross);
                        if (aa1 < x && aa2 < y && aa1 > 0 && aa2 > 0) {
                            grid.padGrid[aa1, aa2] = true;
                            grid.padGrid[aa1 - 1, aa2] = true;
                        }

                        if ((int)(Math.Ceiling((pCross.Y + eps) / grid.cellSize)) != aa2) {
                            if (aa2 < y && aa2 >= 0) {
                                grid.padGrid[aa1, aa2 + 1] = true;
                                grid.padGrid[aa1 - 1, aa2 + 1] = true;
                            }
                        }
                        if ((int)(Math.Ceiling((pCross.Y - eps) / grid.cellSize)) != aa2) {
                            if (aa2 < y && aa2 >= 0) {
                                grid.padGrid[aa1, aa2 - 1] = true;
                                grid.padGrid[aa1 - 1, aa2 - 1] = true;
                            }
                        }


                    }
                }


                for (int i = 1; i < y - 1; i++) {
                    PointF p1 = new PointF(0, (float)(i * grid.cellSize));
                    PointF p2 = new PointF((float)(x * grid.cellSize), (float)(i * grid.cellSize));

                    PointF pCross;
                    bool cross = Interact.cross(pts[k], pts[k + 1], p1, p2, out pCross);

                    if (cross == true) {
                        int aa1 = (int)(Math.Floor(pCross.X / grid.cellSize));
                        int aa2 = (int)(Math.Ceiling((pCross.Y + grid.cellSize / 2) / grid.cellSize));
                        //points.Add(pCross);
                        if (aa1 < x && aa2 < y && aa1 > 0 && aa2 > 0) {
                            grid.padGrid[aa1, aa2] = true;
                            grid.padGrid[aa1, aa2 - 1] = true;
                        }

                        if ((int)(Math.Floor((pCross.X + eps) / grid.cellSize)) != aa1) {
                            if (aa2 < x && aa2 >= 0) {
                                grid.padGrid[aa1 + 1, aa2] = true;
                                grid.padGrid[aa1 + 1, aa2 - 1] = true;
                            }
                        }
                        if ((int)(Math.Floor((pCross.X - eps) / grid.cellSize)) != aa1) {
                            if (aa2 < x && aa2 >= 0) {
                                grid.padGrid[aa1 - 1, aa2] = true;
                                grid.padGrid[aa1 - 1, aa2 - 1] = true;
                            }
                        }

                    }


                }
            }
        }

    }
}
