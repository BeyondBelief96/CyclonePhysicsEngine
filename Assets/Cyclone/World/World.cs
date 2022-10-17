using Assets.Cyclone.ForceGenerators;
using Assets.Cyclone.RigidBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.World
{
    /// <summary>
    /// The world represents an independent simulation of physics. It 
    /// keeps track of a set of rigid bodies, and provides the means to update
    /// them all.
    /// </summary>
    public class World
    {
        #region Properties

        /// <summary>
        /// Holds the force generators for the particles in this world.
        /// </summary>
        public RigidBodyForceRegistry Registry;

        public List<RigidBody> RigidBodies { get; set; }

        #endregion

        #region Ctor

        public World()
        {
            Registry = new RigidBodyForceRegistry();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the world for a simulation frame. This clears
        /// the force and torque accumulators for bodies in the world.
        /// After calling this, the bodies can have their forces and torques
        /// added for this frame.
        /// </summary>
        public void StartFrame()
        {
            foreach(var body in RigidBodies)
            {
                body.ClearAccumulators();
                body.CalculateDerivedData();
            }
        }

        public void Integrate(double duration)
        {
            foreach (var body in RigidBodies)
            {
                //Integrate the body by the given duration
                body.Integrate(duration);
            }
        }


        /// <summary>
        /// Processes all the physics for the world.
        /// </summary>
        public void RunPhysics(double duration)
        {
            //First, apply the force generators.
            Registry.UpdateForces(duration);

            //Then integrate the objects.
            Integrate(duration);

        }

        #endregion
    }
}
