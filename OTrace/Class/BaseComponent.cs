using OTrace.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OTrace {
    internal class BaseComponent {
        public string name;
        public string partName;
        public Vector2 origin;
        public List<Pin> pinList;
        public PadPattern padPattern;

        public XmlElement xml;
        public BaseComponent(XmlElement xml_, List<PadPattern> padPatternList) {
            xml = xml_;
            name = xml.SelectSingleNode("Name").InnerText;
            partName = xml.SelectSingleNode("PartName").InnerText;
            string x = xml.SelectSingleNode("Origin").Attributes["X"].InnerText.Replace(".", ",");
            string y = xml.SelectSingleNode("Origin").Attributes["Y"].InnerText.Replace(".", ",");
            origin = new Vector2(float.Parse(x), float.Parse(y));

            

            string padP = xml.SelectSingleNode("Pattern").Attributes["Style"].InnerText;
            foreach (PadPattern p in padPatternList) {
                if (p.patternStyle == padP) {
                    padPattern = p.copy();
                    break;
                }
            }


            pinList = new List<Pin>();
            XmlNodeList list = xml.SelectNodes("Pins/Pin");
            foreach (XmlElement el in list) {
                pinList.Add(new Pin(el));
            }
        }
        public TreeNode drawToTree() {
            TreeNode node = new TreeNode(name + " - " + partName);

            node.Nodes.Add("Origin: " + origin);
            node.Nodes.Add("PadPattern: " + padPattern.patternStyle);

            foreach (Pin a in pinList) {
                node.Nodes.Add(a.drawToTree());
            }

            return node;

        }

        public void paint(object sender, PaintEventArgs e, double sizeMultiplyForDraw, Vector2 position) {
            //foreach (Pad a in padPattern.padList) {
            //    a.paint(sender, e, sizeMultiplyForDraw, position);
            //}
            foreach (Pin a in pinList) {
                a.paint(sender, e, sizeMultiplyForDraw, position);
            }


        }
    }
}
