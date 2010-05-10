﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Prototype
{
    // A sphere physics object
    public class SphereObject : IPhysicsObject
    {
        float radius;

        // The radius of the sphere
        public float Radius
        {
            get { return radius; }
            set
            {
                // Set the new value
                radius = value;

                // Update the collision skin
                CollisionSkin.RemoveAllPrimitives();
                CollisionSkin.AddPrimitive(new Sphere(Vector3.Zero * 5.0f, value),
                    new MaterialProperties(0.5f, 0.7f, 0.6f));
            }
        }

        // Constructors

        public SphereObject()
            : base()
        {
            SetupSkin(Radius, Vector3.Zero, Vector3.Zero);
        }

        public SphereObject(float Radius)
        {
            SetupSkin(Radius, Vector3.Zero, Vector3.Zero);
        }

        public SphereObject(float Radius, Vector3 Position, Vector3 Rotation)
            : base()
        {
            SetupSkin(Radius, Position, Rotation);
        }


        // Sets up the object
        void SetupSkin(float Radius, Vector3 Position, Vector3 Rotation)
        {
            // Setup the body
            InitializeBody();

            // Set parameters
            this.Radius = Radius;
            this.Position = Position;

            Matrix mRotation = Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
            this.Rotation = mRotation;

        }
    }
}
