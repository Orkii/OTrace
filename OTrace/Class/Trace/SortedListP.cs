using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTrace.Class.Trace {
    /// <summary>
    /// Список сортирующий клеточки по какой-то метрике
    /// </summary>
    internal class SortedListP {
        public List<PointWithDist> pointsD;

        public List<Point> destination;

        public Dictionary<Point, PointWithDist> alreadyUsed;

        public SortedListP(List<Point> destination_) {
            destination = destination_;
            List<Point> points = new List<Point>();

            alreadyUsed = new Dictionary<Point, PointWithDist>();
            pointsD = new List<PointWithDist>();
        }
        /// <summary>
        /// point - Координаты.
        /// routeLeng - Расстояние до клетки.
        /// pointWithDist_ - Предидущая клетка.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="routeLeng"></param>
        /// <param name="pointWithDist_"></param>
        public void add(Point point, int routeLeng, PointWithDist pointWithDist_ = null) { 
            int distance = Math.Abs(destination[0].X - point.X) + Math.Abs(destination[0].Y - point.Y);
            PointWithDist p = new PointWithDist(point, distance, routeLeng, pointWithDist_);
            insert(p);

              
        }

        public void remove(Point point) {
            alreadyUsed.Remove(point);
            for (int i = 0; i < pointsD.Count; i++){
                if (pointsD[i].point == point) {
                    pointsD.RemoveAt(i);
                    return;
                    //////// Надо проверить и имплементировать
                }
            }
        }

        void insert(PointWithDist p) {
            if (pointsD.Count == 0) {
                pointsD.Add(p);
            }

            if (alreadyUsed.ContainsKey(p.point) == true) {
                return;
            }

            //foreach (Point a in alreadyUsed.Keys) {
            //    if ((a.X == p.point.X) && (a.Y == p.point.Y)) {
            //        return;
            //    }
            //}
            if (alreadyUsed.Count== 1) {
                Console.WriteLine();
            }
            pointsD.Add(p);
            alreadyUsed.Add(p.point, p);
            /*
            for (int i = 0; i < pointsD.Count; i++) {
                if (pointsD[i].distance > p.distance) {
                    pointsD.Insert(i, p);
                    alreadyUsed.Add(p.point, p.distance);
                    return;///////
                }
            }*/

            //alreadyUsed.Add(p.point, p.distance);
            //pointsD.Add(p);
        }

        public Point pick() {
            Point a = pointsD[0].point;
            pointsD.RemoveAt(0);
            return a;
        }
        /// <summary>
        /// Возвращает лучшую клетку
        /// </summary>
        /// <returns></returns>
        public PointWithDist pickD() {
            if (pointsD.Count == 0) return null;
            PointWithDist a = pointsD[0];
            pointsD.RemoveAt(0);
            return a;
        }

    }

    /// <summary>
    /// Описывает клетку во время нахлждения пути
    /// </summary>
    class PointWithDist {
        public PointWithDist prevPoint;
        public PointWithDist(Point point_, int distance_, int routeLeng_, PointWithDist prevPoint_ = null) {
            point = point_;
            distance = distance_;
            routeLeng = routeLeng_;
            prevPoint = prevPoint_;
        }
        public Point point;
        public int distance;
        public int routeLeng;
    }
}
