using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTrace.Class.Trace {
    internal class Plate {

        public List<Component> components;
        public Board board;


        public Plate(List<Component> components_, Board board_) {
            components = components_;
            board = board_;
        }

        public void paint(object sender, PaintEventArgs e, Vector3 panelOffset) {
            foreach (Component component in components) {
                component.paint(sender, e, panelOffset);
            }
            board.paint(sender, e, panelOffset);
        }
    }
}
