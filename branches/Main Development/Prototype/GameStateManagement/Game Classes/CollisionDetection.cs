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

namespace GameStateManagement
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
            foreach (PlaneObject planeX in BObjects)
            {
                if (playerObject.position.X > planeX.MinX && playerObject.position.X < planeX.MaxX)
                {
                    playerObject.boundingsphere.Intersects(ref planeX.p, out planeX.result);

                    if (planeX.result == PlaneIntersectionType.Intersecting)
                    {
                        if ((playerObject.top.Intersects(planeX.p) <= 2))
                        {
                            if (playerObject.velocity.Y > 0)
                            {
                                playerObject.velocity.Y -= playerObject.velocity.Y;
                                playerObject.AddTranslation(0, (float)(-1 * (2 - playerObject.top.Intersects(planeX.p))), 0);
                            }
                        }
                        if ((playerObject.bottom.Intersects(planeX.p) <= 1))
                        {
                            if (playerObject.velocity.Y < 0)
                            {
                                playerObject.velocity.Y -= playerObject.velocity.Y;
                                playerObject.jumpState = false;
                                playerObject.doublejumpState = false;
                                playerObject.AddTranslation(0, (float)(1 - playerObject.bottom.Intersects(planeX.p)), 0);
                            }
                        }
                        else
                        {
                            if (playerObject.bottom.Intersects(planeX.p) >= 1 && (playerObject.velocity.X != 0) && (playerObject.jumpState == false))
                            {
                                playerObject.AddTranslation(0, (float)(-1 * (playerObject.bottom.Intersects(planeX.p) - 1)), 0);
                            }
                        }
                        if ((playerObject.front.Intersects(planeX.p) <= 0.5f))
                        {
                            if (playerObject.velocity.X > 0)
                            {
                                playerObject.velocity.X -= playerObject.velocity.X;
                                playerObject.AddTranslation((float)(-1 * (1 - playerObject.front.Intersects(planeX.p))), 0, 0);
                            }
                        }
                        if ((playerObject.back.Intersects(planeX.p) <= 0.5f))
                        {
                            if (playerObject.velocity.X < 0)
                            {
                                playerObject.velocity.X -= playerObject.velocity.X;
                                playerObject.AddTranslation((float)(1 - playerObject.back.Intersects(planeX.p)), 0, 0);
                            }
                        }
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
                    if ((playerObject.top.Intersects(box) <= 2))
                    {
                        if (playerObject.velocity.Y > 0)
                        {
                            playerObject.velocity.Y -= playerObject.velocity.Y;
                            playerObject.AddTranslation(0, (float)(-1 * (2 - playerObject.top.Intersects(box))), 0);
                        }
                    }
                    if ((playerObject.bottom.Intersects(box) <= 1))
                    {
                        if (playerObject.velocity.Y < 0)
                        {
                            playerObject.velocity.Y -= playerObject.velocity.Y;
                            playerObject.jumpState = false;
                            playerObject.doublejumpState = false;
                            playerObject.AddTranslation(0, (float)(1 - playerObject.bottom.Intersects(box)), 0);
                        }
                    }
                    if ((playerObject.front.Intersects(box) <= 0.5f))
                    {
                        if (playerObject.velocity.X > 0)
                        {
                            playerObject.velocity.X -= playerObject.velocity.X;
                            playerObject.AddTranslation((float)(-1 * (1 - playerObject.front.Intersects(box))), 0, 0);
                        }
                    }
                    if ((playerObject.back.Intersects(box) <= 0.5f))
                    {
                        if (playerObject.velocity.X < 0)
                        {
                            playerObject.velocity.X -= playerObject.velocity.X;
                            playerObject.AddTranslation((float)(1 - playerObject.back.Intersects(box)), 0, 0);
                        }
                    }
                }
            }
        }
    }
}
