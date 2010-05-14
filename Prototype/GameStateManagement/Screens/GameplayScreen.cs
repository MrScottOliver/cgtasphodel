#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using DPSF;
using DPSF.ParticleSystems;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

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
        Surface FullLevel1, FullLevel2;
        float YAW, PITCH, ROLL;
        Matrix View;
        Matrix Proj;
        Matrix World = Matrix.CreateTranslation(0, 0, 0);
        Shadows myShadows = new Shadows();

        float OrbGlow = 2.3f;//variables for making the orb glow pulsate
        int pulsate = 1;
        int orbnum = 0;
        Vector3[] OrbPositions = new Vector3[20];

        PostProcess BloomEffect;//bloom post process effect 

        int numlights = 0; //number of point lights

        private float aspectRatio;
        private float FOV;
        private float nearClip;
        private float farClip;

        private float transformDegree = 1.0f;
        private bool revealTransform = false;

        PhysicsActor paBall1;
        PhysicsActor paBall2;
        PhysicsActor paBall3;
        PhysicsActor paBall4;

        //New physics stuff
        Physics physicSystem;

       
        //////////////////////////

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            POS = new Vector3(0.0f, 10.0f, 25.0f);
            TARGET = new Vector3(0.0f, 5.0f, 1.0f);
            UP = Vector3.Up;

            YAW = PITCH = ROLL = 0.0f;

            aspectRatio = ScreenManager.GraphicsDevice.Viewport.AspectRatio;
            FOV = 60.0f;
            nearClip = 1.0f;
            farClip = 1000.0f;

            Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), aspectRatio, nearClip, farClip);
            Audio.Init();

            ParticleGroup.Init(ScreenManager.GraphicsDevice, content);
            MushroomParticleGroup.Init(ScreenManager.GraphicsDevice, content);

            //adds the collision boxes
            SetupPlane();

            //Jess: load effect file
            myEffect = content.Load<Effect>("Effects/Lighting");

            //Kieran: set player model
            player.model = content.Load<Model>("models/REcharacter");
            player.texture = content.Load<Texture2D>("models/metal");

            player.RemapModel(player, myEffect);//remap model to use our effect

            player.AddRotation((float)Math.PI / 2, 0.0f, 0.0f);
            player.ChangeScale(0.05f);
            player.AddTranslation(-7f, 30f, 0f);

            SetUpSky();//Jess:set up sky 


            Model OrbModel = content.Load<Model>("models/ball");
            Model PlantCyl = content.Load<Model>("models/Flower1");
            Model Full1 = content.Load<Model>("models/LevelBuild7");
            Model Full2 = content.Load<Model>("models/LevelBuild6");
            Model Mush1 = content.Load<Model>("models/MurshroomPlatformtest");
            Model Mush2 = content.Load<Model>("models/MurshroomPlatformtest2");
            Model Leaf1 = content.Load<Model>("models/leave1");
  

            RemapModel(OrbModel, myEffect);
            RemapModel(PlantCyl, myEffect);
            RemapModel(Full1, myEffect);
            RemapModel(Full2, myEffect);
            RemapModel(Mush1, myEffect);
            RemapModel(Mush2, myEffect);
            RemapModel(Leaf1, myEffect);

            FullLevel1 = new Surface(Full1, new Vector3(35.0f, -5.2f, -15.0f));
            FullLevel2 = new Surface(Full2, new Vector3(35.0f, -5.2f, -15.0f));
            FullLevel1.RePosition(new Vector3(140, -6, 0));

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

     

            //Stefen: Apply transformations for last object entered

            ObjectControl.ObjectList.Add(
            ObjectFactory.createObject(ObjectType.Orb, OrbModel, new Vector3(44.5f, 23, 0))
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

                ObjectControl.ObjectList.Add(
              ObjectFactory.createObject(ObjectType.Mushroom, Mush1, new Vector3(190, 3.7f, -4))
            );
            

            myShadows.SetUpShadowBuffer(ScreenManager.GraphicsDevice);

            BloomEffect = FindComponent<PostProcess>();

            Audio.SetMusic(Audio.Tracks.piano);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
            ObjectControl.ObjectList.Clear();
            MushroomParticleGroup.ParticleList.Clear();
            ParticleGroup.ParticleList.Clear();
            Audio.SetMusic(Audio.Tracks.mute);
            

        }


        #endregion

        T FindComponent<T>()
        {
            return ScreenManager.Game.Components.OfType<T>().First();
        }

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {

                if (player.position.X > 200.0f && revealTransform == false)
                {
                    revealTransform = true;
                    Audio.SetMusic(Audio.Tracks.acoustic);

                }

                if (revealTransform && transformDegree > 0.0f)
                {
                    transformDegree -= 0.001f;
                }


                ParticleGroup.Update(gameTime);
                MushroomParticleGroup.Update(gameTime);
                Audio.PlayMusic();
                player.Move();
                CollisionDetectionBox.Compare(ref player);
                CollisionDetectionPlane.Compare(ref player);
                player.Update();
             

                ObjControl.HandleState(player);

                //Stefen: Takes in object and cameras positions to provide 3d sound
                Audio.Update(player.position, POS);

                POS.X = player.position.X + 1.0f;
                POS.Y = player.position.Y + 5.0f;
                TARGET.X = player.position.X + 1.0f;
                TARGET.Y = player.position.Y + 3.0f;

                // Update the animation according to the elapsed time //kev
                //animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);

            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                keyState = Keyboard.GetState();
            // Allows the game to exit
#if !XBOX

            //Kieran: arrow keys move player model
            if (keyState.IsKeyDown(Keys.Up))
            {
                player.Jump();
            }

            if (keyState.IsKeyDown(Keys.Enter))
            {
                //Audio.Slide();
                LoadingScreen.Load(ScreenManager, false, null,
                               new CreditScreen());

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

            if (keyState.IsKeyDown(Keys.E))
            {
                player.health--;
            }

            if (keyState.IsKeyDown(Keys.R))
            {
                player.health++;
            }

           
#endif

               
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);


            ScreenManager.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            //reset depth buffer
            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;

            View = QuaternionCamera.GetViewMatrix(ref POS, ref TARGET, ref UP, YAW, PITCH, ROLL);

            CreateShadowMap();
 

            SetUpLighting();//Jess: set light parameters

            UpdateEffectParams();

            //Kieran: call draw player function
            player.DrawPlayer2(player, Proj, View);

            myEffect.CurrentTechnique = myEffect.Techniques["MyTech"];//Jess: set current tech

            ObjControl.Render(View, Proj, ScreenManager.GraphicsDevice);

            FullLevel1.Render(View, Proj, ScreenManager.GraphicsDevice);
            FullLevel2.Render(View, Proj, ScreenManager.GraphicsDevice);
    
            Background.DrawSky(View, Proj, POS, ScreenManager.GraphicsDevice);

            ParticleGroup.Draw(World, View, Proj);
            MushroomParticleGroup.Draw(World, View, Proj);

            BloomEffect.Draw(gameTime);


            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);


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


            //attach point lights to each visible orb
            numlights = 1;
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

            myShadows.SetUpShadowMap1(myEffect, ScreenManager.GraphicsDevice, View, Proj);

            //ObjectManipulator.Draw(GraphicsDevice, stdEffect, myEffect, View, Proj, POS, lights);
            player.DrawPlayerShadow(player, Proj, View);

            ObjControl.Render(View, Proj, ScreenManager.GraphicsDevice);
            FullLevel1.Render(View, Proj, ScreenManager.GraphicsDevice);
            FullLevel2.Render(View, Proj, ScreenManager.GraphicsDevice);


            myShadows.SetUpShadowMap2(myEffect, ScreenManager.GraphicsDevice);

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

            Background.SkyModel = content.Load<Model>("models/quad");
            Background.SkyTexture = content.Load<Texture2D>("spires_north");
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

            CollisionDetectionBox.AddBox(new Vector3(95, -5, -10), new Vector3(103, 0, 0)); //Platform after tree
            CollisionDetectionPlane.AddPlane(new Vector3(103, 0, -10), new Vector3(103, 0, 0), new Vector3(108, 1.1f, -10), 103, 108); //Uphill after tree - start of second level model
            CollisionDetectionPlane.AddPlane(new Vector3(108, 1.1f, -10), new Vector3(108, 1.1f, 0), new Vector3(111, 0.5f, -10), 108, 111); //Downhill after tree - start of second level model
            CollisionDetectionPlane.AddPlane(new Vector3(111, 0.5f, -10), new Vector3(111, 0.5f, 0), new Vector3(116, -2.3f, -10), 111, 116); //Downhill after tree - start of second level model
            CollisionDetectionBox.AddBox(new Vector3(116, -5, -10), new Vector3(124, -2.3f, 0)); //Platform after hill

            CollisionDetectionBox.AddBox(new Vector3(134.5f, -10, -10), new Vector3(134.6f, 0, 0)); //Start of 2nd platform
            CollisionDetectionPlane.AddPlane(new Vector3(134.6f, 0.0f, -10), new Vector3(134.6f, 0.0f, 0), new Vector3(141, 3.0f, -10), 134.6f, 141); //Uphill 2nd platform
            CollisionDetectionPlane.AddPlane(new Vector3(141, 3.0f, -10), new Vector3(141, 3.0f, 0), new Vector3(144, 3.5f, -10), 141, 144); //Uphill 2nd platform
            CollisionDetectionPlane.AddPlane(new Vector3(144, 3.5f, -10), new Vector3(144, 3.5f, 0), new Vector3(150, 2.9f, -10), 144, 150); //Downhill 2nd platform
            CollisionDetectionBox.AddBox(new Vector3(150, -10, -10), new Vector3(155, 2.9f, 0)); //2nd plat top step
            CollisionDetectionBox.AddBox(new Vector3(155, -10, -10), new Vector3(160.5f, 1.1f, 0)); //2nd plat middle step
            CollisionDetectionBox.AddBox(new Vector3(160.5f, -10, -10), new Vector3(166, -0.7f, 0)); //2nd plat lower middle step
            CollisionDetectionBox.AddBox(new Vector3(166, -10, -10), new Vector3(173, -2.7f, 0)); //2nd plat lowest step

            CollisionDetectionBox.AddBox(new Vector3(181.9f, -10, -10), new Vector3(197.3f, 2.7f, 0)); //3rd plat

            CollisionDetectionBox.AddBox(new Vector3(213.5f, -10, -10), new Vector3(229, 26.3f, 0)); //4th plat

            CollisionDetectionBox.AddBox(new Vector3(236.5f, -10, -10), new Vector3(253.3f, 17.6f, 0)); //5th plat

            CollisionDetectionBox.AddBox(new Vector3(259.3f, -10, -10), new Vector3(276.1f, 10.3f, 0)); //6th plat
        }


        #endregion
    }
}
