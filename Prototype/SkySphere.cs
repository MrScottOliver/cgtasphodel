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
    class SkySphere
    {
        public Model SkyModel;//sphere model
        public Effect SkyEffect;//sky sphere effect
        public TextureCube SkyTexture = null;//texture
        Matrix SkyView;//view matrix
        Matrix SkyProj;//projection matrix


        public SkySphere()
        {
            Matrix SkyView = Matrix.Identity;
            Matrix SkyProj = Matrix.Identity;
            SkyModel = null;
            SkyEffect = null;
            SkyTexture = null;

        }

        //set up effect 
        public void SetUpSkyEffect()
        {
            SkyEffect.Parameters["gEnvMap"].SetValue(SkyTexture);
            //SkyEffect.Parameters["gWVP"].SetValue(SkyWVP);
           
            SkyEffect.Parameters["mProjection"].SetValue(SkyProj);
            SkyEffect.Parameters["mView"].SetValue(SkyView);

            foreach (ModelMesh mesh in SkyModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = SkyEffect;
                }
            }

        }

        //draw sky sphere
       public void DrawSkySphere(Matrix view, Matrix proj, GraphicsDevice Device)
        {


            SkyProj = proj;
            SkyView = view;

           //set view and proj params for sky effect
            SkyEffect.Parameters["mProjection"].SetValue(SkyProj);
            SkyEffect.Parameters["mView"].SetValue(SkyView);

            // Draw the sphere model that the effect projects onto
            foreach (ModelMesh mesh in SkyModel.Meshes)
            {
                mesh.Draw();
            }

           //undo render state settings that shader sets
            Device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            Device.RenderState.DepthBufferWriteEnable = true;

        }
    
    }

}
