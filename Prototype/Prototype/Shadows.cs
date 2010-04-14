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
    class Shadows
    {
        private RenderTarget2D shadowRenderTarg;
        private DepthStencilBuffer shadowDepthBuff;
        private DepthStencilBuffer oldDepthBuff;
        private Texture2D shadowMap;

        private Matrix lightviewproj;
        private Vector3 lightDir = new Vector3(1.0f, 10.0f, 0.0f);

        private BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);


        public Shadows()
        {
         
        }

        public void SetUpShadowBuffer(GraphicsDeviceManager gDeviceManager)
        {
            SurfaceFormat shadowMapFormat = SurfaceFormat.Single;
            shadowRenderTarg = new RenderTarget2D(gDeviceManager.GraphicsDevice, 4096, 4096, 1, shadowMapFormat);
            shadowDepthBuff = new DepthStencilBuffer(gDeviceManager.GraphicsDevice, 4096, 4096, DepthFormat.Depth24);
        }

        private Matrix CreateLightViewProj()
        {
            Matrix lightrotation = Matrix.CreateLookAt(Vector3.Zero, -lightDir, Vector3.Up);

            Vector3[] frustrumCorners = cameraFrustum.GetCorners();

            for (int i = 0; i < frustrumCorners.Length; i++)
            {
                frustrumCorners[i] = Vector3.Transform(frustrumCorners[i], lightrotation);
            }

            BoundingBox lightbox = BoundingBox.CreateFromPoints(frustrumCorners);

            Vector3 boxsize = lightbox.Max - lightbox.Min;

            Vector3 halfboxsize = boxsize * 0.5f;

            Vector3 lightpos = lightbox.Min + halfboxsize;
            lightpos.Z = lightbox.Min.Z;

            lightpos = Vector3.Transform(lightpos, Matrix.Invert(lightrotation));

            Matrix lightview = Matrix.CreateLookAt(lightpos, lightpos - lightDir, Vector3.Up);

            Matrix lightproj = Matrix.CreateOrthographic(boxsize.X, boxsize.Y, -boxsize.Z, boxsize.Z);

            return lightview * lightproj;

        }

        public void SetUpShadowMap1(Effect myEffect, GraphicsDeviceManager gDeviceManager, Matrix View, Matrix Proj)
        {
             // Set the new frustum value
             cameraFrustum.Matrix = View * Proj;

             //set light view proj matrix
             lightviewproj = CreateLightViewProj();

             myEffect.Parameters["gLightviewproj"].SetValue(lightviewproj);

             gDeviceManager.GraphicsDevice.SetRenderTarget(0, shadowRenderTarg);

             oldDepthBuff = gDeviceManager.GraphicsDevice.DepthStencilBuffer;

             gDeviceManager.GraphicsDevice.DepthStencilBuffer = shadowDepthBuff;

             gDeviceManager.GraphicsDevice.Clear(Color.White);
    
             myEffect.CurrentTechnique = myEffect.Techniques["CreateShadowMapTech"];// set current tech



        }


       public void SetUpShadowMap2(Effect myEffect, GraphicsDeviceManager gDeviceManager)
        {
            
            gDeviceManager.GraphicsDevice.SetRenderTarget(0, null);

            gDeviceManager.GraphicsDevice.DepthStencilBuffer = oldDepthBuff;

            shadowMap = shadowRenderTarg.GetTexture();

            myEffect.Parameters["gShadowMap"].SetValue(shadowMap);

        }




    }
}
