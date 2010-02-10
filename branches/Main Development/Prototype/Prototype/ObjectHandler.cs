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
interface IObject
{
    void Load();
    void Render();
}

interface IInteraction
{
    void Collision();
    void Activate();
}

//Stefen: Class derived from relevent intefraces
abstract class Level : IObject, IInteraction
{
    public abstract void Load();
    public abstract void Render();
    public abstract void Collision();
    public abstract void Activate();
}

class Surface : Level
{
    override
    public void Load()
    {
        Debug.WriteLine("Load Surface");
    }
    override
    public void Render()
    {
        //Render
    }
    override
    public void Collision()
    {
        //if check collision
        //Effect of collision
    }
    override
    public void Activate()
    {
        // check collision
        //Effect of activation
    }
}

class Platform : Level
{
    override
    public void Load()
    {
        Debug.WriteLine("Load Platform");
    }
    override
    public void Render()
    {
        //Render
    }
    override
    public void Collision()
    {
        //Effect of collision
    }
    override
    public void Activate()
    {
        // check collision
        //Effect of activation
    }
}

class Plant : Level
{
    override
    public void Load()
    {
        Debug.WriteLine("Load Level");
    }
    override
    public void Render()
    {
        //Render
    }
    override
    public void Collision()
    {
        //Effect of collision
    }
    override
    public void Activate()
    {
        // check collision
        //Effect of activation
    }
}

//Stefen: Object factory class returns a new instance of a class
class ObjectFactory
{
    public enum ObjectType
    {
        Surface,
        Platform,
        Plant
    }

    public static IObject createObject(ObjectType item)
    {
        switch (item)
        {
            case ObjectType.Surface:
                return new Surface();
            case ObjectType.Platform:
                return new Platform();
            case ObjectType.Plant:
                return new Plant();
        }
        throw new ArgumentException("Error - " + item + " is not recognized.");
    }
}

//Stefen: Object control class contains the list of objects and the ability to loop through each interfaced function
class ObjectControl : Level
{
    public static List<IObject> ObjectList = new List<IObject>();
    
    override
    public void Load()
    {
        foreach (IObject item in ObjectList)
        {
            //ObjectFactory.createObject(item).Load();
            item.Load();
        }
    }
    override
    public void Render()
    {
        foreach (IObject item in ObjectList)
        {
            //ObjectFactory.createObject(item).Render();
            item.Render();
        }
    }
    override
    public void Collision()
    {
        foreach (IInteraction item in ObjectList)
        {
            //ObjectFactory.createObject(item).Collision();
            item.Collision();
        }
    }
    override
    public void Activate()
    {
        foreach (IInteraction item in ObjectList)
        {
            //ObjectFactory.createObject(item).Collision();
            item.Activate();
        }
    }
}
