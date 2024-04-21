using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OTrace.Class {
    /// <summary>
    /// Плата как её размеры
    /// </summary>
    internal class Board {
        public Vector2 size;
        XmlElement xml;
        /// <summary>
        /// Границы платы
        /// </summary>
        List<PointF> points;
        public Board(XmlElement xml_) {
            xml = xml_;

            points = new List<PointF>();

            XmlNodeList list = xml.SelectNodes("Points/Point");

            foreach (XmlElement el in list) {
                points.Add(new PointF(
                    float.Parse(el.Attributes["X"].InnerText.Replace(".", ",")),
                    float.Parse(el.Attributes["Y"].InnerText.Replace(".", ",")
                    )));
            }

            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (PointF p in points) {
                if (p.X > maxX) maxX = p.X;
                if (p.X < minX) minX = p.X;
                if (p.Y > maxY) maxY = p.Y;
                if (p.Y < minY) minY = p.Y;
                
            }
            size = new Vector2((float)(maxX - minX), (float)(maxY - minY));
        }
        public void paint(object sender, PaintEventArgs e, Vector3 panelOffset) {
            List<PointF> pointsToDraw = new List<PointF>();
            Pen pen = new Pen(Color.Black);

            foreach (PointF el in points) {
                pointsToDraw.Add(new PointF(
                    panelOffset.X +                               el.X * panelOffset.Z,
                    panelOffset.Y + ((Panel)sender).Size.Height - el.Y * panelOffset.Z

                    ));

            }
            e.Graphics.DrawPolygon(pen, pointsToDraw.ToArray());


        }
    }
}
