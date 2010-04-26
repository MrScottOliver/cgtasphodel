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
using DPSF;
using DPSF.ParticleSystems;

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
        GrowEvent Plant = new GrowEvent(); //Stefen: create plant growth handler
        Vector3 POS;
        Vector3 TARGET;
        Vector3 UP;
        SkySphere LevelSky = new SkySphere(); //Jess: sky sphere
        Lights[] lights;
        ObjectControl ObjControl = new ObjectControl();//Stefen: object interface handeler, could be converted to singlton
       // Surface  PlatLeaf3, PlatHill1, PlatHill2, Mushroom, MainHill;
        Surface PlatLeaf1,PlatLeaf2, FullLevel, Mushroom1, Mushroom2;
        float YAW, PITCH, ROLL;
        Matrix View;
        Matrix Proj;
        Matrix World = Matrix.CreateTranslation(0, 0, 0);
        Shadows myShadows = new Shadows();

        PhysicsActor paHill1;
        PhysicsActor paBall;

        //New physics stuff
        Physics physicSystem;

        //stefen: this bool is rubbish, prevents multiple jump noises
        bool jumppressed;

        int numlights = 0; //number of point lights

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
            Audio.Init();
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
            ParticleGroup.Init(this.GraphicsDevice, this.Content);
            //adds the collision boxes
            SetupPlane();
            //Stefen:Creates + sets vertex and index buffers
            ObjectManipulator.UpdateObjects(GraphicsDevice);
            //Jess: load effect file
            myEffect = Content.Load<Effect>("Lighting");

            //Kieran: set player model
            player.model = Content.Load<Model>("characterX");
            player.texture = Content.Load<Texture2D>("metal");

            player.RemapModel(player, myEffect);//remap model to use our effect
            
            player.AddRotation((float)Math.PI  / 2, 0.0f, 0.0f);
            player.ChangeScale(1.0f);
            player.AddTranslation(-7f, 30f, 0f);
            /*
            player.position.X = -7f;
            player.position.Y = 20f;
            player.position.Z = -5f;
            //*/





            SetUpSkySphere();//Jess:set up sky sphere
            Model OrbModel = Content.Load<Model>("ball");
            Model PlantCyl = Content.Load<Model>("Flower1");
            Model Hill1 = Content.Load<Model>("Hill1");
            Model Hill2 = Content.Load<Model>("Hill2");
            Model Full = Content.Load<Model>("MainLevel1");
            Model Mush1 = Content.Load<Model>("MurshroomPlatformtest");
            Model Mush2 = Content.Load<Model>("MurshroomPlatformtest2");
            Model Leaf1 = Content.Load<Model>("leave1");
            Model Mush= Content.Load<Model>("Murshroom1");
            Model LevelBuild2 = Content.Load<Model>("LevelBuild2");
            Model Flower = Content.Load<Model>("Flower2");
            Model MnHill = Content.Load<Model>("Mainhill2");



            //New physics
            physicSystem = new Physics();

            TriangleMeshObject tmoHill1 = new TriangleMeshObject(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
            tmoHill1.SetModel("Content/Hill1", Hill1);

            paHill1 = new PhysicsActor(Hill1, tmoHill1);
            paHill1.PhysicsObject.Immovable = true;

            BoxObject ball = new BoxObject(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(5.0f, 20.0f, -5.0f), new Vector3(0.0f, 0.0f, 0.0f));

            paBall = new PhysicsActor(OrbModel, ball);
            paBall.PhysicsObject.Mass = 1000;




           /* PlatHill1 = new Surface(Hill1, new Vector3(45, 0, -8));
            PlatHill1.Scale(0.3f, 0.3f, 0.3f);
            PlatHill1.Rotate(1.0f, 0.0f, 0.0f);
            PlatHill2 = new Surface(Hill2, new Vector3(0, 0, 0));
            PlatHill2.Scale(0.3f, 0.3f, 0.3f);
            PlatHill2.Rotate(3.0f, 0.0f, 0.0f);*/
            FullLevel = new Surface(Full, new Vector3(0, 0, 0));
            /*
           // FullLevel.Rotate(3.0f, 0.0f, 0.0f);
            Mushroom = new Surface(Mush, new Vector3(0, 0, 0));
            MainHill = new Surface(MnHill, new Vector3(0, 0, 0));

            PlatLeaf3 = new Surface(Flower, new Vector3(0, 0, 0));
            PlatHill2 = new Surface(LevelBuild2, new Vector3(0, 0, 0));
            */
            PlatLeaf1 = new Surface(Leaf1, new Vector3(25, 14, -3));
            PlatLeaf1.Scale(0.75f, 0.75f, 0.75f);
            PlatLeaf1.Rotate(1.0f, 0.0f, 0.0f);
            PlatLeaf2 = new Surface(Leaf1, new Vector3(10, 8, -3));
            PlatLeaf2.Scale(0.75f, 0.75f, 0.75f);
            PlatLeaf2.Rotate(1.0f, 0.0f, 0.0f);
            Mushroom1 = new Surface(Mush1, new Vector3(20, 20, -3));
            Mushroom2 = new Surface(Mush2, new Vector3(50, 20, -3));
           //Stefen: Apply transformations for last object entered

            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(0, 0, 0))
            );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(5, 5, 0))
            );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(10, 10, 0))
            );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(15, 15, 0))
            );
            ObjectControl.ObjectList.Add(
           ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(30, -20, 0))
           );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(10, -20, 0))
            );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(25, 14, 0))
            );
            ObjectControl.ObjectList.Add(
           ObjectFactory.createObject(ObjectType.Plant, PlantCyl, new Vector3(30, -20, 0))
           );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Plant, PlantCyl, new Vector3(10, -20, 0))
        );
            ObjectControl.ObjectList.Add(
           ObjectFactory.createObject(ObjectType.Mushroom, Mush1, new Vector3(20, 0, -4))
           );
            ObjectControl.ObjectList.Add(
 ObjectFactory.createObject(ObjectType.Mushroom, Mush1, new Vector3(22, 0, -4))
 );
            ObjectControl.ObjectList.Add(
 ObjectFactory.createObject(ObjectType.Mushroom, Mush1, new Vector3(24, 0, -4))
 );
            ObjectControl.ObjectList.Add(
 ObjectFactory.createObject(ObjectType.Mushroom, Mush1, new Vector3(26, 0, -4))
 );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Mushroom, Mush2, new Vector3(10, 0, -4))
        );
           // ObjectControl.ObjectList..SetPosition(10, 10, 0, 0);
           // ObjControl.Load();

            myShadows.SetUpShadowBuffer(gDeviceManager);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //mcOrbParticleSystem.Destroy();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ParticleGroup.Update(gameTime);
            physicSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            Audio.PlayMusic();
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
                //rough audio handler
                if (jumppressed == false)
                    Audio.Jump();
                jumppressed = true;
            }
            else
                jumppressed = false;

            if (keyState.IsKeyDown(Keys.Down))
            {
                player.AddTranslation(0, -0.3f, 0);
                Audio.Slide();
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                player.AddTranslation(-0.3f, 0, 0);
                Audio.Step();
                //POS.X -= 0.2f;
                //TARGET.X -= 0.2f;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                player.AddTranslation(0.3f, 0, 0);
                Audio.Step();
                //POS.X += 0.2f;
                //TARGET.X += 0.2f;
            }
            ObjControl.Collision(player.boundingsphere);
            //Stefen: Rotate player model for viewing
            if (keyState.IsKeyDown(Keys.E))
            {
                player.AddRotation(0.1f,0,0);
            }

            if (keyState.IsKeyDown(Keys.R))
            {
                player.AddRotation(0, 0.1f, 0);
            }
            if (keyState.IsKeyDown(Keys.T))
            {
                player.AddRotation(0, 0, 0.1f);
            }
            //Stefen:Controls for switching background music
            if (keyState.IsKeyDown(Keys.G))
            {
                Audio.SetMusic(Audio.Tracks.mute);
            }
            if (keyState.IsKeyDown(Keys.H))
            {
                Audio.SetMusic(Audio.Tracks.acoustic);
            }
            if (keyState.IsKeyDown(Keys.J))
            {
                Audio.SetMusic(Audio.Tracks.dark);
            }
            if (keyState.IsKeyDown(Keys.K))
            {
                Audio.SetMusic(Audio.Tracks.piano);
            }
            if (keyState.IsKeyDown(Keys.L))
            {
                Audio.SetMusic(Audio.Tracks.title);
            }
            //Kieran: intersection sphere for plant just to get things working! is located underneath the platform
            BoundingSphere plantSphere = new BoundingSphere(new Vector3(10, 0, 0), 5);
            //Stefen: if x is pressed and the event is available the event is activated
            if (keyState.IsKeyDown(Keys.X)&&(Plant.getStatus()==true)&&(player.boundingsphere.Intersects(plantSphere)))
            {
                Plant.Activate();

                ObjectManipulator.UpdateObjects(GraphicsDevice);
                CollisionDetectionBox.AddBox(new Vector3(5, 5, -10), new Vector3(15, 8, 0));
                
            }
            if (keyState.IsKeyDown(Keys.PageUp))

                if (keyState.IsKeyDown(Keys.PageDown))

#endif
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();

            //Stefen: Takes in object and cameras positions to provide 3d sound
            Audio.Update(player.position, POS);

            POS.X = player.position.X + 1.0f;
            POS.Y = player.position.Y + 5.0f;
            TARGET.X = player.position.X + 1.0f;
            TARGET.Y = player.position.Y + 3.0f;

            //Stefen: Animate Growth
            if ((Plant.getActive()==true))
            {
                Plant.Animate(15);      //parameter is the desired y position by end of animation
                if (Plant.getPos().Y > 10)
                {
                    PlatLeaf2.Render(View, Proj, GraphicsDevice);
                }
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

            CreateShadowMap();

            myEffect.CurrentTechnique = myEffect.Techniques["MyTech"];//Jess: set current tech

            SetUpLighting();//Jess: set light parameters

            UpdateEffectParams();

            ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj, Plant, POS, lights);

            //Kieran: call draw player function
            player.DrawPlayer2(player, Proj, View);
            paHill1.Draw(Proj, View);
            paBall.Draw(Proj, View);

            LevelSky.DrawSkySphere(View, Proj, GraphicsDevice);

            ObjControl.Render(View, Proj, GraphicsDevice);
           // PlatHill1.Render(View, Proj, GraphicsDevice);
           // PlatHill2.Render(View, Proj, GraphicsDevice);
            FullLevel.Render(View, Proj, GraphicsDevice);
           // MainHill.Render(View, Proj, GraphicsDevice);
           // Mushroom.Render(View, Proj, GraphicsDevice);
           // PlatLeaf3.Render(View, Proj, GraphicsDevice);
           // PlatHill2.Render(View, Proj, GraphicsDevice);
            PlatLeaf1.Render(View, Proj, GraphicsDevice);
            Mushroom1.Render(View, Proj, GraphicsDevice);
            Mushroom2.Render(View, Proj, GraphicsDevice);
            if (Plant.getPos().Y > 10)
            {
                PlatLeaf2.Render(View, Proj, GraphicsDevice);
            }
            base.Draw(gameTime);

            ParticleGroup.Draw(World, View, Proj);

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

        List<Vector3> GetOrbPosition()
        {
            List<IObject> ObjectList = ObjectControl.ObjectList;
            List<Vector3> OrbPositions = new List<Vector3>();
            int OrbNum = 0;
            foreach (IObject item in ObjectList)
            {
                Type x = item.GetType();
                if (x.Name == "Orb")
                {
                    Orb y = (Orb)item;
                    OrbPositions.Add(y.GetPosition());
                    OrbNum++;
                };
            }
            return OrbPositions;
        }

        private void SetUpLighting()//set point lights and directional light
        {

            lights = new Lights[8];

            
            Vector4 Pos = new Vector4(0.0f, 5000.0f, 0.0f, 1.0f);//light positions

            lights[0] = new Lights();
            lights[0].Position = Pos;//fake sun :p
            lights[0].Ambient = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            lights[0].Diffuse = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
            lights[0].Specular = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            lights[0].Attenuation = new Vector3(0.0f, 0.00000001f, 0.00000002f);


            //Stefen: to test orbs seperate from other lights
            numlights= 1;
            foreach (Vector3 coordinate in GetOrbPosition())
            {
                numlights++;
                lights[numlights-1] = new Lights();//point light
                lights[numlights-1].Position = new Vector4(coordinate, 1.0f);
                
            }

            myEffect.Parameters["numlights"].SetValue(numlights);//set number of lights

        }

        private void CreateShadowMap()
        {

            myShadows.SetUpShadowMap1(myEffect, gDeviceManager, View, Proj);

            ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj, Plant, POS, lights);
            player.DrawPlayerShadow(player, Proj, View);

            myShadows.SetUpShadowMap2(myEffect, gDeviceManager);

        }


        private void UpdateEffectParams()
        {
            Vector4 CamPosition = new Vector4(POS.X, POS.Y, POS.Z, 0);
            myEffect.Parameters["gCamPosW"].SetValue(CamPosition);

            myEffect.Parameters["gCenterX"].SetValue(Plant.getX() + 2);
            myEffect.Parameters["gCenterY"].SetValue(Plant.getY());
            myEffect.Parameters["gCenterZ"].SetValue(Plant.getZ());
            myEffect.Parameters["gMinY"].SetValue(Plant.minY);
            myEffect.Parameters["gMaxY"].SetValue(Plant.maxY);
            myEffect.Parameters["gRadius"].SetValue(Plant.radius);


            for (int i = 0; i < numlights; i++)
            {
                lights[i].UpdateLight(myEffect.Parameters["light"].Elements[i]);
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
            CollisionDetectionPlane.AddPlane(new Vector3(-5, 5, 0), new Vector3(5, 0, 0), new Vector3(5, 0, -10), -5, 5); //Middle slope, works but doesn't
        }

    }
}
