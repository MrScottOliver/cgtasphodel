﻿using System;
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

namespace Prototype
{
    public sealed class Audio
    {
        //Stefen: Audio objects
        static AudioEngine Engine;
        static SoundBank Sound_Bank;
        static WaveBank Player_Effects, Sound_Effects, Music;
        static Cue FootstepCue, SlideCue, JumpCue, PickupCue, GrowthCue;
        static Cue TitleCue, ForestCue, PianoCue;
        //Stefen:  3D audio controls
        static AudioEmitter Emitter = new AudioEmitter();
        static AudioListener Listener = new AudioListener();

        //Stefen: Loads Xact Files
        private Audio()
        {       
            Engine = new AudioEngine("Content\\Audio\\Prototype.xgs");
            Sound_Bank = new SoundBank(Engine, "Content\\Audio\\Sound_Bank.xsb");
            Player_Effects = new WaveBank(Engine, "Content\\Audio\\Player_Effects.xwb");
            Sound_Effects = new WaveBank(Engine, "Content\\Audio\\SFX.xwb");
            Music = new WaveBank(Engine, "Content\\Audio\\Music.xwb");
            FootstepCue = Sound_Bank.GetCue("Player_Footstep");
            SlideCue = Sound_Bank.GetCue("Slide");
            JumpCue = Sound_Bank.GetCue("Jump");
            PickupCue = Sound_Bank.GetCue("Orb_Pickup");
            GrowthCue = Sound_Bank.GetCue("Plant_Growth");
        }

       public static void Init()
        {
            Engine = new AudioEngine("Content\\Audio\\Prototype.xgs");
            Sound_Bank = new SoundBank(Engine, "Content\\Audio\\Sound_Bank.xsb");
            Player_Effects = new WaveBank(Engine, "Content\\Audio\\Player_Effects.xwb");
            Sound_Effects = new WaveBank(Engine, "Content\\Audio\\SFX.xwb");
            // Music = new WaveBank(Engine, "Content\\Audio\\Music.xwb");
            FootstepCue = Sound_Bank.GetCue("Player_Footstep");
            SlideCue = Sound_Bank.GetCue("Slide");
            JumpCue = Sound_Bank.GetCue("Jump");
            PickupCue = Sound_Bank.GetCue("Orb_Pickup");
            GrowthCue = Sound_Bank.GetCue("Plant_Growth");
        }

        //Stefen: Plays a footstep sound
        public static void Step() 
        {
            if (!FootstepCue.IsPlaying)
                FootstepCue.Play();
            FootstepCue = Sound_Bank.GetCue("Player_Footstep"); 
        }

        public static void Slide()
        {
            if (!SlideCue.IsPlaying)
                SlideCue.Play();
            SlideCue = Sound_Bank.GetCue("Slide");
        }

        public static void Jump()
        {
            if (!JumpCue.IsPlaying)
                JumpCue.Play();
            JumpCue = Sound_Bank.GetCue("Jump");
        }

        public static void Pickup()
        {
            if (!PickupCue.IsPlaying)
                PickupCue.Play();
            PickupCue = Sound_Bank.GetCue("Orb_Pickup");
        }

        public static void Growth()
        {
            if (!GrowthCue.IsPlaying)
                GrowthCue.Play();
            GrowthCue = Sound_Bank.GetCue("Plant_Growth");
        }

        public static void TitleSong()
        {
            if (!GrowthCue.IsPlaying)
                GrowthCue.Play();
            GrowthCue = Sound_Bank.GetCue("Plant_Growth");
        }

        public static void Update(Vector3 EmitterPosition, Vector3 ListenerPosition)
        {
            //Stefen: Updates Audio
            Emitter.Position = EmitterPosition;
            Listener.Position = ListenerPosition;
            FootstepCue.Apply3D(Listener, Emitter);
            SlideCue.Apply3D(Listener, Emitter);
            JumpCue.Apply3D(Listener, Emitter);
            Engine.Update();
        }
    }
}