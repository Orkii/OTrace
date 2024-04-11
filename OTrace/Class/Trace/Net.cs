using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTrace.Class.Trace {
    internal class Net : IComparable<Net> {
        public int id;
        public List<Pad> pads;
        public List<Point> routePoints;
        public int totalLength { get => routePoints.Count(); }

        public double routeWidth = 0.2;


        public Net(int id_) {
            pads = new List<Pad>();
            id = id_;
            routePoints = new List<Point>();
        }


        public int CompareTo(Net other) {

            if (this.totalLength > other.totalLength) return 1;
            if (this.totalLength < other.totalLength) return -1;
            return 0;

        }


    }
}
