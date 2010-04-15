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
namespace Prototype
{
    public enum Actions
    {
        Scale,
        Rotate,
        Position
    }

        public enum ObjectType
        {
            Platform,
            Plant,
            Orb
        }
    interface IObject
    {
        void Load(Actions State, float Val1, float Val2, float Val3);
        void Render(Matrix view, Matrix projection, GraphicsDevice graphics);
        void Collision(BoundingSphere PlayerSphere);
        void Activate();
    }

    //Stefen: Class derived from relevent intefraces
    abstract class Level : IObject
    {
        public abstract void Load(Actions State, float Val1, float Val2, float Val3);
        public abstract void Render(Matrix view, Matrix projection, GraphicsDevice graphics);
        public abstract void Collision(BoundingSphere PlayerSphere);
        public abstract void Activate();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Placeholder class
    class Platform : Level
    {
        Model ObjModel;
        private Vector3 Position;


        public Platform(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
        }

        override
        public void Load(Actions State, float Val1, float Val2, float Val3)
        {
            switch (State)
            {
                case Actions.Scale:
                    //Scale(Val1, Val2, Val3);
                    break;
                case Actions.Rotate:
                    //Rotate(Val1, Val2, Val3);
                    break;
                case Actions.Position:
                    break;
            }
        }
        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            Matrix[] transforms = new Matrix[ObjModel.Bones.Count];
            ObjModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in ObjModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    //// effect.Parameters[""]
                    effect.View = view;
                    effect.Projection = projection;
                    Matrix scale, rotation;
                    scale = Matrix.Identity; ;
                    scale = Matrix.CreateScale(0.1f, 0.1f, 0.1f);
                    rotation = Matrix.Identity; 
                    rotation *= Matrix.CreateFromYawPitchRoll(0, 90, 0);
                    effect.World = /*gameWorldRotation * */ transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position)* rotation * scale; /*scale*/
                }
                mesh.Draw();
            }
        }
        override
        public void Collision(BoundingSphere PlayerSphere)
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
                case ObjectType.Platform:
                    return new Platform(model, Position);

                case ObjectType.Plant:
                    return new Plant(model, Position);

                case ObjectType.Orb:
                    return new Orb(model, Position);
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
        public void Load(Actions State, float Val1, float Val2, float Val3)
        {
            foreach (IObject item in ObjectList)
            {
                //ObjectFactory.createObject(item).Load();
                //item.Load();
            }
        }
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
        public void Collision(BoundingSphere PlayerSphere) //Collision returns true if object is to be destroyed
        {
            itemsToRemove = new List<IObject>();
            foreach (IObject item in ObjectList)
            {
                item.Collision(PlayerSphere);
            }
            foreach (IObject itemToRemove in itemsToRemove)
                ObjectList.Remove(itemToRemove);
        }
        override
        public void Activate()
        {
            foreach (IObject item in ObjectList)
            {
                //ObjectFactory.createObject(item).Collision();
                item.Activate();
            }
        }
    }
}