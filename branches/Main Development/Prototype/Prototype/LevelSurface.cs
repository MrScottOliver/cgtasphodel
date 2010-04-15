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
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    //// effect.Parameters[""]
                    effect.View = view;
                    effect.Projection = projection;

                    effect.World = /*gameWorldRotation * transforms[mesh.ParentBone.Index]**/    scale * rotation * Matrix.CreateTranslation(Position);  /*scale*/
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
