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

namespace Graphics_Code_SO
{
    class Audio
    {
        //Stefen: Audio objects
        AudioEngine Engine;
        SoundBank Sound_Bank;
        WaveBank Player_Effects;
        Cue Footstep;

        //Stefen:  3D audio controls
        AudioEmitter Emitter = new AudioEmitter();
        AudioListener Listener = new AudioListener();

        //Stefen: Loads Xact Files
        public Audio()
        {       
            Engine = new AudioEngine("Content\\Audio\\Prototype.xgs");
            Sound_Bank = new SoundBank(Engine, "Content\\Audio\\Sound_Bank.xsb");
            Player_Effects = new WaveBank(Engine, "Content\\Audio\\Player_Effects.xwb");
            Footstep = Sound_Bank.GetCue("Player_Footstep");
        }

        //Stefen: Plays a footstep sound
        public void Step() 
        {
            if(!Footstep.IsPlaying)
                Footstep.Play();
            Footstep = Sound_Bank.GetCue("Player_Footstep"); 
        }

        public void Update(Vector3 EmitterPosition, Vector3 ListenerPosition)
        {
            //Stefen: Updates Audio
            Emitter.Position = EmitterPosition;
            Listener.Position = ListenerPosition;
            Footstep.Apply3D(Listener, Emitter);
            Engine.Update();
        }
    }
}
