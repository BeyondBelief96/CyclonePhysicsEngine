using Assets.Cyclone.RigidBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator can be asked to add a force to one or more bodies.
    /// </summary>
    public interface IForceGenerator
    {
        /// <summary>
        /// Implement this to calculate and update the force applied to the given rigid body.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        void UpdateForce(RigidBody body, double duration);
    }
}
