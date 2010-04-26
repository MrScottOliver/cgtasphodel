using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Prototype
{
    public abstract class IPhysicsObject
    {
        // Local copy of the mass of the object
        float mass = 1;

        // The Body managed by the IPhysicsObject
        public Body Body;

        // The CollisionSkin managed by the IPhysicsObject
        public CollisionSkin CollisionSkin;

        // The mass of the IPhysicsObject
        public float Mass
        {
            get { return mass; }
            set
            {
                // Set the new value
                mass = value;

                // Fix transforms
                Vector3 com = SetMass(value);
                if (CollisionSkin != null)
                    CollisionSkin.ApplyLocalTransform(
                        new JigLibX.Math.Transform(-com, Matrix.Identity));
            }
        }

        // The IPhysicsObject's position
        public Vector3 Position
        {
            get { return Body.Position; }
            set { Body.MoveTo(value, Body.Orientation); }
        }

        // The IPhysicsObject's orientation
        public Matrix Rotation
        {
            get { return Body.Orientation; }
            set { Body.MoveTo(Body.Position, value); }
        }

        // Whether or not the physics object is locked in place
        public bool Immovable
        {
            get { return Body.Immovable; }
            set { Body.Immovable = value; }
        }


        // The IPhysicsObject's rotation
        /*public Vector3 Rotation
        {
            get
            {
                Quaternion q = Quaternion.CreateFromRotationMatrix(Orientation);
                return new Vector3(q.X, q.Y, q.Z);
            }
            set
            {
                Body.MoveTo(Body.Position, Matrix.CreateFromYawPitchRoll(
                    MathHelper.ToRadians(value.Y),
                    MathHelper.ToRadians(value.X),
                    MathHelper.ToRadians(value.Z)));
            }
        }*/

        // Returns the IPhysicsObject's BoundingBox
        public BoundingBox BoundingBox
        {
            get
            {
                if (Body.CollisionSkin != null)
                    return Body.CollisionSkin.WorldBoundingBox;
                else
                    return new BoundingBox(Position - Vector3.One,
                        Position + Vector3.One);
            }
        }

        // The body's velocity
        public Vector3 Velocity
        {
            get { return Body.Velocity; }
            set { Body.Velocity = value; }
        }

        // Constructors
        public IPhysicsObject()
        {
        }


        // Sets up the body and collision skin
        protected void InitializeBody()
        {
            Body = new Body();
            CollisionSkin = new CollisionSkin(Body);
            Body.CollisionSkin = this.CollisionSkin;
            Body.EnableBody();
        }

        // Sets the mass of the IPhysicsObject
        public Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties =
                new PrimitiveProperties(
                    PrimitiveProperties.MassDistributionEnum.Solid,
                    PrimitiveProperties.MassTypeEnum.Density, mass);

            float junk; Vector3 com; Matrix it, itCoM;

            CollisionSkin.GetMassProperties(primitiveProperties,
                out junk, out com, out it, out itCoM);
            Body.BodyInertia = itCoM;
            Body.Mass = junk;

            return com;
        }

        // Rotates and moves the model relative to the physics object to
        // better align the model with the object
        public void OffsetModel(Vector3 PositionOffset,
            Matrix RotationOffset)
        {
            CollisionSkin.ApplyLocalTransform(
                new JigLibX.Math.Transform(PositionOffset, RotationOffset));
        }

        // Disables physics body and component
        public void DisableComponent()
        {
            Body.DisableBody();
        }
    }
}
