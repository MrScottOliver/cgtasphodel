using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Prototype
{
    // Manages a PhysicsObject and allows the engine to work
    // with it
    public class PhysicsActor
    {
        // The object we are managing
        public IPhysicsObject PhysicsObject;

        // Override the position of the base class to that
        // of the physics object
        public Vector3 Position
        {
            get { return PhysicsObject.Position; }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.Position = value;
            }
        }

        // Override the rotation of the base class to that
        // of the physics object
        public Vector3 Rotation
        {
            get { return PhysicsObject.Rotation; }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.Rotation = value;
            }
        }

        // Override the BoundingBox of the base class to that
        // of the physics object
        public BoundingBox BoundingBox
        {
            get { return PhysicsObject.BoundingBox; }
        }

        // Constructors

        public PhysicsActor(Model Model, IPhysicsObject PhysicsObject)
        {
            this.PhysicsObject = PhysicsObject;
        }


        // Override DisableComponent so we can remove the physics
        // object as well
        public void DisableComponent()
        {
            this.PhysicsObject.DisableComponent();
        }
    }
}