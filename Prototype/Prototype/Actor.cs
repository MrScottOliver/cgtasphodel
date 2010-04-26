using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Prototype
{
    public class Actor
    {
        // The model to draw
        Model model;

        // I3DComponent values
        public virtual Vector3 Position { get; set; }
 
        public virtual Matrix Rotation { get; set; }
        public virtual Vector3 Scale { get; set; }
        public virtual BoundingBox BoundingBox

        {
            get
            {
                return new BoundingBox(
                    Position - (Scale / 2),
                    Position + (Scale / 2)
                );
            }
        }

        // Constructors take a model to draw and a position
        public Actor(Model Model, Vector3 Position)
        {
            Setup(Model, Position);
        }

        // Provide a method to setup the actor so we don't need to 
        // write it in each constructor
        void Setup(Model Model, Vector3 Position)
        {
            this.model = Model;
            this.Position = Position;
            Scale = Vector3.One;
            Rotation = Matrix.Identity;
        }

        public void Draw(Matrix Proj, Matrix View)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    Matrix world = Matrix.CreateScale(Scale) *
                                    Rotation *
                                    Matrix.CreateTranslation(Position);

                    // effect.World = player.world;
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.Projection = Proj;
                    effect.View = View;
                }
                mesh.Draw();
            }
        }

     
    }
}
