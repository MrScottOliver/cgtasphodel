using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Prototype
{
    // A box shaped physics object
    public class BoxObject : IPhysicsObject
    {
        Vector3 sideLengths;

        // The length of the sides of the box
        public Vector3 SideLengths
        {
            get { return sideLengths; }
            set
            {
                // Set the new value
                sideLengths = value;

                // Update the collision skin
                CollisionSkin.RemoveAllPrimitives();
                CollisionSkin.AddPrimitive(
                    new Box(-0.5f * value, Body.Orientation, value),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));

                // Set the mass to itself to fix the local transform
                // on the CollisionSkin in the set accessor
                this.Mass = this.Mass;
            }
        }

        // Constructors

        public BoxObject()
            : base()
        {
            InitializeBody();
            SideLengths = Vector3.One;
        }

        public BoxObject(Vector3 SideLengths)
            : base()
        {
            SetupSkin(SideLengths, Vector3.Zero, Vector3.Zero);
        }

        public BoxObject(Vector3 SideLengths, Vector3 Position,
            Vector3 Rotation)
            : base()
        {
            SetupSkin(SideLengths, Position, Rotation);
        }


        // Sets up the object with the specified parameters
        void SetupSkin(Vector3 SideLengths, Vector3 Position,
            Vector3 Rotation)
        {
            // Setup the body
            InitializeBody();

            // Set properties
            this.SideLengths = SideLengths;
            this.Position = Position;

            Matrix mRotation = Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

            this.Rotation = mRotation;
        }
    }
}
