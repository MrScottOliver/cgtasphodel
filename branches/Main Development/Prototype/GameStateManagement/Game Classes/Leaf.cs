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

namespace GameStateManagement
{
    class Leaf : Level
    {
        public enum LifeCycle
        {
            Seedling,
            Growing,
            FullyGrown
        }

        Model ObjModel;
        private Vector3 Position, Scale;
        float ZRotate;
        public LifeCycle Current;

        
        public Leaf(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            Current = LifeCycle.Seedling;
            Scale.X = Scale.Y = Scale.Z = 0.01f;
            ZRotate = (float)Math.PI / 2;
        }

        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            if (Current != LifeCycle.Seedling)
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
                        Matrix scale, rotate;
                        scale = Matrix.Identity; ;
                        scale = Matrix.CreateScale(Scale);
                        rotate = Matrix.Identity; ;
                        rotate = Matrix.CreateRotationZ(ZRotate) * Matrix.CreateRotationY((float)Math.PI/2);
                        //effect.World = /*gameWorldRotation * */   transforms[mesh.ParentBone.Index] * rotate * scale * Matrix.CreateTranslation(Position); /*scale*/

                        Matrix world = /*gameWorldRotation * */   transforms[mesh.ParentBone.Index] * rotate * scale * Matrix.CreateTranslation(Position); /*scale*/
                        Matrix wvp = world * view * projection;
                        Matrix vp = view * projection;

                        Vector4 ambMtrl = new Vector4(1.0f, 1.0f, 0.2f, 1.0f);//Jess: default material vals
                        Vector4 diffMtrl = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                        Vector4 specMtrl = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

                        //set matrix params
                        effect.Parameters["gWVP"].SetValue(wvp);
                        effect.Parameters["gWorld"].SetValue(world);
                        effect.Parameters["gViewProj"].SetValue(vp);
                        effect.Parameters["gAmbMtrl"].SetValue(ambMtrl);
                        effect.Parameters["gDiffuseMtrl"].SetValue(diffMtrl);
                        effect.Parameters["gSpecMtrl"].SetValue(specMtrl);
                        effect.Parameters["withgrey"].SetValue(true);
                        effect.Parameters["withlights"].SetValue(true);
                        effect.Parameters["withshadow"].SetValue(true);

                        effect.CommitChanges();
                    }
                    mesh.Draw();
                }
            }
        }

        override
        public void HandleState(Player Player)
        {
            switch (Current)
            {
                case LifeCycle.Seedling:                   
                       // if (true)
                        //{
                        //    Current = LifeCycle.Growing;
                        //} 
                    break;

                case LifeCycle.Growing:
                    if (Scale.X<0.25)
                    {
                        Scale.X+=0.002f;
                        Scale.Y+=0.002f;
                        Scale.Z+=0.002f;
                        ZRotate -= 0.014f;
                    }
                    else
                    {
                        Current = LifeCycle.FullyGrown;
                        CollisionDetectionBox.AddBox(new Vector3(Position.X, Position.Y, Position.Z - 10), new Vector3(Position.X + 4, Position.Y + 2, Position.Z));
                    }
                    break;

                case LifeCycle.FullyGrown:
                    break;

                default:
                    throw new ArgumentException("Error - " + Current + " is not recognized.");
            }
        }
    }
}
