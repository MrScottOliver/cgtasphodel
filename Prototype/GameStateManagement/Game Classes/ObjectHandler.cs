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
using System.Diagnostics;

//Stefen: Multiple interfaces are declaired, the interfaces are inherited by a abstract class to group them for purpose. These classes are inherited
//by each game class, attaching the interfaces.
//Stefen: Object factory class returns a new instance of a class
//Stefen: Object control class contains the list of objects and the ability to loop through each interfaced function

//Stefen: General interface for game object
namespace GameStateManagement
{
        public enum ObjectType
        {
            Plant,
            Orb,
            Mushroom,
            Leaf
        }
    interface IObject
    {
        void Render(Matrix view, Matrix projection, GraphicsDevice graphics);
        void HandleState(Player Player);
    }

    //Stefen: Class derived from relevent intefraces
    abstract class Level : IObject
    {
        public abstract void Render(Matrix view, Matrix projection, GraphicsDevice graphics);
        public abstract void HandleState(Player Player);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Stefen: Object factory class returns a new instance of a class
    class ObjectFactory
    {
        public static IObject createObject(ObjectType item, Model model, Vector3 Position)
        {
            switch (item)
            {
                case ObjectType.Plant:
                    return new Plant(model, Position);

                case ObjectType.Orb:
                    return new Orb(model, Position);

                case ObjectType.Mushroom:
                    return new Mushroom(model, Position);

                case ObjectType.Leaf:
                    return new Leaf(model, Position);
            }
            throw new ArgumentException("Error - " + item + " is not recognized.");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Stefen: Object control class contains the list of objects and the ability to loop through each interfaced function
    class ObjectControl : Level
    {
        public static List<IObject> ObjectList = new List<IObject>();
        public static List<IObject> itemsToRemove;

        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            foreach (IObject item in ObjectList)
            {
                item.Render(view, projection, graphics);
            }
        }
        //If a collision takes place the associated effect is activated
        override
        public void HandleState(Player Player) 
        {
            itemsToRemove = new List<IObject>();
            foreach (IObject item in ObjectList)
                item.HandleState(Player);

            foreach (IObject itemToRemove in itemsToRemove)
                ObjectList.Remove(itemToRemove);
        }
    }
}