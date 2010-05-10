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
    class Surface
    {
        Model ObjModel;
        private Vector3 Position;
        Matrix scale, rotation;

        public Surface(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            scale = Matrix.Identity; ;
            rotation = Matrix.Identity;
        }

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

                    ////effect.World = /*gameWorldRotation * transforms[mesh.ParentBone.Index]**/    scale *rotation  * Matrix.CreateTranslation(Position);  /*scale*/
                    //effect.World = transforms[mesh.ParentBone.Index] * rotation * Matrix.CreateTranslation(Position);


                    Matrix world = transforms[mesh.ParentBone.Index] * rotation * Matrix.CreateTranslation(Position);
                    Matrix wvp = world * view * projection;
                    Matrix vp = view * projection;

                   // Matrix wv = world * view;

                   // Matrix wvIT = Matrix.Invert(wv);
                   // wvIT = Matrix.Transpose(wvIT);

                    Matrix worldIT = Matrix.Invert(world);
                    worldIT = Matrix.Transpose(worldIT);

                    Vector4 ambMtrl = new Vector4(0.0f, 0.9f, 0.0f, 1.0f);//Jess: default material vals
                    Vector4 diffMtrl = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                    Vector4 specMtrl = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

                    //set matrix params
                    effect.Parameters["gWVP"].SetValue(wvp);
                  //  effect.Parameters["gView"].SetValue(view);
                  //  effect.Parameters["gProj"].SetValue(projection);
                    effect.Parameters["gWorld"].SetValue(world);

              
                   // effect.Parameters["gWorldView"].SetValue(wv);
                    //effect.Parameters["gWorldViewIT"].SetValue(wvIT);
                    effect.Parameters["gWorldIT"].SetValue(worldIT);

                    effect.Parameters["gViewProj"].SetValue(vp);
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
        public void RePosition(Vector3 Pos) 
        {
            Position = Pos;
        }

        public void Move(float x, float y, float z)
        {
            Position.X += x;
            Position.Y += y;
            Position.Z += z;
        }

        public void Collision(BoundingSphere PlayerSphere)
        {
        }

        public void Scale(float x, float y, float z)
        {
            scale = Matrix.CreateScale(x, y, z);
        }

        public void Rotate(float x, float y, float z)  //uses radians
        {
            rotation = Matrix.CreateFromYawPitchRoll((float)Math.PI * x / 2, (float)Math.PI * y / 2, (float)Math.PI * z / 2);
        }
    }
}
