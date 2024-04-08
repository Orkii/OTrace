using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OTrace {
    /// <summary>
    /// How pad is look
    /// </summary>
    internal class PadStyle {
        public string name;//PadT0, PadT1, ...
        public string type;
        public string side;
        public string shape;
        public Vector2 size;

        public XmlElement xml;


        public PadStyle(XmlElement xml_) {
            xml = xml_;

            //RefDes = xml.SelectSingleNode("RefDes").InnerText;
            //string x = xml.GetAttribute("X").Replace(".", ",");
            //string y = xml.GetAttribute("Y").Replace(".", ",");
            //position = new Vector2(float.Parse(x), float.Parse(y));


            name = xml.Attributes["Name"].InnerText;
            type = xml.Attributes["Type"].InnerText;
            side = xml.Attributes["Side"].InnerText;
            shape = xml.SelectSingleNode("MainStack").Attributes["Shape"].InnerText;
            size = new Vector2(
                float.Parse(xml.SelectSingleNode("MainStack").Attributes["Width"].InnerText.Replace(".", ",")),
                float.Parse(xml.SelectSingleNode("MainStack").Attributes["Height"].InnerText.Replace(".", ",")));
        }

        public override string ToString() {
            return name;
        }
    }
}
