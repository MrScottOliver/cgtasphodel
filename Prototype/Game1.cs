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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager gDeviceManager;   //Graphics device manager
        BasicEffect stdEffect;
        Effect myEffect;//Jess: custom effect
        KeyboardState keyState;
        Player player = new Player(); //Kieran: create player instance
        Audio audio = new Audio();    //Stefen: create audio instance
        Vector3 POS;
        Vector3 TARGET;
        Vector3 UP;

        float YAW, PITCH, ROLL;

        Matrix View;
        Matrix Proj;



        private float aspectRatio;
        private float FOV;
        private float nearClip;
        private float farClip;



        public Game1()
        {
            gDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Prototype App";

            if (!InitGraphicsMode(1440, 900, false))
            {
                if (!InitGraphicsMode(1280, 720, false))
                {
                    if (!InitGraphicsMode(1024, 768, false))
                    {
                        if (!InitGraphicsMode(800, 600, false))
                        {
                            if (!InitGraphicsMode(640, 480, false))
                            {
                                this.Exit();
                            }
                        }
                    }
                }
            }

            stdEffect = new BasicEffect(gDeviceManager.GraphicsDevice, null);

            POS = new Vector3(10.0f, 15.0f, 32.0f);
            TARGET = new Vector3(10.0f, 5.0f, 1.0f);
            UP = Vector3.Up;

            YAW = PITCH = ROLL = 0.0f;

            aspectRatio = gDeviceManager.GraphicsDevice.Viewport.AspectRatio;
            FOV = 60.0f;
            nearClip = 1.0f;
            farClip = 1000.0f;

            Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), aspectRatio, nearClip, farClip);
            ObjectManipulator.Initialise(GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            AddLevelFront();
            AddLevelTop();
            AddPlatform();

            ObjectManipulator.CalculateBufferLengths();
            ObjectManipulator.ConcatanateArrays();
            ObjectManipulator.CreateBuffers(GraphicsDevice);

            //Jess: load effect file
            myEffect = Content.Load<Effect>("Phong_Shader");//Jess: load simple fx file

            //Kieran: set player model
            player.model = Content.Load<Model>("Ship2");
            player.scale = 2.0f;
            player.position.Y = 5f;
            player.position.Z = -5f;


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyState = Keyboard.GetState();
            // Allows the game to exit
#if !XBOX
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyState.IsKeyDown(Keys.W))
            {
                POS.Z -= 1.0f;
                TARGET.Z -= 1.0f;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                POS.Z += 1.0f;
                TARGET.Z += 1.0f;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                POS.X -= 1.0f;
                TARGET.X -= 1.0f;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                POS.X += 1.0f;
                TARGET.X += 1.0f;
            }

            //Kieran: arrow keys move player model (a ship)
            if (keyState.IsKeyDown(Keys.Up))
            {
                player.position.Y += 1.0f;
                audio.Step();
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                player.position.Y -= 1.0f;
                audio.Step();
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                player.position.X -= 1.0f;
                audio.Step();
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                player.position.X += 1.0f;
                audio.Step();
            }
            if (keyState.IsKeyDown(Keys.PageUp))

                if (keyState.IsKeyDown(Keys.PageDown))

#endif
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();

            //Stefen: Takes in object and cameras positions to provide 3d sound
            audio.Update(player.position, POS);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.VertexDeclaration = ObjectManipulator.vertexFormat;
            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            View = QuaternionCamera.GetViewMatrix(ref POS, ref TARGET, ref UP, YAW, PITCH, ROLL);

            myEffect.CurrentTechnique = myEffect.Techniques["TestLightTech"];//Jess: set current tech

            SetLightParams();//Jess: set light parameters

            ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj);

            //Kieran: call draw player function
            DrawPlayer(player);

            base.Draw(gameTime);


        }

        //Kieran: draw player function
        void DrawPlayer(Player player)
        {
            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World =
                        Matrix.CreateFromYawPitchRoll(
                        player.rotation.Y,
                        player.rotation.X,
                        player.rotation.Z) *

                        Matrix.CreateScale(player.scale) *

                        Matrix.CreateTranslation(player.position);

                    effect.Projection = Proj;
                    effect.View = View;
                }
                mesh.Draw();
            }
        }
        /***********************
         * Not my code!
        ***********************/
        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    gDeviceManager.PreferredBackBufferWidth = iWidth;
                    gDeviceManager.PreferredBackBufferHeight = iHeight;
                    gDeviceManager.IsFullScreen = bFullScreen;
                    gDeviceManager.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set. To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        gDeviceManager.PreferredBackBufferWidth = iWidth;
                        gDeviceManager.PreferredBackBufferHeight = iHeight;
                        gDeviceManager.IsFullScreen = bFullScreen;
                        gDeviceManager.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddLevelFront()
        {
            ObjectManipulator.NewLevelObject(8, 15, PrimitiveType.TriangleList, 5);

            ObjectManipulator.Current().AddVertexPNT(-10.0f, 5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(-10.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(-5.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);
            ObjectManipulator.Current().AddVertexPNT(30.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(-10.0f, -5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
            ObjectManipulator.Current().AddVertexPNT(30.0f, -5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

            ObjectManipulator.Current().AddIndex(0, 3, 2);
            ObjectManipulator.Current().AddIndex(0, 1, 3);
            ObjectManipulator.Current().AddIndex(1, 4, 3);
            ObjectManipulator.Current().AddIndex(2, 5, 7);
            ObjectManipulator.Current().AddIndex(2, 7, 6);

            ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexgrass");//load texture

            ObjectManipulator.Current().CalculateWorld();

            ObjectManipulator.Current().basicEffect = false;
        }

        private void AddLevelTop()
        {
            ObjectManipulator.NewLevelObject(12, 18, PrimitiveType.TriangleList, 6);

            Vector3 norm = Vector3.Transform(new Vector3(0, 1, 0), Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(30)));

            ObjectManipulator.Current().AddVertexPNT(-10.0f, 5.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f);//top
            ObjectManipulator.Current().AddVertexPNT(-10.0f, 5.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);//top

            ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);//top
            ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f);//top
            ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, 0.0f, norm.X, norm.Y, norm.Z, 0.0f, 0.0f);//middle
            ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, -10.0f, norm.X, norm.Y, norm.Z, 1.0f, 0.0f);//middle

            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f);//bottom
            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);//bottom
            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, norm.X, norm.Y, norm.Z, 0.0f, 1.0f);//middle
            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, -10.0f, norm.X, norm.Y, norm.Z, 1.0f, 1.0f);//middle

            ObjectManipulator.Current().AddVertexPNT(30.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);//bottom
            ObjectManipulator.Current().AddVertexPNT(30.0f, 0.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f);//bottom



            ObjectManipulator.Current().AddIndex(0, 1, 2);
            ObjectManipulator.Current().AddIndex(1, 3, 2);
            ObjectManipulator.Current().AddIndex(4, 5, 9);
            ObjectManipulator.Current().AddIndex(4, 9, 8);
            ObjectManipulator.Current().AddIndex(6, 7, 11);
            ObjectManipulator.Current().AddIndex(6, 11, 10);

            ObjectManipulator.Current().CalculateWorld();

            ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexleaves");

            ObjectManipulator.Current().basicEffect = false;


        }

        private void AddPlatform()
        {
            ObjectManipulator.NewLevelObject(16, 24, PrimitiveType.TriangleList, 8);

            //Top Face
            ObjectManipulator.Current().AddVertexPNT(0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);
            ObjectManipulator.Current().AddVertexPNT(0.0f, 1.0f, -10.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(10.0f, 1.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(10.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f);

            ObjectManipulator.Current().AddIndex(0, 1, 2);
            ObjectManipulator.Current().AddIndex(0, 2, 3);

            //Bottom Face
            ObjectManipulator.Current().AddVertexPNT(0, 0, 0, 0, -1, 0, 0, 1);
            ObjectManipulator.Current().AddVertexPNT(0, 0, -10, 0, -1, 0, 0, 0);
            ObjectManipulator.Current().AddVertexPNT(10, 0, -10, 0, -1, 0, 1, 0);
            ObjectManipulator.Current().AddVertexPNT(10, 0, 0, 0, -1, 0, 1, 1);

            ObjectManipulator.Current().AddIndex(5, 4, 6);
            ObjectManipulator.Current().AddIndex(4, 7, 6);

            //Front Face
            ObjectManipulator.Current().AddVertexPNT(0, -1, 0, 0, 0, -1, 0, 1);
            ObjectManipulator.Current().AddVertexPNT(0, 1, 0, 0, 0, -1, 0, 0);
            ObjectManipulator.Current().AddVertexPNT(10, 1, 0, 0, 0, -1, 1, 0);
            ObjectManipulator.Current().AddVertexPNT(10, -1, 0, 0, 0, -1, 1, 1);

            ObjectManipulator.Current().AddIndex(8, 9, 10);
            ObjectManipulator.Current().AddIndex(8, 10, 11);

            //Left Face
            ObjectManipulator.Current().AddVertexPNT(0, -1, -10, -1, 0, 0, 0, 1);
            ObjectManipulator.Current().AddVertexPNT(0, 1, -10, -1, 0, 0, 0, 0);
            ObjectManipulator.Current().AddVertexPNT(0, 1, 0, -1, 0, 0, 1, 0);
            ObjectManipulator.Current().AddVertexPNT(0, -1, 0, -1, 0, 0, 1, 1);

            ObjectManipulator.Current().AddIndex(12, 13, 14);
            ObjectManipulator.Current().AddIndex(12, 14, 15);



            ObjectManipulator.Current().AddTranslation(20, 10, 0);


            ObjectManipulator.Current().CalculateWorld();


            ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexgrass");

            ObjectManipulator.Current().basicEffect = false;
        }




        private void SetLightParams()
        {

            Vector4 Direction = new Vector4(-1, -10, 0, 0);
            Vector4 lcolour = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            //Jess: light parameters such as light color, direction, position, intensity which will be the same for all objects.//
            myEffect.Parameters["gLightCol"].SetValue(lcolour);
            myEffect.Parameters["gLightDir"].SetValue(Direction);


        }

    }
}
