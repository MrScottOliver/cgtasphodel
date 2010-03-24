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
    interface IObject
    {
        void Load();
        void Render(Matrix view, Matrix projection, GraphicsDevice graphics);
        bool Collision(BoundingSphere PlayerSphere);
    }

    interface IInteraction
    {

        void Activate();
    }

    //Stefen: Class derived from relevent intefraces
    abstract class Level : IObject, IInteraction
    {
        public abstract void Load();
        public abstract void Render(Matrix view, Matrix projection, GraphicsDevice graphics);
        public abstract bool Collision(BoundingSphere PlayerSphere);
        public abstract void Activate();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Placeholder class
    class Surface : Level
    {
        public Surface(Model model)
        {
            Model ObjModel = model;
        }

        override
        public void Load()
        {
            Debug.WriteLine("Load Surface");
        }
        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            //Render
        }
        override
        public bool Collision(BoundingSphere PlayerSphere)
        {
            return false;
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
    //Placeholder class
    class Platform : Level
    {
        public Platform(Model model)
        {
            Model ObjModel = model;
        }

        override
        public void Load()
        {
            Debug.WriteLine("Load Platform");
        }
        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            //Render
        }
        override
        public bool Collision(BoundingSphere PlayerSphere)
        {
            //Effect of collision
            return false;
        }
        override
        public void Activate()
        {
            // check collision
            //Effect of activation
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Growable plants
    class Plant : Level
    {
        public enum LifeCycle
        {
            Active,
            Animate,
            Collision
        }

        BoundingSphere sphere;
        Model ObjModel;
        private Vector3 Position;
        LifeCycle Current;

        public Plant(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            sphere.Center = Pos;//set position
            sphere.Center.Z += 5;//model is behind collision point
            sphere.Radius = 1;//set radius
            Current = LifeCycle.Collision;
        }

        override
        public void Load()
        {
            Debug.WriteLine("Load Level");
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
                    effect.World = /*gameWorldRotation * */ transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position);
                }
                mesh.Draw();
            }
        }
        override
        public bool Collision(BoundingSphere PlayerSphere)
        {
            // Effect of collision//designed to call attention, this version should not be used
            //
            //if the boxes collide                // run plant and grow event on different levels, delete event once activated
            //if the object hasnt been activated  // create activation list, animate list and destruction list
            //                                    // other: in here, case active, case animate, collision
            //      
            switch (Current)
            {
                case LifeCycle.Active:
                    //call generic function
                    break;
                case LifeCycle.Animate:
                    if (Position.Y < 20)                    //call animate function
                        Position.Y += 0.1f;
                    else
                        Current = LifeCycle.Active;         //function sets current to Active after use is spent
                    break;
                case LifeCycle.Collision:
                    if (PlayerSphere.Intersects(sphere))
                    {
                        KeyboardState keyState = Keyboard.GetState();
                        if (keyState.IsKeyDown(Keys.X))
                            Current = LifeCycle.Animate;
                    }
                    break;
                default:
                    throw new ArgumentException("Error - " + Current + " is not recognized.");
            }



            return false;
        }

        override
        public void Activate()
        {

        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Pick-ups
    class Orb : Level
    {
        BoundingSphere sphere;
        Model ObjModel;
        private Vector3 Position;

        public Orb(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            sphere.Center = Pos;//set position
            sphere.Radius = 1;//set radius
        }

        override
        public void Load()
        {
            //  model
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
                    effect.World = /*gameWorldRotation * */ transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position);
                }
                mesh.Draw();
            }

        }
        override
        public bool Collision(BoundingSphere PlayerSphere)
        {
            if (PlayerSphere.Intersects(sphere))
            {
                //increment points
                //remove orb from list
                return true;
            }
            else
                return false;
        }
        override
        public void Activate()
        {
            // check collision
            //Effect of activation
        }
        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Stefen: Object factory class returns a new instance of a class
    class ObjectFactory
    {
        public enum ObjectType
        {
            Surface,
            Platform,
            Plant,
            Orb
        }

        public static IObject createObject(ObjectType item, Model model, Vector3 Position)
        {

            switch (item)
            {
                case ObjectType.Surface:
                    return new Surface(model);

                case ObjectType.Platform:
                    return new Platform(model);

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
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            foreach (IObject item in ObjectList)
            {
                //ObjectFactory.createObject(item).Render();
                item.Render(view, projection, graphics);
            }
        }
        //If a collision takes place the associated effect is activated
        override
        public bool Collision(BoundingSphere PlayerSphere) //Collision returns true if object is to be destroyed
        {
            List<IObject> itemsToRemove = new List<IObject>();
            foreach (IObject item in ObjectList)
            {
                if (item.Collision(PlayerSphere))
                {
                    itemsToRemove.Add(item);
                    Audio.Pickup();
                }
                // alternative: have seperate function for effect
                // if (item.Collision(PlayerSphere))
                //      item.Activate();
            }
            foreach (IObject itemToRemove in itemsToRemove)
                ObjectList.Remove(itemToRemove);

            return false;
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
}