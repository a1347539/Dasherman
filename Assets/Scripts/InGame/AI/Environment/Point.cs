using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int _x, int _y) { x = _x; y = _y; }

        public static Point worldToPoint(Vector2 pos)
        {
            return new Point(
                (int)((pos.x - MapController.Instance.playableAreaOrigin.x) / MapController.Instance.cellSize),
                (int)((MapController.Instance.playableAreaOrigin.y - pos.y) / MapController.Instance.cellSize)
                );
        }

        public static byte[] serialize(object obj)
        {
            Point p = (Point)obj;
            return new byte[] { (byte)p.x, (byte)p.y };
        }

        public static object deserialize(byte[] data)
        {
            Point p = new Point(data[0], data[1]);
            return p;
        }

        public void print()
        {
            Debug.Log(x.ToString() + " " + y.ToString());
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public override int GetHashCode()
        {
            int hash = 269;
            hash = (hash * 47) + x.GetHashCode();
            hash = (hash * 47) + y.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            Point other = (Point)obj;
            return other.x == x && other.y == y;
        }
    }
}