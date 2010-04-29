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
        public Vector3 Position;
        LifeCycle Current;
        double GrowSpeed;
        float InitialY;
        public float minY, maxY, radius;
        static int counter = 0;
        public int plantnum;

        public Plant(Model model, Vector3 Pos)
        {
            ObjModel = model;
            Position = Pos;
            sphere.Center = Pos;//set position
            sphere.Center.Y += 20;//model is under collision point
            //sphere.Center.X -= 2;//model is behind collision point
            sphere.Radius = 5;//set radius
            Current = LifeCycle.Collision;
            GrowSpeed = 0.5f;
            InitialY = Position.Y;
            plantnum = counter;
            counter++;
            minY = -20;
            maxY = 20;
        }

        override
        public void Render(Matrix view, Matrix projection, GraphicsDevice graphics)
        {
            if (Current != LifeCycle.Collision)
            {
                Matrix[] transforms = new Matrix[ObjModel.Bones.Count];
                ObjModel.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in ObjModel.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        ////// effect.Parameters[""]
                        //effect.View = view;
                        //effect.Projection = projection;
                        //Matrix scale;
                        //scale = Matrix.Identity; ;
                        //scale = Matrix.CreateScale(0.1f, 0.1f, 0.1f);
                        //effect.World = /*gameWorldRotation * */   transforms[mesh.ParentBone.Index] * scale * Matrix.CreateTranslation(Position); /*scale*/

                        Matrix scale;
                        scale = Matrix.Identity; ;
                        scale = Matrix.CreateScale(0.3f, 0.1f, 0.1f);
                        Matrix world = /*gameWorldRotation * */ scale * transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Position); /*scale*/
                        Matrix wvp = world * view * projection;
                        Matrix vp = view * projection;

                        Matrix worldIT = Matrix.Invert(world);
                        worldIT = Matrix.Transpose(worldIT);

                        Vector4 ambMtrl = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);//Jess: default material vals
                        Vector4 diffMtrl = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                        Vector4 specMtrl = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

                        //set matrix params
                        effect.Parameters["gWVP"].SetValue(wvp);
                        effect.Parameters["gWorld"].SetValue(world);
                        effect.Parameters["gViewProj"].SetValue(vp);
                        effect.Parameters["gWorldIT"].SetValue(worldIT);

                        effect.Parameters["gAmbMtrl"].SetValue(ambMtrl);
                        effect.Parameters["gDiffuseMtrl"].SetValue(diffMtrl);
                        effect.Parameters["gSpecMtrl"].SetValue(specMtrl);
                        effect.Parameters["withgrey"].SetValue(true);
                        effect.Parameters["withlights"].SetValue(true);
                        effect.Parameters["player"].SetValue(false);


                    }
                    mesh.Draw();
                }
            }
        }
        override
        public void HandleState(Player Player)
        {
            //if the boxes collide                // run plant and grow event on different levels, delete event once activated
            //if the object hasnt been activated  // create activation list, animate list and destruction list
            //                                    // other: in here, case active, case animate, collision
            //      
            switch (Current)
            {
                case LifeCycle.Active:
                    break;

                case LifeCycle.Animate:
                    if (Position.Y < -0.05)                    //call animate function
                    {
                        Position.Y += (float)GrowSpeed;
                        double d = (3 + (Position.Y / InitialY)) * (Math.PI / 2);
                        GrowSpeed = Math.Cos(d);
                        radius += 0.5f;
                    }
                    else
                    {
                        Current = LifeCycle.Active;         //function sets current to Active after use is spent
                        SpawnLeaves();
                    }
                    break;

                case LifeCycle.Collision:
                    if (Player.boundingsphere.Intersects(sphere))
                    {
                        KeyboardState keyState = Keyboard.GetState();
                        if (keyState.IsKeyDown(Keys.X))
                        {
                            Current = LifeCycle.Animate;
                            Position.Z -= 2;
                            Audio.Growth();
                        }
                    }
                    break;

                default:
                    throw new ArgumentException("Error - " + Current + " is not recognized.");
            }
        }

        public float getX()
        {
            return Position.X;
        }

        public float getY()
        {
            return Position.Y;
        }

        public float getZ()
        {
            return Position.Z;
        }
        void SpawnLeaves()
        {
            bool found = false;
            foreach (IObject item in ObjectControl.ObjectList)
            {
                Type x = item.GetType();
                if (x.Name == "Plant")
                {
                    Plant y = (Plant)item;
                    if (y.plantnum == plantnum)
                        found = true;
                    else
                        found = false;
                }
                else if (found == true)
                {

                    if (x.Name == "Leaf")
                    {   
                        Leaf z = (Leaf)item;
                        z.Current=Leaf.LifeCycle.Growing;
                    }
                }
            }

        }
    }
}
