using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OTrace {
    internal class Pin {
        int padId;
        public string name;
        public Vector2 position;
        public double orientation;
        public double length;


        public XmlElement xml;

        public Pin(XmlElement xml_) {
            xml = xml_;

            name = xml_.SelectSingleNode("Name").InnerText;
            string x = xml.Attributes["X"].InnerText.Replace(".", ",");
            string y = xml.Attributes["Y"].InnerText.Replace(".", ",");
            position = new Vector2(float.Parse(x), float.Parse(y));
            orientation = float.Parse(xml.Attributes["Orientation"].InnerText.Replace(".", ","));
            length = float.Parse(xml.Attributes["Length"].InnerText.Replace(".", ","));

        }

        public TreeNode drawToTree() {
            TreeNode node = new TreeNode(name);

            node.Nodes.Add("Position: " + position);
            node.Nodes.Add("orientation: " + orientation);
            node.Nodes.Add("length: " + length);
            return node;
        }

        public void paint(object sender, PaintEventArgs e, double sizeMultiplyForDraw, Vector2 pos) {
            /*
            Pen pen = new Pen(Color.Black);

            float m = (float)sizeMultiplyForDraw;


            e.Graphics.DrawEllipse(pen,
                400 + (pos.X + position.X) * m,
                400 + -(pos.Y + position.Y) * m,
                1 * m,
                1 * m);
            */
        }
    }
}
