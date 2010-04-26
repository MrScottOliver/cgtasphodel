using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using JigLibX.Physics;

namespace Prototype
{
    class Physics
    {
        // The physics simulation
        public PhysicsSystem PhysicsSystem = new PhysicsSystem();
 
        // Whether or not we should update
        public bool UpdatePhysics = true;

        public Physics()
        {
            // Set up physics system
            this.PhysicsSystem.EnableFreezing = true;
            this.PhysicsSystem.SolverType = PhysicsSystem.Solver.Normal;
            this.PhysicsSystem.CollisionSystem = new CollisionSystemSAP();
        }
 
        public void Update(float _dt)
        {
            // Update the physics system
            if (UpdatePhysics)

                PhysicsSystem.CurrentPhysicsSystem.Integrate(_dt);

        }
    }
}
