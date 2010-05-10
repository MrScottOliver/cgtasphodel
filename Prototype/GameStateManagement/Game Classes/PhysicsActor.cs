using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
    // Manages a PhysicsObject and allows the engine to work
    // with it
    public class PhysicsActor : Actor
    {
        // The object we are managing
        public IPhysicsObject PhysicsObject;
        // Override the position of the base class to that
        // of the physics object
        public override Vector3 Position
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.Position;
                else
                    return Vector3.Zero;
            }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.Position = value;
            }
        }

        // Override the rotation of the base class to that
        // of the physics object
        public override Matrix Rotation
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.Rotation;
                else
                    return Matrix.Identity;
            }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.Rotation = value;
            }
        }

        // Override the BoundingBox of the base class to that
        // of the physics object
        public override BoundingBox BoundingBox
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.BoundingBox;
                else
                    return new BoundingBox(-Vector3.One, Vector3.One);
            }
        }

        // Constructors

        public PhysicsActor(Model Model, IPhysicsObject PhysicsObject)
            : base(Model, PhysicsObject.Position)
        {
            this.PhysicsObject = PhysicsObject;
        }


        // DisableComponent so we can remove the physics
        // object as well
        public void DisableComponent()
        {
            this.PhysicsObject.DisableComponent();
        }
    }
}