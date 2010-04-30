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
using DPSF;
using DPSF.ParticleSystems;
using System.Diagnostics;

namespace Prototype
{
    class Orb : Level
    {
        BoundingSphere sphere;
        Model ObjModel;
        private Vector3 Position;

        public Orb(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            sphere.Center = Pos;//set position
            sphere.Radius = 1;//set radius
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
                    //effect.World = /*gameWorldRotation * */ transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position);

                    Matrix world = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position);
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
                    effect.Parameters["withgrey"].SetValue(false);
                    effect.Parameters["withlights"].SetValue(true);
                    effect.Parameters["withshadow"].SetValue(false);

                    effect.CommitChanges();

                }
                mesh.Draw();
            }

        }
        override
        public void HandleState(Player Player)
        {
            if (Player.boundingsphere.Intersects(sphere))
            {
                ObjectControl.itemsToRemove.Add(this);
                Audio.Pickup();
                Player.OrbCount++;
                ParticleGroup.NewParticle(Position);
               // mcOrbParticleSystem.SetWorldViewProjectionMatrices(World * Matrix.CreateTranslation(this.GetPosition), View, Proj);
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
        public Vector3 GetPosition()
        {
           return Position;
        }
    }
}
