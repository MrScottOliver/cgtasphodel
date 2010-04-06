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
        
        public BoundingSphere boundingsphere;
        public Ray top, bottom, front, back;
        public Matrix translation;
        public Matrix rotation;
        public Matrix world;
        public Matrix scale;

        public Vector4 ambMtrl = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);//Jess: default material vals
        public Vector4 diffMtrl = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public Vector4 specMtrl = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);


        public Player()
        {
        model = null;
        velocity = Vector3.Zero;
        position = Vector3.Zero;
        
        boundingsphere = new BoundingSphere(position, 1.0f);
        top = new Ray(position, new Vector3(position.X, position.Y + 1.0f, position.Z));
        bottom = new Ray(position, new Vector3(position.X, position.Y - 1.0f, position.Z));
        front = new Ray(position, new Vector3(position.X + 1.0f, position.Y, position.Z));
        back = new Ray(position, new Vector3(position.X - 1.0f, position.Y + 1.0f, position.Z));

        scale = Matrix.Identity;
        translation = Matrix.Identity;
        rotation = Matrix.Identity;
        world = Matrix.Identity;
        gravity = new Vector3(0f, -0.005f, 0f);
        }

        //Kieran: draw player function
        public void DrawPlayer(Player player, Matrix Proj, Matrix View)
        {
            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = player.world;

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
            Matrix wv = world * View;

            Matrix wvIT = Matrix.Invert(wv);
            wvIT = Matrix.Transpose(wvIT);

            Matrix worldIT = Matrix.Invert(world);
            worldIT = Matrix.Transpose(worldIT);

            foreach (ModelMesh mesh in player.model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    //set technique
                    effect.CurrentTechnique = effect.Techniques["MyTech"];

                    //set matrix params
                    effect.Parameters["gWVP"].SetValue(wvp);
                    effect.Parameters["gWorldView"].SetValue(wv);
                    effect.Parameters["gWorldViewIT"].SetValue(wvIT);
                    effect.Parameters["gWorld"].SetValue(world);
                    effect.Parameters["gWorldIT"].SetValue(worldIT);

                    //set material params
                    effect.Parameters["gAmbMtrl"].SetValue(player.ambMtrl);
                    effect.Parameters["gDiffuseMtrl"].SetValue(player.diffMtrl);
                    effect.Parameters["gSpecMtrl"].SetValue(player.specMtrl);

                    //set texture
                    effect.Parameters["gTex"].SetValue(player.texture);



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
            AddTranslation(velocity.X, velocity.Y, velocity.Z) ;
        }
        
    }


}
