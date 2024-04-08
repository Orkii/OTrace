using OTrace.Class;
using OTrace.Class.Trace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OTrace {
    internal class Component {
        public string name;
        public string RefDes;

        public PadPattern patternStyle;
        public Vector2 position;
        public XmlElement xml;
        public string xmlCode;
        public double angle;


        public Component(XmlElement xml_, List<PadPattern> padPatterns/*, List<BaseComponent> baseComponentList_*/) {
            xml = xml_;
            //baseComponentList = baseComponentList_;

            name = xml.SelectSingleNode("Name").InnerText;
            RefDes = xml.SelectSingleNode("RefDes").InnerText;
            string x = xml.GetAttribute("X").Replace(".", ",");
            string y = xml.GetAttribute("Y").Replace(".", ",");
            position = new Vector2(float.Parse(x), float.Parse(y));

            string aa = xml.GetAttribute("Angle");
            if (aa!= "") {
                angle = double.Parse(aa.Replace(".", ","));
            }
            

            string padP = xml.Attributes["PatternStyle"].InnerText;
            foreach (PadPattern p in padPatterns) {
                if (p.patternStyle == padP) {
                    patternStyle = p.copy();
                    break;
                }
            }

            foreach (XmlElement p in xml.SelectNodes("Pads/Pad")) {
                int i = int.Parse(p.Attributes["Id"].InnerText);

                foreach (Pad a in patternStyle.padList) {
                    if (i == a.id) {
                        a.netId = int.Parse(p.Attributes["NetId"].InnerText);
                    }
                }

            }


            /*foreach (BaseComponent a in baseComponentList) {
                if (name == a.name) {
                    baseComponent = a;
                    break;
                }
            }*/
            //if (baseComponent == null) { throw new MException(name);  }
            Log.log("Created " + RefDes + " - " + name);
        }
        public override string ToString() {
            return name;
        }
        public TreeNode drawToTree() {
            TreeNode node = new TreeNode(RefDes + " - " + name);

            node.Nodes.Add("Position: " + position);
            node.Nodes.Add("PatternStyle: " + patternStyle.patternStyle);
            foreach (Pad a in patternStyle.padList) {
                node.Nodes.Add(a.drawToTree());
            }

            return node;
        }
        public void paint(object sender, PaintEventArgs e, Vector3 panelOffset) {
            foreach(Pad a in patternStyle.padList) {
                a.paint(sender, e, position, angle, panelOffset);
            }
            if (patternStyle.holeList != null) {
                foreach (Hole a in patternStyle.holeList) {
                    a.paint(sender, e, position, angle, panelOffset);
                }
            }


        }

        public void fillGrid(Grid grid) {
            Log.log(name + " Component Fill grid");
            foreach (Pad a in patternStyle.padList) {
                
                a.fillGrid(grid, position, angle);
            }
            if (patternStyle.holeList != null) {
                foreach (Hole a in patternStyle.holeList) {
                    a.fillGrid(grid, position, angle);
                }
            }
        }

    }
}
