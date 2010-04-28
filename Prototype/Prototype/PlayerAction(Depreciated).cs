//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

/////Stefen: This class will handle interaction between the character and environment
/////Note: This is in no way finished



//namespace Prototype
//{
//    interface EventsInterface
//    {
//        bool getStatus();//Jess:get whether event is active
//        int whichEvent();//used to determine
//      BoundingSphere GetBoundingSphere();
//      void Activate();
//    }

//    //growing flower event
//    class GrowEvent : EventsInterface
//    {
//        BoundingSphere sphere;
//        bool Available;
//        bool Active = false;
//        public bool Activated = false;
//        int thisEvent;
//        Vector3 Position;
//        public float radius { get; set; }
//        public float minY { get; set; }
//        public float maxY { get; set; }

//        //jess: Sets position and radius for event bounding sphere. Also set active to true/false :) 
//        public void SetEvent(int x,int y,int z,int r, bool available)
//        {
//            Position = new Vector3(x, y, z);
//            sphere.Center = Position;//set position
//            sphere.Radius = r;//set radius
//            Available = available;//set available bool
//            thisEvent = 1;//set event indicator
//            radius = 0;
//        }

//        public void Activate()
//        {
//            //create box & platforms
//            //translate box so it grows up...while loop perhaps
//            //set available to false
//            Active = true;
//            Available = false;
//            Activated = true;
//        }

//        public void Dectivate()
//        {
//            Active = false;
//        }

//       public BoundingSphere GetBoundingSphere()//return bounding sphere
//        {
//            return sphere;
//        }

//       public bool getStatus()//return whether available for interaction
//       {
//           return Available;
//       }
//       //stefen:[notes to self] used to determin if animation should run
//       public bool getActive()//return whether or not is active
//       {
//           return Active;
//       }

//       public int whichEvent()//return event indicator
//       {
//           return thisEvent;
//       }

//       public Vector3 getPos()
//       {
//           return Position;
//       }
//        //stefen:[notes to self] object manipulator 
//        //center point of grow event, think its for expanding radius effect
//        //:start
        
//       public float getX()
//       {
//           return Position.X;
//       }
//       public float getY()
//       {
//           return Position.Y;
//       }
//       public float getZ()
//       {
//           return Position.Z;
//       }
//       //:end
//       public void Animate(Int32 newy)
//       {
//           if (Position.Y <= newy)
//               Position.Y += 0.1F;
//           else
//               Active = false;

//           if (radius < 100)
//           {
//               radius += 0.1f;
//           }
//           if (maxY < 20)
//           {
//               maxY += 0.1f;
//           }
//           if (minY > -5)
//           {
//               minY -= 0.1f;
//           }
//       }

//       public void Transform()
//       {
//           if (radius < 150)
//           {
//               radius += 0.5f;
//           }
//           if (maxY < 20)
//           {
//               maxY += 0.5f;
//           }
//           if (minY > -5)
//           {
//               minY -= 0.5f;
//           }
//       }

//    }

//    //Jess: running past & watering flowers event
//    class FlowerEvent : EventsInterface
//    {
//        BoundingSphere sphere;
//        bool Available;
//        int thisEvent;

//        //jess: Sets position and radius for event bounding sphere. Also set active to true/false :) 
//        public void SetEvent(int x, int y, int z, int r, bool available)
//        {
//            sphere.Center = new Vector3(x, y, z);//set position
//            sphere.Radius = r;//set radius
//            Available = available;//set active bool
//            thisEvent = 2;//set event indicator

//        }

//        public void Activate()
//        {

//            //animate model
//            //animate flower
//            //activate grey to colour change
//            //set available to false

//        }

//        public BoundingSphere GetBoundingSphere()//return bounding sphere
//        {
//            return sphere;
//        }

//        public bool getStatus()//return whether available for interaction
//        {
//            return Available;
//        }

//        public int whichEvent()//return event indicator
//        {
//            return thisEvent;
//        }
//    }




//        //Jess: loops through list of events and checks for intersection with the player
//        class EventsController
//        {

//            public static List<EventsInterface> GameEvents;
//           public static List<EventsInterface>.Enumerator iterator;

//            public void CheckBounding(Player player, int anEvent)
//            {
//            foreach( EventsInterface Ev in GameEvents)
//            {
//                //if (Ev.whichEvent == anEvent)//only loop through specific type of event i.e. growevent =1 and flowerevent = 2
//                   //if (Ev.getStatus() == true)
//              //  if (player.bounding.intersect(Ev.GetBoundingSphere()))
//               //     Ev.Activate();
//             }
//            }


//         }
    
//}
////check if player is undisposed, check if player is close enough to nearest context sensitive object, check if object is 
////ready for usage, 

////If activated- set object to inactive, set player to inactive for however long the action takes,( animate player and object),
////sound effect

////if activated while impossible, 