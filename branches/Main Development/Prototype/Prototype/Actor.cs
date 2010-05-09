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
        public Vector4 ambMtrl = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);//Jess: default material vals
        public Vector4 diffMtrl = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public Vector4 specMtrl = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);


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

        public void Draw2( Matrix Proj, Matrix View)
        {
            //calculate matrices
            Matrix world = Matrix.CreateScale(Scale) *
                                    Rotation *
                                    Matrix.CreateTranslation(Position);
            Matrix wvp = world * View * Proj;
            Matrix worldIT = Matrix.Invert(world);
            worldIT = Matrix.Transpose(worldIT);


            foreach (ModelMesh mesh in model.Meshes)
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
                   
                    //set material params
                    effect.Parameters["gAmbMtrl"].SetValue(ambMtrl);
                    effect.Parameters["gDiffuseMtrl"].SetValue(diffMtrl);
                    effect.Parameters["gSpecMtrl"].SetValue(specMtrl);

                    //set texture
                    effect.Parameters["withgrey"].SetValue(true);
                    effect.Parameters["withshadow"].SetValue(true);

                    effect.CommitChanges();



                }
                mesh.Draw();
            }
        }

        public void RemapModel(Actor actor, Effect effect)
        {
            foreach (ModelMesh mesh in actor.model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

     
    }
}
