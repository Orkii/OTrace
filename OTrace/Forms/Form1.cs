using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using OTrace.Properties;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.XPath;
using System.Numerics;
using OTrace.Class;
using System.Reflection;
using OTrace.Class.Trace;
using System.Collections;
using System.Threading;

namespace OTrace {
    public partial class Form1 : Form {

        //static string filePath = "E:\\Диплом\\Plate\\Empty.dipxml";
        static string filePath = "E:\\Диплом\\Plate\\Empty.dipxml";
        List<Component> list = new List<Component>();

        List<PadStyle> padStylesList;
        List<PadPattern> patternList;
        //List<Component> componentList;
        Plate plate;
        //List<BaseComponent> baseComponentLst;

        Algorithm algorithm;

        Vector3 panelOffset;
        public double sizeMultiplyForDraw = 10;
        public Form1() {
            InitializeComponent();
            DoubleBuffered = true;
            typeof(Panel).InvokeMember("DoubleBuffered", // Двойная буферизация, убирает мерцание
                            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, panel1, new object[] { true });

            panelOffset = new Vector3(0,0,10);

            openFile(filePath);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dipxml (*.dipxml)|*.dipxml";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                filePath = openFileDialog.FileName;
                openFile(openFileDialog.FileName);
            }




        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e) {

        }


        private void openFile(string file) {
            
            Log.log("Open File to read data " + file);
            XmlReader reader = XmlReader.Create(filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            //Что грузим
            padStylesList = new List<PadStyle>();
            patternList = new List<PadPattern>();
            List<Component> componentList = new List<Component>();
            Board board;

            {
                Log.log("Try parse padStyle");
                XmlNodeList list = doc.SelectNodes("Source/Library/Library/PadStyles/PadStyle");
                foreach (XmlElement el in list) {
                    padStylesList.Add(new PadStyle(el));
                }
                //Console.WriteLine();
            }
            {
                Log.log("Try parse Patterns");
                XmlNodeList list = doc.SelectNodes("Source/Library/Library/Patterns/Pattern");
                foreach (XmlElement el in list) {
                    patternList.Add(new PadPattern(el, padStylesList));
                }
                //Console.WriteLine();
            }
            {
                Log.log("Try parseComponents");
                XmlNodeList list = doc.SelectNodes("Source/Board/Components/Component");
                foreach (XmlElement el in list) {
                    Component component = new Component(el, patternList);
                    componentList.Add(component);
                    //ComponentTree.Nodes.Add(component.RefDes + " - " + component.name);
                }
                //Console.WriteLine();
            }
            {
                Log.log("Try parseBoard");
                XmlElement list = (XmlElement)doc.SelectSingleNode("Source/Board/BoardOutline");
                board = new Board(list);
            }
            plate = new Plate(componentList, board);
            
            algorithm = new Algorithm(plate);
            algorithm.infoRB = infoRB;
            drawComponentTree();
        }

        private void drawComponentTree() {
            ComponentTree.Nodes.Clear();


            foreach (Component comp in plate.components) {
                ComponentTree.Nodes.Add(comp.drawToTree());
            }

        }
        private void panel1_Paint(object sender, PaintEventArgs e) {
            algorithm.paint(sender, e, panelOffset);
        }


        bool mouseDown = false;
        Point mousePrevPoint;
        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;
            mousePrevPoint = e.Location;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e) {
            mouseDown = false;
        }
        private void panel1_MouseWheel(object sender, MouseEventArgs e) {
            panelOffset.Z += e.Delta / 100;
            if (panelOffset.Z <= 2) panelOffset.Z = 2f;
            else if (panelOffset.Z >= 100) panelOffset.Z = 100f;
            panel1.Invalidate();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e) {
            float slowCoef = 1f;

            if (mouseDown == true) {

                panelOffset = new Vector3(
                    panelOffset.X + (float)(e.Location.X - mousePrevPoint.X) * slowCoef,
                    panelOffset.Y + (float)(e.Location.Y - mousePrevPoint.Y) * slowCoef,
                    panelOffset.Z);
                mousePrevPoint = e.Location;
                panel1.Invalidate();
            }

            //Console.WriteLine(panelOffset);
            //Console.WriteLine(e.Location);
            mousePosL.Text = new PointF(
                (-panelOffset.X +e.Location.X) / panelOffset.Z,
                (panelOffset.Y + ((Panel)sender).Size.Height - e.Location.Y) / panelOffset.Z).ToString();
        }

        private void Form1_Load(object sender, EventArgs e) {
        }

        private void panel1_Resize(object sender, EventArgs e) {
            panel1.Invalidate();
        }

        private void showPlateBox_CheckedChanged(object sender, EventArgs e) {
            if (algorithm != null) algorithm.settings.showPlate = showPlateBox.Checked;
            panel1.Invalidate();
        }

        private void showGridBox_CheckedChanged(object sender, EventArgs e) {
            if (algorithm != null) algorithm.settings.showGrid = showGridBox.Checked;
            panel1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e) {

            Thread th = new Thread(new ThreadStart(() => {
                algorithm.alg((int)(numericUpDown1.Value));
            }));
            th.Start();
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }
    }
}
