using Assets.Cyclone.RigidBodies;
using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator that applies a gravitational force. One instance 
    /// can be used for multiple rigid bodies.
    /// </summary>
    public class Gravity : IForceGenerator
    {
        #region Properties

        //Holds the acceleration due to gravity.
        private Vector3 gravity;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates the generator with the given acceleration.
        /// </summary>
        /// <param name="gravity"></param>
        public Gravity(Vector3 gravityVector)
        {
            gravity = gravityVector;
        }

        #endregion

        #region IForceGenerator Implementation

        /// <summary>
        /// Applies the gravitational force to the given rigid body.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        public void UpdateForce(RigidBody body, double duration)
        {
            //Check that we do not have infinite mass.
            if (body.HasInfiniteMass) return;

            //Apply the mass-scaled force to the body.
            body.AddForce(gravity * body.GetMass());
        }

        #endregion
    }
}
