﻿using System;
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
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            Matrix[] transforms = new Matrix[ObjModel.Bones.Count];
            ObjModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in ObjModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    ////// effect.Parameters[""]
                    //effect.View = view;
                    //effect.Projection = projection;
                    Matrix scale;
                    scale = Matrix.Identity; ;
                    scale = Matrix.CreateScale(Scale);
                    //effect.World = /*gameWorldRotation * */   transforms[mesh.ParentBone.Index] * scale * Matrix.CreateTranslation(Position); /*scale*/


                    Matrix world = /*gameWorldRotation * */   transforms[mesh.ParentBone.Index] * scale * Matrix.CreateTranslation(Position); /*scale*/
                    Matrix wvp = world * view * projection;
                    Matrix vp = view * projection;

           
                    Matrix worldIT = Matrix.Invert(world);
                    worldIT = Matrix.Transpose(worldIT);

                    Vector4 ambMtrl = new Vector4(0.9f, 0.0f, 0.0f, 1.0f);//Jess: default material vals
                    Vector4 diffMtrl = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                    Vector4 specMtrl = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

                    //set matrix params
                    effect.Parameters["gWVP"].SetValue(wvp);
                    effect.Parameters["gWorld"].SetValue(world);
                    effect.Parameters["gViewProj"].SetValue(vp);

                    effect.Parameters["gWorldIT"].SetValue(worldIT);

                    effect.Parameters["gAmbMtrl"].SetValue(ambMtrl);
                    effect.Parameters["gDiffuseMtrl"].SetValue(diffMtrl);
                    effect.Parameters["gSpecMtrl"].SetValue(specMtrl);
                    effect.Parameters["withgrey"].SetValue(true);
                    effect.Parameters["withlights"].SetValue(true);
                    effect.Parameters["player"].SetValue(false);
                    effect.Parameters["withshadow"].SetValue(true);


                }
                mesh.Draw();
            }
        }
        override
        public void HandleState(Player Player)
        {
            switch (Current)
            {
                case LifeCycle.Active:
                    if (Player.boundingsphere.Intersects(sphere))
                    {
                        if (Player.velocity.Y < 0)
                        {
                            Player.velocity.Y = 0;
                            Player.velocity.Y += 0.5f;
                            Current = LifeCycle.AnimateDown;
                        } 
                    }
                    break;
                case LifeCycle.AnimateDown:
                    if (Scale.Y > 0.01)
                    {
                        Scale.X += 0.02f;
                        Scale.Y -= 0.02f;
                    }
                    else
                        Current = LifeCycle.AnimateUp;
                   //check time, stretch the shroom to suit
                    //once complete return to active
                    break;
                case LifeCycle.AnimateUp:
                    if (Scale.X > 0.25)
                    {
                        Scale.X -= 0.02f;
                        Scale.Y += 0.02f;
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
    }
}
