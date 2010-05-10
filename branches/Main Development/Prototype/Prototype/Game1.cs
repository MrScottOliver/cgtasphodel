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
        Vector3 POS;
        Vector3 TARGET;
        Vector3 UP;
       //SkySphere LevelSky = new SkySphere(); //Jess: sky sphere
        SimpleSky Background = new SimpleSky();
        Lights[] lights;
        ObjectControl ObjControl = new ObjectControl();//Stefen: object interface handeler, could be converted to singlton
        Surface  FullLevel1, FullLevel2, Mushroom1, Mushroom2;
        float YAW, PITCH, ROLL;
        Matrix View;
        Matrix Proj;
        Matrix World = Matrix.CreateTranslation(0, 0, 0);
        Shadows myShadows = new Shadows();

        float OrbGlow = 2.3f;//variables for making the orb glow pulsate
        int pulsate = 1;
        int orbnum = 0;
        Vector3[] OrbPositions = new Vector3[20];

        PostProcess Bloom;//bloom post process effect 

        
        PhysicsActor paBall1;
        PhysicsActor paBall2;
        PhysicsActor paBall3;
        PhysicsActor paBall4;

        //New physics stuff
        Physics physicSystem;

        //stefen: this bool is rubbish, prevents multiple jump noises
       // bool jumppressed;

        int numlights = 0; //number of point lights

        private float aspectRatio;
        private float FOV;
        private float nearClip;
        private float farClip;

        private float transformDegree = 1.0f;
        private bool revealTransform = false;



        public Game1()
        {
            gDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Bloom= new PostProcess(this);
            Components.Add(Bloom);
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

            Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), aspectRatio, nearClip, farClip);
           // ObjectManipulator.Initialise(GraphicsDevice);
            Audio.Init();
            Audio.SetMusic(Audio.Tracks.title);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
           // AddLevelFront();
            //AddLevelTop();
            ParticleGroup.Init(this.GraphicsDevice, this.Content);
            //adds the collision boxes
            SetupPlane();
            //Stefen:Creates + sets vertex and index buffers
            //ObjectManipulator.UpdateObjects(GraphicsDevice);
            //Jess: load effect file
            myEffect = Content.Load<Effect>("Lighting");

            //Kieran: set player model
            player.model = Content.Load<Model>("characterX");
            player.texture = Content.Load<Texture2D>("metal");

            player.RemapModel(player, myEffect);//remap model to use our effect
            
            player.AddRotation((float)Math.PI  / 2, 0.0f, 0.0f);
            player.ChangeScale(0.5f);
            player.AddTranslation(-7f, 30f, 0f);
            /*
            player.position.X = -7f;
            player.position.Y = 20f;
            player.position.Z = -5f;
            //*/


            SetUpSky();//Jess:set up sky 


            Model OrbModel = Content.Load<Model>("ball");
            Model PlantCyl = Content.Load<Model>("Flower1");
            Model Hill1 = Content.Load<Model>("Hill1");
           // Model Hill2 = Content.Load<Model>("Hill2");
            Model Full1 = Content.Load<Model>("LevelBuild7");
            Model Full2 = Content.Load<Model>("LevelBuild6");
           Model Mush1 = Content.Load<Model>("MurshroomPlatformtest");
           Model Mush2 = Content.Load<Model>("MurshroomPlatformtest2");
            Model Leaf1 = Content.Load<Model>("leave1");
           // Model Mush= Content.Load<Model>("Murshroom1");
            //Model LevelBuild2 = Content.Load<Model>("LevelBuild2");
            //Model Flower = Content.Load<Model>("Flower2");
            // Model MnHill = Content.Load<Model>("Mainhill2");

            RemapModel(OrbModel, myEffect);
            RemapModel(PlantCyl, myEffect);
            RemapModel(Full1, myEffect);
            RemapModel(Full2, myEffect);
            RemapModel(Mush1, myEffect);
            RemapModel(Mush2, myEffect);
            RemapModel(Leaf1, myEffect);

            //New physics
            physicSystem = new Physics();




            SphereObject ball1 = new SphereObject(2.0f, new Vector3(5.0f, 16.0f, -5.0f), new Vector3(0.0f, 0.0f, 0.0f));
            SphereObject ball2 = new SphereObject(1.0f, new Vector3(4.0f, 22.0f, -5.0f), new Vector3(0.0f, 0.0f, 0.0f));
            SphereObject ball3 = new SphereObject(1.0f, new Vector3(5.0f, 24.0f, -3.0f), new Vector3(0.0f, 0.0f, 0.0f));
            SphereObject ball4 = new SphereObject(1.0f, new Vector3(5.1f, 28.0f, -5.0f), new Vector3(0.0f, 0.0f, 0.0f));

            paBall1 = new PhysicsActor(OrbModel, ball1);
            paBall1.RemapModel(paBall1, myEffect);
            paBall1.PhysicsObject.Mass = 1000;
            paBall1.PhysicsObject.Immovable = true;

            paBall2 = new PhysicsActor(OrbModel, ball2);
            paBall2.RemapModel(paBall2, myEffect);
            paBall2.PhysicsObject.Mass = 2000;

            paBall3 = new PhysicsActor(OrbModel, ball3);
            paBall3.RemapModel(paBall3, myEffect);
            paBall3.PhysicsObject.Mass = 2000;
           

            paBall4 = new PhysicsActor(OrbModel, ball4);
            paBall4.RemapModel(paBall4, myEffect);
            paBall4.PhysicsObject.Mass = 500;





           /* PlatHill1 = new Surface(Hill1, new Vector3(45, 0, -8));
            PlatHill1.Scale(0.3f, 0.3f, 0.3f);
            PlatHill1.Rotate(1.0f, 0.0f, 0.0f);
            PlatHill2 = new Surface(Hill2, new Vector3(0, 0, 0));
            PlatHill2.Scale(0.3f, 0.3f, 0.3f);
            PlatHill2.Rotate(3.0f, 0.0f, 0.0f);*/
            FullLevel1 = new Surface(Full1, new Vector3(35.0f, -5.2f, -15.0f));
            FullLevel2 = new Surface(Full2, new Vector3(35.0f, -5.2f, -15.0f));
            /*
           // FullLevel.Rotate(3.0f, 0.0f, 0.0f);
            Mushroom = new Surface(Mush, new Vector3(0, 0, 0));
            MainHill = new Surface(MnHill, new Vector3(0, 0, 0));

            PlatLeaf3 = new Surface(Flower, new Vector3(0, 0, 0));
            PlatHill2 = new Surface(LevelBuild2, new Vector3(0, 0, 0));
            */
           //Stefen: Apply transformations for last object entered

            
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(44.5f, 26, 0))
            );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(99, 26, 0))
            );
            ObjectControl.ObjectList.Add(
           ObjectFactory.createObject(ObjectType.Plant, PlantCyl, new Vector3(44, -20, 0))
           );
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Leaf, Leaf1, new Vector3(43, 4, 0))
            );
                        ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Leaf, Leaf1, new Vector3(46, 9, 0))
            );
                        ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Leaf, Leaf1, new Vector3(43, 14, 0))
            );
                        ObjectControl.ObjectList.Add(
             ObjectFactory.createObject(ObjectType.Plant, PlantCyl, new Vector3(10, -20, 0))
            );
            
            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Mushroom, Mush1, new Vector3(99, 0, -4))
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

            if (revealTransform && transformDegree > 0.0f)
            {
                transformDegree -= 0.001f;
            }

            ParticleGroup.Update(gameTime);
            //physicSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
                player.Jump();
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                Audio.Slide();
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                player.moveLeft();
            }
            else
            {
                player.stopLeft();
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                player.moveRight();
            }
            else
            {
                player.stopRight();
            }

            if (keyState.IsKeyDown(Keys.Space))
            {
                revealTransform = true;
            }

            ObjControl.HandleState(player);
            //Stefen: Rotate player model for viewing
            if (keyState.IsKeyDown(Keys.E))
            {
                //player.AddRotation(0.1f,0,0);
                player.health--;
            }

            if (keyState.IsKeyDown(Keys.R))
            {
                //player.AddRotation(0, 0.1f, 0);
                player.health++;
            }

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
            BoundingSphere plantSphere = new BoundingSphere(new Vector3(10, 0, 0), 5);//DOGGON

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
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {


            GraphicsDevice.Clear(Color.CornflowerBlue);
           // GraphicsDevice.VertexDeclaration = ObjectManipulator.vertexFormat;
            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            View = QuaternionCamera.GetViewMatrix(ref POS, ref TARGET, ref UP, YAW, PITCH, ROLL);

            CreateShadowMap();

            myEffect.CurrentTechnique = myEffect.Techniques["MyTech"];//Jess: set current tech

            SetUpLighting();//Jess: set light parameters

            UpdateEffectParams();

            //ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj, POS, lights);
           
            //LevelSky.DrawSkySphere(View, Proj, GraphicsDevice);
           
            //Kieran: call draw player function
            player.DrawPlayer2(player, Proj, View);
            //paHill1.Draw(Proj, View);
            /*paBall1.Draw2(Proj, View);
            paBall2.Draw2(Proj, View);
            paBall3.Draw2(Proj, View);
            paBall4.Draw2(Proj, View);*/
      

            ObjControl.Render(View, Proj, GraphicsDevice);
           // PlatHill1.Render(View, Proj, GraphicsDevice);
           // PlatHill2.Render(View, Proj, GraphicsDevice);
            FullLevel1.RePosition(new Vector3(140,-6,0));
            FullLevel1.Render(View, Proj, GraphicsDevice);
            FullLevel2.Render(View, Proj, GraphicsDevice);
           // MainHill.Render(View, Proj, GraphicsDevice);
           // Mushroom.Render(View, Proj, GraphicsDevice);
           // PlatLeaf3.Render(View, Proj, GraphicsDevice);
           // PlatHill2.Render(View, Proj, GraphicsDevice);
            //Mushroom1.Render(View, Proj, GraphicsDevice);
            //Mushroom2.Render(View, Proj, GraphicsDevice);
            
            Background.DrawSky(View, Proj, POS, GraphicsDevice);


            ParticleGroup.Draw(World, View, Proj);

             
 
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

        //private void AddLevelFront()
        //{
        //    ObjectManipulator.NewLevelObject(8, 15, PrimitiveType.TriangleList, 5);

        //    ObjectManipulator.Current().AddVertexPNT(-10.0f, 5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
        //    ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
        //    ObjectManipulator.Current().AddVertexPNT(-10.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
        //    ObjectManipulator.Current().AddVertexPNT(-5.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
        //    ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);
        //    ObjectManipulator.Current().AddVertexPNT(30.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
        //    ObjectManipulator.Current().AddVertexPNT(-10.0f, -5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
        //    ObjectManipulator.Current().AddVertexPNT(30.0f, -5.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

        //    ObjectManipulator.Current().AddIndex(0, 3, 2);
        //    ObjectManipulator.Current().AddIndex(0, 1, 3);
        //    ObjectManipulator.Current().AddIndex(1, 4, 3);
        //    ObjectManipulator.Current().AddIndex(2, 5, 7);
        //    ObjectManipulator.Current().AddIndex(2, 7, 6);

        //    ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexgrass");//load texture

        //    ObjectManipulator.Current().CalculateWorld();

        //    ObjectManipulator.Current().basicEffect = false;
        //}

        //private void AddLevelTop()
        //{
        //    ObjectManipulator.NewLevelObject(12, 18, PrimitiveType.TriangleList, 6);

        //    Vector3 norm = Vector3.Transform(new Vector3(0, 1, 0), Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(30)));

        //    ObjectManipulator.Current().AddVertexPNT(-10.0f, 5.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f);//top
        //    ObjectManipulator.Current().AddVertexPNT(-10.0f, 5.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);//top

        //    ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);//top
        //    ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f);//top
        //    ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, 0.0f, norm.X, norm.Y, norm.Z, 0.0f, 0.0f);//middle
        //    ObjectManipulator.Current().AddVertexPNT(-5.0f, 5.0f, -10.0f, norm.X, norm.Y, norm.Z, 1.0f, 0.0f);//middle

        //    ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f);//bottom
        //    ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);//bottom
        //    ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, 0.0f, norm.X, norm.Y, norm.Z, 0.0f, 1.0f);//middle
        //    ObjectManipulator.Current().AddVertexPNT(5.0f, 0.0f, -10.0f, norm.X, norm.Y, norm.Z, 1.0f, 1.0f);//middle

        //    ObjectManipulator.Current().AddVertexPNT(30.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);//bottom
        //    ObjectManipulator.Current().AddVertexPNT(30.0f, 0.0f, -10.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f);//bottom



        //    ObjectManipulator.Current().AddIndex(0, 1, 2);
        //    ObjectManipulator.Current().AddIndex(1, 3, 2);
        //    ObjectManipulator.Current().AddIndex(4, 5, 9);
        //    ObjectManipulator.Current().AddIndex(4, 9, 8);
        //    ObjectManipulator.Current().AddIndex(6, 7, 11);
        //    ObjectManipulator.Current().AddIndex(6, 11, 10);

        //    ObjectManipulator.Current().CalculateWorld();

        //    ObjectManipulator.Current().tex = Content.Load<Texture2D>("testtexleaves");

        //    ObjectManipulator.Current().basicEffect = false;


        //}

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


            //attach point lights to each visible orb
            numlights= 1;
            orbnum = 0;
            foreach (Vector3 coordinate in GetOrbPosition())
            {
                numlights++;
                orbnum++;
                lights[numlights - 1] = new Lights();//point light
                lights[numlights - 1].Position = new Vector4(coordinate, 1.0f);

                OrbPositions[orbnum - 1] = coordinate;

            }

            myEffect.Parameters["orbnum"].SetValue(orbnum);
            myEffect.Parameters["numlights"].SetValue(numlights);//set number of lights

        }


        private void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

        private void CreateShadowMap()
        {

            myShadows.SetUpShadowMap1(myEffect, gDeviceManager, View, Proj);

            //ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj, POS, lights);
            player.DrawPlayerShadow(player, Proj, View);

            ObjControl.Render(View, Proj, GraphicsDevice);
            FullLevel2.Render(View, Proj, GraphicsDevice);


            myShadows.SetUpShadowMap2(myEffect, gDeviceManager);

        }


        private void UpdateEffectParams()
        {
            Vector4 CamPosition = new Vector4(POS.X, POS.Y, POS.Z, 0);
            myEffect.Parameters["gCamPosW"].SetValue(CamPosition);
            List<IObject> ObjectList = ObjectControl.ObjectList;
            foreach (IObject item in ObjectList)
            {
                Type x = item.GetType();
                if (x.Name == "Plant")
                {
                    Plant y = (Plant)item;
                    myEffect.Parameters["gCenterX"].Elements[y.plantnum].SetValue(y.getX() + 2);
                    myEffect.Parameters["gCenterY"].Elements[y.plantnum].SetValue(y.getY());
                    myEffect.Parameters["gCenterZ"].Elements[y.plantnum].SetValue(y.getZ());
                    myEffect.Parameters["gMinY"].Elements[y.plantnum].SetValue(y.minY);
                    myEffect.Parameters["gMaxY"].Elements[y.plantnum].SetValue(y.maxY);
                    myEffect.Parameters["gRadius"].Elements[y.plantnum].SetValue(y.radius);
                    myEffect.Parameters["player"].SetValue(false);
                    myEffect.Parameters["reveal"].SetValue(revealTransform);
                    myEffect.Parameters["transformDegree"].SetValue(transformDegree);
                }
            }


            for (int i = 0; i < numlights; i++)
            {
                lights[i].UpdateLight(myEffect.Parameters["light"].Elements[i]);
            }


            myEffect.Parameters["OrbPos"].SetValue(OrbPositions);

            if (pulsate == 1)
            {
                if (OrbGlow > 2.2f)
                {
                    OrbGlow -= 0.01f;
                }
                else if (OrbGlow <= 2.2f)
                {
                    pulsate = 0;
                }
            }
            else if (pulsate == 0)
            {
                if (OrbGlow < 2.5f)
                {
                    OrbGlow += 0.01f;
                }
                else if (OrbGlow >= 2.5f)
                {
                    pulsate = 1;
                }
            }

            myEffect.Parameters["orbRadius"].SetValue(OrbGlow);

        }

        //Jess: Set up effects, texture, model for sky sphere
        private void SetUpSky()
        {
            //LevelSky.SkyModel = Content.Load<Model>("SphereHighPoly");
            //LevelSky.SkyEffect = Content.Load<Effect>("SkySphere");
            //LevelSky.SkyTexture = Content.Load<TextureCube>("skyboxmystic");

            //LevelSky.SetUpSkyEffect();


            Background.SkyModel = Content.Load<Model>("quad");
            Background.SkyTexture = Content.Load<Texture2D>("spires_north");
            Background.SetUpSkyEffect(myEffect); 

        }

        private void SetupPlane()
        {
            CollisionDetectionBox.AddBox(new Vector3(-12, 3, -10), new Vector3(-3, 4.7f, 0)); //First platform

            CollisionDetectionBox.AddBox(new Vector3(-1, 8, -10), new Vector3(8, 10.8f, 0)); //Second platform

            CollisionDetectionBox.AddBox(new Vector3(10, 3, -10), new Vector3(18, 4.9f, 0)); //Third platform

            CollisionDetectionBox.AddBox(new Vector3(18, -6, -10), new Vector3(28, -3.2f, 0)); //Bottom of hill 1
            CollisionDetectionPlane.AddPlane(new Vector3(28, -3, -10), new Vector3(28, -3, 0), new Vector3(35, 0.7f, -10), 28, 35); //Hill 1 slope
            CollisionDetectionBox.AddBox(new Vector3(35, -2, -10), new Vector3(58, 0.7f, 0)); //Top of hill 1

            CollisionDetectionBox.AddBox(new Vector3(62.8f, -2, -10), new Vector3(65, 0.7f, 0)); //Bottom of hill 2
            CollisionDetectionPlane.AddPlane(new Vector3(65, 0.7f, -10), new Vector3(65, 0.7f, 0), new Vector3(67, 1.8f, -10), 65, 67); //Hill 2 slope
            CollisionDetectionPlane.AddPlane(new Vector3(67, 2, -10), new Vector3(67, 2, 0), new Vector3(69, 3, -10), 67, 69); //Hill 2 slope
            CollisionDetectionPlane.AddPlane(new Vector3(69, 3, -10), new Vector3(69, 3, 0), new Vector3(73, 3, -10), 69, 73); //Hill 2 slope

            CollisionDetectionBox.AddBox(new Vector3(73, 3, -10), new Vector3(74, 4.5f, 0)); //Start of tree
            CollisionDetectionPlane.AddPlane(new Vector3(74, 4.5f, -10), new Vector3(74, 4.5f, 0), new Vector3(80, 2.8f, -10), 74, 80); //Middle of tree
            CollisionDetectionPlane.AddPlane(new Vector3(80, 2.6f, -10), new Vector3(80, 2.6f, 0), new Vector3(89, 0.9f, -10), 80, 88); //Middle of tree
            CollisionDetectionPlane.AddPlane(new Vector3(88, 0.9f, -10), new Vector3(88, 0.9f, 0), new Vector3(91, 0.6f, -10), 88, 91); //Middle of tree
            CollisionDetectionPlane.AddPlane(new Vector3(91, 0.6f, -10), new Vector3(91, 0.6f, 0), new Vector3(93, 0.6f, -10), 91, 93); //Middle of tree
            CollisionDetectionPlane.AddPlane(new Vector3(93, 0.6f, -10), new Vector3(93, 0.6f, 0), new Vector3(94, 1.1f, -10), 93, 94); //End of tree
            CollisionDetectionBox.AddBox(new Vector3(93, -2, -10), new Vector3(94.5f, 0.9f, 0)); //End of tree

            CollisionDetectionBox.AddBox(new Vector3(95, -5, -10), new Vector3(103, 0, 0)); //End of tree
        }

    }
}