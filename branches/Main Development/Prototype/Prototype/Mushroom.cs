using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;

namespace Prototype
{
    class Mushroom : Level
    {
        public enum LifeCycle
        {
            Active,
            AnimateDown,
            AnimateUp
        }

        BoundingSphere sphere;
        Model ObjModel;
        private Vector3 Position, Scale;
        LifeCycle Current;
   

        public Mushroom(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            sphere.Center = Pos;//set position
            sphere.Radius = 5;//set radius
            Current = LifeCycle.Active;
            Scale.X = Scale.Y = Scale.Z = 0.25f;
        }

        override
        public void Load()
        {
          
        }

        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            Matrix[] transforms = new Matrix[ObjModel.Bones.Count];
            ObjModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in ObjModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    //// effect.Parameters[""]
                    effect.View = view;
                    effect.Projection = projection;
                    Matrix scale;
                    scale = Matrix.Identity; ;
                    scale = Matrix.CreateScale(Scale);
                    effect.World = /*gameWorldRotation * */   transforms[mesh.ParentBone.Index] * scale * Matrix.CreateTranslation(Position); /*scale*/
                }
                mesh.Draw();
            }
        }
        override
        public void Collision(BoundingSphere PlayerSphere)
        {
            switch (Current)
            {
                case LifeCycle.Active:
                    if (PlayerSphere.Intersects(sphere))
                        Current = LifeCycle.AnimateDown;
                    //Check collision
                    //if true, boost player
                    //set state animate
                    break;
                case LifeCycle.AnimateDown:
                    if (Scale.Y > 0.01)
                    {
                        Scale.X += 0.01f;
                        Scale.Y -= 0.01f;
                    }
                    else
                        Current = LifeCycle.AnimateUp;
                   //check time, stretch the shroom to suit
                    //once complete return to active
                    break;
                case LifeCycle.AnimateUp:
                    if (Scale.X > 0.25)
                    {
                        Scale.X -= 0.01f;
                        Scale.Y += 0.01f;
                    }
                    else
                        Current = LifeCycle.Active;
                    //check time, stretch the shroom to suit
                    //once complete return to active
                    break;
                default:
                    throw new ArgumentException("Error - " + Current + " is not recognized.");
            }
        }

        override
        public void Activate()
        {

        }
    }
}
