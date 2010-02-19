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
        GrowEvent Plant = new GrowEvent(); //Stefen: create plant growth handler
        Vector3 POS;
        Vector3 TARGET;
        Vector3 UP;
        SkySphere LevelSky = new SkySphere(); //Jess: sky sphere
        PointLight[] lights;
        ObjectControl Control = new ObjectControl();//Stefen: object interface handeler, could be converted to singlton
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

            //if (!InitGraphicsMode(1440, 900, false))
            //{
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
            //}

            stdEffect = new BasicEffect(gDeviceManager.GraphicsDevice, null);

            POS = new Vector3(0.0f, 10.0f, 15.0f);
            TARGET = new Vector3(0.0f, 5.0f, 1.0f);
            UP = Vector3.Up;

            YAW = PITCH = ROLL = 0.0f;

            aspectRatio = gDeviceManager.GraphicsDevice.Viewport.AspectRatio;
            FOV = 60.0f;
            nearClip = 1.0f;
            farClip = 1000.0f;

            //Stefen: sets boundingbox and availability
            Plant.SetEvent(10, 0, -10, 1, true);

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
            AddPlatform(new Vector3(20, 13, 0));

            //adds the collision boxes
            SetupPlane();
            //Stefen:Creates + sets vertex and index buffers
            ObjectManipulator.UpdateObjects(GraphicsDevice);
            //Jess: load effect file
            myEffect = Content.Load<Effect>("simplepointlight");

            //Kieran: set player model
            player.model = Content.Load<Model>("tiny");
            player.AddRotation(4.70f,0.0f,0.0f);
            player.ChangeScale(0.012f);
            player.AddTranslation(-7f, 30f, -5f);
            /*
            player.position.X = -7f;
            player.position.Y = 20f;
            player.position.Z = -5f;
            //*/

            SetUpSkySphere();//Jess:set up sky sphere



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

            player.Move();
            CollisionDetectionBox.Compare(ref player);
            CollisionDetectionPlane.Compare(ref player);
            player.Update();

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
                player.AddTranslation(0, 0.3f, 0);
                audio.Step();
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                player.AddTranslation(0, -0.3f, 0);
                audio.Step();
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                player.AddTranslation(-0.3f, 0, 0);
                audio.Step();
                //POS.X -= 0.2f;
                //TARGET.X -= 0.2f;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                player.AddTranslation(0.3f, 0, 0);
                audio.Step();
                //POS.X += 0.2f;
                //TARGET.X += 0.2f;
            }

            //Kieran: intersection sphere for plant just to get things working! is located underneath the platform
            BoundingSphere plantSphere = new BoundingSphere(new Vector3(10, 0, -5), 5);
            //Stefen: if x is pressed and the event is available the event is activated
            if (keyState.IsKeyDown(Keys.X)&&(Plant.getStatus()==true)&&(player.boundingsphere.Intersects(plantSphere)))
            {
                Plant.Activate();
                AddPlant(Plant.getPos());
                ObjectManipulator.UpdateObjects(GraphicsDevice);
                CollisionDetectionBox.AddBox(new Vector3(5, 5, -10), new Vector3(15, 8, 0));
            }
            if (keyState.IsKeyDown(Keys.PageUp))

                if (keyState.IsKeyDown(Keys.PageDown))

#endif
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();

            //Stefen: Takes in object and cameras positions to provide 3d sound
            audio.Update(player.position, POS);

            POS.X = player.position.X + 1.0f;
            POS.Y = player.position.Y + 5.0f;
            TARGET.X = player.position.X + 1.0f;
            TARGET.Y = player.position.Y + 3.0f;

            //Stefen: Animate Growth
            if ((Plant.getActive()==true))
            {
                Plant.Animate(15);      //parameter is the desired y position by end of animation
                ObjectManipulator.LevelData.ElementAt(3).AddTranslation(0, 0.1F, 0); 
                if (Plant.getPos().Y>10)
                    AddPlatform(new Vector3(5, 7, 0));

                ObjectManipulator.UpdateObjects(GraphicsDevice);
            }
            if (Plant.Activated == true)
            {
                Plant.Transform();
            }
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

            myEffect.CurrentTechnique = myEffect.Techniques["MyTech"];//Jess: set current tech

            SetPointLights();//Jess: set light parameters

            ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj, Plant, POS, lights);

            //Kieran: call draw player function
            player.DrawPlayer(player, Proj, View);

            LevelSky.DrawSkySphere(View, Proj, GraphicsDevice);

            base.Draw(gameTime);


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

        private void AddPlatform(Vector3 Pos)
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



            ObjectManipulator.Current().AddTranslation(Pos.X, Pos.Y, Pos.Z);


            ObjectManipulator.Current().CalculateWorld();


            ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexgrass");

            ObjectManipulator.Current().basicEffect = false;
        }

        private void AddPlant(Vector3 Pos)
        {
            ObjectManipulator.NewLevelObject(16, 24, PrimitiveType.TriangleList, 8);

            //Top Face
            ObjectManipulator.Current().AddVertexPNT(0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);
            ObjectManipulator.Current().AddVertexPNT(0.0f, 0.0f, -10.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);
            ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f);

            ObjectManipulator.Current().AddIndex(0, 1, 2);
            ObjectManipulator.Current().AddIndex(0, 2, 3);

            //Bottom Face
            ObjectManipulator.Current().AddVertexPNT(0, -1, 0, 0, -1, 0, 0, 1);
            ObjectManipulator.Current().AddVertexPNT(0, -1, -10, 0, -1, 0, 0, 0);
            ObjectManipulator.Current().AddVertexPNT(5, -1, -10, 0, -1, 0, 1, 0);
            ObjectManipulator.Current().AddVertexPNT(5, -1, 0, 0, -1, 0, 1, 1);

            ObjectManipulator.Current().AddIndex(5, 4, 6);
            ObjectManipulator.Current().AddIndex(4, 7, 6);

            //Front Face
            ObjectManipulator.Current().AddVertexPNT(0, -15, 0, 0, 0, -1, 0, 1);
            ObjectManipulator.Current().AddVertexPNT(0, 0, 0, 0, 0, -1, 0, 0);
            ObjectManipulator.Current().AddVertexPNT(5, 0, 0, 0, 0, -1, 1, 0);
            ObjectManipulator.Current().AddVertexPNT(5, -15, 0, 0, 0, -1, 1, 1);

            ObjectManipulator.Current().AddIndex(8, 9, 10);
            ObjectManipulator.Current().AddIndex(8, 10, 11);

            //Left Face
            ObjectManipulator.Current().AddVertexPNT(0, -15, -10, -1, 0, 0, 0, 1);
            ObjectManipulator.Current().AddVertexPNT(0, 0, -10, -1, 0, 0, 0, 0);
            ObjectManipulator.Current().AddVertexPNT(0, 0, 0, -1, 0, 0, 1, 0);
            ObjectManipulator.Current().AddVertexPNT(0, -15, 0, -1, 0, 0, 1, 1);

            ObjectManipulator.Current().AddIndex(12, 13, 14);
            ObjectManipulator.Current().AddIndex(12, 14, 15);



            ObjectManipulator.Current().AddTranslation(Pos.X, Pos.Y, Pos.Z);


            ObjectManipulator.Current().CalculateWorld();


            ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexgrass");

            ObjectManipulator.Current().basicEffect = false;
        }

        //set directional light
        private void SetDirectionalLight()
        {
            Vector4 Direction = new Vector4(-1, -10, 0, 0);
            Vector4 lcolour = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            myEffect.Parameters["gLightCol"].SetValue(lcolour);
            myEffect.Parameters["gLightDir"].SetValue(Direction);
        }

        //set point lights
        private void SetPointLights()
        {
            int num = 2;

            myEffect.Parameters["numlights"].SetValue(num);//set number of lights

            lights = new PointLight[2];

            Vector4 Pos = new Vector4(20.0f, 20.0f, 0.0f, 1.0f);//light positions
            Vector4 Pos2 = new Vector4(0.0f, 10.0f, 0.0f, 1.0f);
            lights[0] = new PointLight(Pos);//init new point lights
            lights[1] = new PointLight(Pos2);

            for (int i = 0; i < num; i++)//set light colour values
            {
                lights[i].Ambient = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
                lights[i].Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                lights[i].Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                lights[i].Attenuation = new Vector3(0.0f, 0.002f, 0.005f);
            }

        }

       

        //Jess: Set up effects, texture, model for sky sphere
        private void SetUpSkySphere()
        {
            LevelSky.SkyModel = Content.Load<Model>("SphereHighPoly");
            LevelSky.SkyEffect = Content.Load<Effect>("SkySphere");
            LevelSky.SkyTexture = Content.Load<TextureCube>("skyboxmystic");

            LevelSky.SetUpSkyEffect();
        }

        private void SetupPlane()
        {
            //CollisionDetectionPlane.AddPlane(new Vector3(-10, 5, 0), new Vector3(-10, 5, -10), new Vector3(-5, 5, -10), -10, -5);
            CollisionDetectionBox.AddBox(new Vector3(-10, 3, -10), new Vector3(-5, 5, 0)); //Top slope
            CollisionDetectionBox.AddBox(new Vector3(20, 12, -10), new Vector3(30, 14, 0)); //Platform
            CollisionDetectionBox.AddBox(new Vector3(5, 0, -10), new Vector3(30, 0, 0)); //Bottom slope
            //CollisionDetectionPlane.AddPlane(new Vector3(-5, 5, 0), new Vector3(-5, 5, -10), new Vector3(5, 0, -10), -5, 5); //Middle slope, works but doesn't
        }

    }
}
