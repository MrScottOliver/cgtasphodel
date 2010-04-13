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
using System.Diagnostics;

namespace Prototype
{
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
            sphere.Center.Y += 20;//model is under collision point
            //sphere.Center.X -= 2;//model is behind collision point
            sphere.Radius = 5;//set radius
            Current = LifeCycle.Collision;
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
                    Matrix scale;
                    scale = Matrix.Identity; ;
                    scale = Matrix.CreateScale(0.1f, 0.1f, 0.1f);
                    effect.World = /*gameWorldRotation * */ scale * transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position); /*scale*/
                }
                mesh.Draw();
            }
        }
        override
        public bool Collision(BoundingSphere PlayerSphere)
        {
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
                    if (Position.Y < -5)                    //call animate function
                        Position.Y += 0.1f;
                    else
                        Current = LifeCycle.Active;         //function sets current to Active after use is spent
                    break;
                case LifeCycle.Collision:
                    if (PlayerSphere.Intersects(sphere))
                    {
                        KeyboardState keyState = Keyboard.GetState();
                        if (keyState.IsKeyDown(Keys.X))
                        {
                            Current = LifeCycle.Animate;
                            Audio.Growth();
                        }
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
}
