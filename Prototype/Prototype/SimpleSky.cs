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
    class SimpleSky
    {
        public Model SkyModel;//model
        public Texture2D SkyTexture = null;//texture
        Effect SkyEffect;
        Matrix world;
        Matrix scale;
        Matrix translation;
        Matrix rotation;


        public SimpleSky()
        {
            Matrix SkyView = Matrix.Identity;
            Matrix SkyProj = Matrix.Identity;
            scale = Matrix.Identity;
            translation = Matrix.Identity;
            rotation = Matrix.Identity;
            world = Matrix.Identity;

            SkyModel = null;
            SkyTexture = null;

        }

        //set up effect 
        public void SetUpSkyEffect(Effect effect)
        {
            SkyEffect = effect;

            scale = Matrix.CreateScale(5);

            foreach (ModelMesh mesh in SkyModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = SkyEffect;
                }
            }

        }

        //draw sky sphere
       public void DrawSky(Matrix view, Matrix proj, Vector3 pos, GraphicsDevice Device)
        {
            Device.RenderState.DepthBufferWriteEnable = false;

            //calculate matrices
            pos.Z = pos.Z -100;
            pos.Y = pos.Y - 20;
            translation = Matrix.CreateTranslation(pos);
            world = scale *translation; 

            Matrix wvp = world * view * proj;
            Matrix wv = world * view;

            Matrix wvIT = Matrix.Invert(wv);
            wvIT = Matrix.Transpose(wvIT);

            Matrix worldIT = Matrix.Invert(world);
            worldIT = Matrix.Transpose(worldIT);


            SkyEffect.Parameters["gTex"].SetValue(SkyTexture);

            SkyEffect.CurrentTechnique = SkyEffect.Techniques["MyTech"];
            SkyEffect.Parameters["withlights"].SetValue(false);
            //set matrix params
            SkyEffect.Parameters["gWVP"].SetValue(wvp);
            SkyEffect.Parameters["gWorldView"].SetValue(wv);
            SkyEffect.Parameters["gWorldViewIT"].SetValue(wvIT);
            SkyEffect.Parameters["gWorld"].SetValue(world);
            SkyEffect.Parameters["gWorldIT"].SetValue(worldIT);
            //set texture
            SkyEffect.Parameters["gTex"].SetValue(SkyTexture);
            SkyEffect.Parameters["withgrey"].SetValue(true);

            SkyEffect.CommitChanges();


            // Draw the sphere model that the effect projects onto
            foreach (ModelMesh mesh in SkyModel.Meshes)
            {

                SkyEffect.CommitChanges();
                mesh.Draw();
            }

            Device.RenderState.DepthBufferWriteEnable = true;

        }
    }
}
