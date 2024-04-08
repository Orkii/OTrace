using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTrace.Class.Trace {
    internal class Net {
        public int id;
        public List<Pad> pads;
        public List<Point> routePoints; 



        public Net(int id_) {
            pads = new List<Pad>();
            id = id_;
            routePoints = new List<Point>();
        }
    }
}
