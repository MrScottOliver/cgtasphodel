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



namespace Prototype
{
    interface EventsInterface
    {
        bool getStatus();//Jess:get whether event is active
        int whichEvent();//used to determine
      BoundingSphere GetBoundingSphere();
      void Activate();
    }

    //growing flower event
    class GrowEvent : EventsInterface
    {
        BoundingSphere sphere;
        bool active;
        int thisEvent;

        //jess: Sets position and radius for event bounding sphere. Also set active to true/false :) 
        public void SetEvent(int x,int y,int z,int r, bool act)
        {
          sphere.Center =new Vector3(x,y,z);//set position
          sphere.Radius = r;//set radius
          active = act;//set active bool
          thisEvent = 1;//set event indicator

        }

        public void Activate()
        {

            //create box & platforms
            //translate box so it grows up...while loop perhaps
            //set active to false
            
        }

       public BoundingSphere GetBoundingSphere()//return bounding sphere
        {
            return sphere;
        }

       public bool getStatus()//return whether active
       {
           return active;
       }

       public int whichEvent()//return event indicator
       {
           return thisEvent;
       }
    }

    //Jess: running past & watering flowers event
    class FlowerEvent : EventsInterface
    {
        BoundingSphere sphere;
        bool active;
        int thisEvent;

        //jess: Sets position and radius for event bounding sphere. Also set active to true/false :) 
        public void SetEvent(int x, int y, int z, int r, bool act)
        {
            sphere.Center = new Vector3(x, y, z);//set position
            sphere.Radius = r;//set radius
            active = act;//set active bool
            thisEvent = 2;//set event indicator

        }

        public void Activate()
        {

            //animate model
            //animate flower
            //activate grey to colour change
            //setactive to false

        }

        public BoundingSphere GetBoundingSphere()//return bounding sphere
        {
            return sphere;
        }

        public bool getStatus()//return whether active
        {
            return active;
        }

        public int whichEvent()//return event indicator
        {
            return thisEvent;
        }
    }




        //Jess: loops through list of events and checks for intersection with the player
        class EventsController
        {

            public static List<EventsInterface> GameEvents;
           public static List<EventsInterface>.Enumerator iterator;

            public void CheckBounding(Player player, int anEvent)
            {
            foreach( EventsInterface Ev in GameEvents)
            {
                //if (Ev.whichEvent == anEvent)//only loop through specific type of event i.e. growevent =1 and flowerevent = 2
                   //if (Ev.getStatus() == true)
              //  if (player.bounding.intersect(Ev.GetBoundingSphere()))
               //     Ev.Activate();
             }
            }


         }
    
}
//check if player is undisposed, check if player is close enough to nearest context sensitive object, check if object is 
//ready for usage, 

//If activated- set object to inactive, set player to inactive for however long the action takes,( animate player and object),
//sound effect

//if activated while impossible, 