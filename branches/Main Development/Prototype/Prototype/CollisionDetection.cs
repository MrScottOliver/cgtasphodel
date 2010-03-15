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
    static class CollisionDetectionPlane
    {
        static List<PlaneObject> BObjects;

        static CollisionDetectionPlane()
        {
            BObjects = new List<PlaneObject>();
        }

        static public void AddPlane(Vector3 x, Vector3 y, Vector3 z, float min, float max)
        {
            BObjects.Add(new PlaneObject(x, y, z, min, max));
        }

        static public void Compare(ref Player playerObject)
        {
            int result;

            foreach (PlaneObject planeX in BObjects)
            {
                if (playerObject.position.X > planeX.MinX && playerObject.position.X < planeX.MaxX)
                {
                    playerObject.boundingsphere.Intersects(ref planeX.p, out planeX.result);
                    //result = planeX.Position();
                    /* if (result == 1)
                    {
                        playerObject.velocity = Vector3.Zero;
                        playerObject.position += new Vector3(0, 0.001f, 0);
                    }
                    if (result == -1)
                    {
                        playerObject.velocity.Normalize();
                        playerObject.velocity *= -1;
                    }
                    if (result == 0)
                    {
                        playerObject.velocity = Vector3.Zero;
                    }*/
                    if (planeX.result == PlaneIntersectionType.Intersecting)
                    {
                        playerObject.velocity = Vector3.Zero;
                    }
                }
            }
        }
    }

    static class CollisionDetectionBox
    {
        static List<BoundingBox> Blist;

        static CollisionDetectionBox()
        {
            Blist = new List<BoundingBox>();
        }

        public static void AddBox(Vector3 min, Vector3 max)
        {
            Blist.Add(new BoundingBox(min, max));
        }

        public static void Compare(ref Player playerObject)
        {
            foreach (BoundingBox box in Blist)
            {
                if (playerObject.boundingsphere.Intersects(box))
                {
                    if ((playerObject.top.Intersects(box) != 0))
                    {
                        playerObject.velocity.Y = 0;
                    }
                    if ((playerObject.bottom.Intersects(box) != 0))
                    {
                        playerObject.velocity.Y = 0;
                    }
                    if ((playerObject.front.Intersects(box) != 0))
                    {
                        playerObject.velocity.X = 0;
                    }
                    if ((playerObject.back.Intersects(box) != 0))
                    {
                        playerObject.velocity.X = 0;
                    }
                }
            }
        }
    }
}
