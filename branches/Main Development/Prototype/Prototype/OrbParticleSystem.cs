﻿#region File Description
//===================================================================
// DefaultPointSpriteParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Point Sprite Particle
// System that inherits from the Default Point Sprite Particle System.
//
// The spots that should be modified are marked with TODO statements.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    class OrbParticleSystem : DefaultPointSpriteParticleSystem
    {
        public Vector3 Pos;//Stefen: thrown this in to ease positioning
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public OrbParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any Particle System properties here
        //-----------------------------------------------------------

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any overridden Particle System functions here
        //-----------------------------------------------------------

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize the Particle System with default values
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device to draw to</param>
        /// <param name="cContentManager">The Content Manager to use to load Textures and Effect files</param>
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
        {
            //-----------------------------------------------------------
            // TODO: Change any Initialization parameters desired and the Name
            //-----------------------------------------------------------
            // Initialize the Particle System before doing anything else
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000, 
                                                UpdateVertexProperties, "Star9");

            // Set the Name of the Particle System
            Name = "Default Point Sprite Particle System Template";

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadParticleSystem()
        {
            //-----------------------------------------------------------
            // TODO: Setup the Particle System to achieve the desired result.
            // You may change all of the code in this function. It is just
            // provided to show you how to setup a simple particle system.
            //-----------------------------------------------------------

            // Set the Function to use to Initialize new Particles.
            // The Default Templates include a Particle Initialization Function called
            // InitializeParticleUsingInitialProperties, which initializes new Particles
            // according to the settings in the InitialProperties object (see further below).
            // You can also create your own Particle Initialization Functions as well, as shown with
            // the InitializeParticleProperties function below.
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
            //ParticleInitializationFunction = InitializeParticleProperties;

            // Setup the Initial properties of the Particles.
            // These are only applied if using InitializeParticleUsingInitialProperties 
            // as the Particle Initialization Function.
            InitialProperties.LifetimeMin = 5.0f;
            InitialProperties.LifetimeMax = 5.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.VelocityMin = new Vector3(-3f, -3f, -3f);
            InitialProperties.VelocityMax = new Vector3(3f, 3f, 3f);
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.Pi;
            InitialProperties.RotationalVelocityMin = -MathHelper.Pi;
            InitialProperties.RotationalVelocityMax = MathHelper.Pi;
            InitialProperties.StartSizeMin = 1.0f;
            InitialProperties.StartSizeMax = 2.0f;
            InitialProperties.EndSizeMin = 3.0f;
            InitialProperties.EndSizeMax = 3.0f;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Position, Rotation, Size, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            // Set the Particle System's Emitter to toggle on and off every 0.5 seconds
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 2.0f;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 50;
            Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultPointSpriteParticle cParticle)
        {
            //-----------------------------------------------------------
            // TODO: Initialize all of the Particle's properties here.
            // If you plan on simply using the default InitializeParticleUsingInitialProperties
            // Particle Initialization Function (see the LoadParticleSystem() function above), 
            // then you may delete this function all together.
            //-----------------------------------------------------------

            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            Vector3 sVelocityMin = new Vector3(-50, 50, -50);
            Vector3 sVelocityMax = new Vector3(50, 100, 50);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = RandomNumber.Next(10, 50);

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle Update functions here, using the 
        // same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
        //-----------------------------------------------------------

        /// <summary>
        /// Example of how to create a Particle Event Function
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleFunctionExample(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle here
            // Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle System Update functions here, using 
        // the same function prototype as below (i.e. public void FunctionName(float))
        //-----------------------------------------------------------

        /// <summary>
        /// Example of how to create a Particle System Event Function
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemFunctionExample(float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle System here
            // Example: Emitter.EmitParticles = true;
            // Example: SetTexture("TextureAssetName");
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any other functions here
        //-----------------------------------------------------------
        
       
    }
    //Stefen:attempting a list based way to create multiple effects
    /* class ParticleGroup
     {
        public static List<OrbParticleSystem> ParticleList;
         OrbParticleSystem mcOrbParticleSystem;

        public void NewParticle(Vector3 Position)
        {
            mcOrbParticleSystem = new OrbParticleSystem(null);
            mcOrbParticleSystem.AutoInitialize(this.GraphicsDevice, this.Content);
            mcOrbParticleSystem.Pos = Position;
        }

         public void Update()
         {
        mcOrbParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
         }

         public void Draw(Matrix World, Matrix View, Matrix Proj)
         {
           mcOrbParticleSystem.SetWorldViewProjectionMatrices(World * Matrix.CreateTranslation(mcOrbParticleSystem.Pos), View, Proj);
           mcOrbParticleSystem.Draw();}
         }
    */
}
