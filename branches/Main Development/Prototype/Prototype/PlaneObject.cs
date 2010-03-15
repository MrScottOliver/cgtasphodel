using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Prototype
{
    class PlaneObject
    {
        public Plane p;
        public float MaxX { get; set; }
        public float MinX { get; set; }
        public PlaneIntersectionType prev { get; set; }
        public PlaneIntersectionType next;
        //public int result { get; set; }
        public PlaneIntersectionType result;

        public PlaneObject()
        {
            MaxX = MinX = 0.0f;
            p = new Plane();
        }

        public PlaneObject(Vector3 x, Vector3 y, Vector3 z, float min, float max)
        {
            p = new Plane(x, y, z);
            MaxX = max;
            MinX = min;
        }

        public int Position()
        {
            if (prev == PlaneIntersectionType.Back && next == PlaneIntersectionType.Intersecting)
            {
                prev = next;
                return -1;
            }

            if (prev == PlaneIntersectionType.Front && next == PlaneIntersectionType.Intersecting)
            {
                prev = next;
                return 1;
            }

            prev = next;
            return 0;
        }
    }
}
