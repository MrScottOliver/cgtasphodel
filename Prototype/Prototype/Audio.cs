using System;
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
        static Cue TitleCue, ForestCue, PianoCue, AcousticCue;
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
            TitleCue = Sound_Bank.GetCue("Title_Song");
            ForestCue = Sound_Bank.GetCue("Dark_Forest");
            PianoCue = Sound_Bank.GetCue("Piano_Theme");
            AcousticCue = Sound_Bank.GetCue("Acoustic");
        }

       public static void Init()
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
            TitleCue = Sound_Bank.GetCue("Title_Song");
            ForestCue = Sound_Bank.GetCue("Dark_Forest");
            PianoCue = Sound_Bank.GetCue("Piano_Theme");
            AcousticCue = Sound_Bank.GetCue("Acoustic");
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
            if (!TitleCue.IsPlaying)
            {
                if (ForestCue.IsPlaying)
                    ForestCue.Stop(AudioStopOptions.Immediate);
                if (AcousticCue.IsPlaying)
                    AcousticCue.Stop(AudioStopOptions.Immediate);
                if (PianoCue.IsPlaying)
                    PianoCue.Stop(AudioStopOptions.Immediate);

                TitleCue = Sound_Bank.GetCue("Title_Song");
                TitleCue.Play();
            }
        }

        public static void Dark()
        {
            if (!ForestCue.IsPlaying)
            {
                if (TitleCue.IsPlaying)
                    TitleCue.Stop(AudioStopOptions.Immediate);
                if (AcousticCue.IsPlaying)
                    AcousticCue.Stop(AudioStopOptions.Immediate);
                if (PianoCue.IsPlaying)
                    PianoCue.Stop(AudioStopOptions.Immediate);

               ForestCue = Sound_Bank.GetCue("Dark_Forest");
                ForestCue.Play(); 
            }
        }

        public static void Acoustic()
        {
            if (!AcousticCue.IsPlaying)
            {
                if (ForestCue.IsPlaying)
                    ForestCue.Stop(AudioStopOptions.Immediate);
                if (TitleCue.IsPlaying)
                    TitleCue.Stop(AudioStopOptions.Immediate);
                if (PianoCue.IsPlaying)
                    PianoCue.Stop(AudioStopOptions.Immediate);

                AcousticCue = Sound_Bank.GetCue("Acoustic");
                AcousticCue.Play();
            }

        }

        public static void Piano()
        {
            if (!PianoCue.IsPlaying)
            {
                if (ForestCue.IsPlaying)
                    ForestCue.Stop(AudioStopOptions.Immediate);
                if (AcousticCue.IsPlaying)
                    AcousticCue.Stop(AudioStopOptions.Immediate);
                if (TitleCue.IsPlaying)
                    TitleCue.Stop(AudioStopOptions.Immediate);

                PianoCue = Sound_Bank.GetCue("Piano_Theme");
                PianoCue.Play();
            }
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
