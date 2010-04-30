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

//Kieran: player class: basic so far, just to get a model moving around screen

namespace Prototype
{
    class Player
    {
        public Model model;
        public Texture2D texture;
        public Vector3 velocity;
        public Vector3 position;
        public Vector3 gravity;
        public float health;
        public bool jumpState;
        public bool doublejumpState;

        public BoundingSphere boundingsphere;
        public Ray top, bottom, front, back;
        public Matrix translation;
        public Matrix rotation;
        public Matrix world;
        public Matrix scale;

        public Vector4 ambMtrl = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);//Jess: default material vals
        public Vector4 diffMtrl = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public Vector4 specMtrl = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

        public static int OrbCount;

        public Player()
        {
            model = null;
            velocity = Vector3.Zero;
            position = Vector3.Zero;

            boundingsphere = new BoundingSphere(position, 1.0f);
            top = new Ray(position, new Vector3(0, 1, 0));
            bottom = new Ray(position, new Vector3(0, -1, 0));
            front = new Ray(position, new Vector3(1, 0, 0));
            back = new Ray(position, new Vector3(-1, 0, 0));

            scale = Matrix.Identity;
            translation = Matrix.Identity;
            rotation = Matrix.Identity;
            world = Matrix.Identity;
            gravity = new Vector3(0f, -0.005f, 0f);

            jumpState = false;
            doublejumpState = false;
            health = 0;

            OrbCount = 0;
        }

        //Kieran: draw player function
        public void DrawPlayer(Player player, Matrix Proj, Matrix View)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    // effect.World = player.world;
                    effect.World = transforms[mesh.ParentBone.Index] * rotation * scale * translation;
                    effect.Projection = Proj;
                    effect.View = View;
                }
                mesh.Draw();
            }
        }

        //draws player with our custom lighting effect
        public void DrawPlayer2(Player player, Matrix Proj, Matrix View)
        {
            //calculate matrices
            Matrix world = player.world;
            Matrix wvp = world * View * Proj;
            Matrix worldIT = Matrix.Invert(world);
            worldIT = Matrix.Transpose(worldIT);


            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    //set technique
                    effect.CurrentTechnique = effect.Techniques["MyTech"];

                    effect.Parameters["withlights"].SetValue(true);

                    //set matrix params
                    effect.Parameters["gWVP"].SetValue(wvp);
                    effect.Parameters["gWorld"].SetValue(world);
                    effect.Parameters["gWorldIT"].SetValue(worldIT);
                    effect.Parameters["player"].SetValue(true);
                    effect.Parameters["health"].SetValue(health);
                    //set material params
                    effect.Parameters["gAmbMtrl"].SetValue(player.ambMtrl);
                    effect.Parameters["gDiffuseMtrl"].SetValue(player.diffMtrl);
                    effect.Parameters["gSpecMtrl"].SetValue(player.specMtrl);

                    //set texture
                    effect.Parameters["gTex"].SetValue(player.texture);
                    effect.Parameters["withgrey"].SetValue(true);

                    effect.CommitChanges();



                }
                mesh.Draw();
            }
        }

        //remaps the model to use our effect instead of basic effect
        public void RemapModel(Player player, Effect effect)
        {
            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

        //creates the shadow map for the player
        public void DrawPlayerShadow(Player player, Matrix Proj, Matrix View)
        {
            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["CreateShadowMapTech"];

                    effect.Parameters["gWorld"].SetValue(player.world);
                    effect.CommitChanges();

                }
                mesh.Draw();
            }
        }


        public void CreateWorld()
        {
            world = rotation * scale * translation;
        }

        public void AddRotation(float x, float y, float z)
        {
            rotation *= Matrix.CreateFromYawPitchRoll(x, y, z);
            CreateWorld();
        }

        public void AddTranslation(float x, float y, float z)
        {
            position += new Vector3(x, y, z);
            boundingsphere.Center = position;
            top.Position = position;
            bottom.Position = position;
            front.Position = position;
            back.Position = position;
            translation = Matrix.CreateTranslation(position);
            CreateWorld();
        }

        public void ChangeScale(float s)
        {
            scale = Matrix.CreateScale(s);
            CreateWorld();
        }

        public void ChangeRadius(float r)
        {
            boundingsphere.Radius = r;
        }

        public void Move()
        {
            velocity += gravity;
        }

        public void Update()
        {
            AddTranslation(velocity.X, velocity.Y, velocity.Z);
        }

        public void Jump()
        {
            if (!jumpState)
            {
                //AddTranslation(0, 2.0f, 0);
                velocity.Y = 0.1f;
                jumpState = true;
                Audio.Jump();
            }
            else
            {
                doubleJump();
            }
        }

        public void doubleJump()
        {
            if (!doublejumpState)
            {
                velocity.Y = 0.3f;
                Audio.Jump();
                doublejumpState = true;
            }
        }

        public void moveRight()
        {
            if (velocity.X < 0.1f)
            {
                velocity.X = 0.2f;
                Audio.Step();
            }
        }

        public void moveLeft()
        {
            if (velocity.X > -0.1f)
            {
                velocity.X = -0.2f;
                Audio.Step();
            }
        }

        public void stopRight()
        {
            if (velocity.X > 0.0f)
            {
                velocity.X = 0.0f;
            }
        }

        public void stopLeft()
        {
            if (velocity.X < 0.0f)
            {
                velocity.X = 0.0f;
            }
        }
    }
}
