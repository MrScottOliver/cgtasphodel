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

///Stefen: This class will handle interaction between the character and environment
///Note: This is in no way finished



namespace Graphics_Code_SO
{
    interface EventsInterface
    {
     //BoundingSphere sphere;
      void Activate();
    }

    class CustomEvent : EventsInterface
    {
        BoundingSphere sphere;
        //jess: Sets position and radius for event bounding sphere. :) 
        public void SetBoundingSphere(int x,int y,int z,int r)
        {
            //sphere.Center = Vector3(x,y,z);
            sphere.Radius = r;

        }

        public void Activate()
        {
            //translate box by something
            
        }

        //Jess: loops through list of events and checks for intersection with the player
        class EventsController
        {

            public static List<CustomEvent> GameEvents;
           public static List<CustomEvent>.Enumerator iterator;

            public void CheckBounding(Player player)
            {
            foreach( CustomEvent Ev in GameEvents)
            {
              //  if (player.bounding.intersect(Ev.sphere))
               //     Ev.Activate();
        }
            }


        }
    }
}
//check if player is undisposed, check if player is close enough to nearest context sensitive object, check if object is 
//ready for usage, 

//If activated- set object to inactive, set player to inactive for however long the action takes,( animate player and object),
//sound effect

//if activated while impossible, 