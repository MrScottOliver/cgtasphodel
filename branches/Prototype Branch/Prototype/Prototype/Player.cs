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
        public Vector3 velocity;
        public Vector3 position;
        public Vector3 gravity;
        
        public BoundingSphere boundingsphere;
        public Matrix translation;
        public Matrix rotation;
        public Matrix world;
        public Matrix scale;


        public Player()
        {
        model = null;
        velocity = Vector3.Zero;
        position = Vector3.Zero;
        
        boundingsphere = new BoundingSphere(position, 1.0f);
        scale = Matrix.Identity;
        translation = Matrix.Identity;
        rotation = Matrix.Identity;
        world = Matrix.Identity;
        gravity = new Vector3(0f, -0.1f, 0f);
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

        
    }


}
