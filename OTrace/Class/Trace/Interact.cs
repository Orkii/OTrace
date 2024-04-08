using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OTrace.Class.Trace {
    class Interact {
        // Метод вычисления точки пересечения.
        private static PointF Cross(double a1, double b1, double c1, double a2, double b2, double c2) {
            /*
			 решение
             x = (b1c2 - c1b2) / (a1b2 - a2b1)
             y = (a2c1 - c2a1) / (a1b2 - a2b1)
			*/
            PointF pCross = new PointF();
            pCross.X = (float)((b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1));
            pCross.Y = (float)((a2 * c1 - a1 * c2) / (a1 * b2 - a2 * b1));

            return pCross;
        }

        public static bool cross(PointF p1, PointF p2, PointF d1, PointF d2, out PointF pCross) {
            pCross = new PointF(float.MinValue, float.MinValue);
            float eps = 0.00000000000000000001f;
            float x = float.MinValue;
            float y = float.MinValue;

            bool a = false;

            if (Math.Abs(p2.Y - p1.Y) < eps) { // Первая линия Паралельна оси X
                y = (p2.Y + p1.Y) / 2;
                if (Math.Abs(d2.Y - d1.Y) < eps) { // Вторая линия Паралельна оси X
                    return false;
                }
            }
            if (Math.Abs(d2.Y - d1.Y) < eps) { // Вторая линия Паралельна оси X
                y = (d2.Y + d1.Y) / 2;
                a = true;
            }

            if (Math.Abs(p2.X - p1.X) < eps) { // Первая линия Паралельна оси Y
                x = (p2.X + p1.X) / 2;
                if (Math.Abs(d2.X - d1.X) < eps) { // Вторая линия Паралельна оси Y
                    return false;
                }
            }
            if (Math.Abs(d2.X - d1.X) < eps) { // Вторая линия Паралельна оси Y
                x = (d2.X + d1.X) / 2;
                a = true;
            }

            if ((x != float.MinValue) && (y != float.MinValue)) {
                if (x - eps > Math.Max(p1.X, p2.X)) { return false; }
                if (y - eps > Math.Max(p1.Y, p2.Y)) { return false; }
                if (x + eps < Math.Min(p1.X, p2.X)) { return false; }
                if (y + eps < Math.Min(p1.Y, p2.Y)) { return false; }
                pCross = new PointF(x, y);
                return true;
            }

            if ((x != float.MinValue) || (y != float.MinValue)) {
                if ((a == true)) { // Вторая линия паралелна оси
                    float k1 = (p2.Y - p1.Y) / (p2.X - p1.X);
                    float b1 = p1.Y - p1.X * k1;

                    if (x == float.MinValue) {
                        x = (y - b1) / k1;
                    }
                    else {
                        y = k1 * x + b1;
                    }
                }
                else {// Первая линия паралелна оси
                    float k2 = (d2.Y - d1.Y) / (d2.X - d1.X);
                    float b2 = d1.Y - d1.X * k2;

                    if (x == float.MinValue) {
                        x = (y - b2) / k2;
                    }
                    else {
                        y = k2 * x + b2;
                    }
                }
                //pCross = new PointF(x, y);

                if (x - eps > Math.Max(p1.X, p2.X)) { return false; }
                if (y - eps > Math.Max(p1.Y, p2.Y)) { return false; }
                if (x + eps < Math.Min(p1.X, p2.X)) { return false; }
                if (y + eps < Math.Min(p1.Y, p2.Y)) { return false; }
                pCross = new PointF(x, y);

                return true;
            }



            /*
            if (Math.Abs(p2.Y - p1.Y) < eps) { // Первая линия Паралельна оси X
                y = p2.Y + p1.Y / 2;
                if (Math.Abs(d2.Y - d1.Y) < eps) { // Вторая линия Паралельна оси X
                    return false;
                }
                else {
                    float k2 = (d2.Y - d1.Y) / (d2.X - d1.X);
                    float b2 = d1.Y - d1.X * k2;
                    x = (y - b2) / k2;
                }
            }
            else if (Math.Abs(d2.Y - d1.Y) < eps) {// Вторая линия Паралельна оси X
                y = d2.Y + d1.Y / 2;
                float k1 = (p2.Y - p1.Y) / (p2.X - p1.X);
                float b1 = p1.Y - p1.X * k1;
                x = (y - b1) / k1;

            }
            else{


                if (float.IsNaN(x) || float.IsNaN(y)) return false;
            }
            */

            {
                float k1 = (p2.Y - p1.Y) / (p2.X - p1.X);
                float b1 = p1.Y - p1.X * k1;

                float k2 = (d2.Y - d1.Y) / (d2.X - d1.X);
                float b2 = d1.Y - d1.X * k2;



                x = (b2 - b1) / (k1 - k2);
                y = k1 * x + b1;
            }
            if (x - eps > Math.Max(p1.X, p2.X)) {
                return false;
            }
            if (y - eps > Math.Max(p1.Y, p2.Y)) {
                return false;
            }
            if (x + eps < Math.Min(p1.X, p2.X)) {
                return false;
            }
            if (y + eps < Math.Min(p1.Y, p2.Y)) {
                return false;
            }
            /*
            if (x > Math.Max(Math.Max(p1.X, p2.X), Math.Max(d1.X, d2.X))) {
                return false;
            } // Точка за пределами 
            if (y > Math.Max(Math.Max(p1.Y, p2.Y), Math.Max(d1.Y, d2.Y))) {
                return false;
            }
            if (x < Math.Min(Math.Min(p1.X, p2.X), Math.Min(d1.X, d2.X))) {
                return false;
            }
            if (y < Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(d1.Y, d2.Y))) {
                return false;
            }
            */

            pCross = new PointF(x, y);

            return true;
        }

    }



}

