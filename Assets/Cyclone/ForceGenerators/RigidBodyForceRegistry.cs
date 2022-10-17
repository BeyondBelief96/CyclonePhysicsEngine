using Assets.Cyclone.RigidBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// Holds all the force generators and the rigid bodies they apply to.
    /// </summary>
    public class RigidBodyForceRegistry
    {
        protected struct RigidBodyForceRegistration
        {
            public RigidBody Body { get; set; }

            public IForceGenerator ForceGenerator { get; set; }
        }

        #region Properties

        protected List<RigidBodyForceRegistration> Registry { get; set; }

        #endregion


        #region Ctor

        public RigidBodyForceRegistry()
        {
            Registry = new List<RigidBodyForceRegistration>();
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Registers the given force generator with the given rigid body.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="fg"></param>
        public void AddForceGenerator(RigidBody body, IParticleForceGenerator fg)
        {
            Registry.Add(new RigidBodyForceRegistration()
            {
                Body = body,
                ForceGenerator = fg
            });
        }

        /// <summary>
        /// Removes the given registered pair from the registry. If the pair is not registered,
        /// the method will have no effect.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="fg"></param>
        public void RemoveForceGenerator(RigidBody body, IForceGenerator fg)
        {
            RigidBodyForceRegistration registration = new RigidBodyForceRegistration()
            {
                Body = body,
                ForceGenerator = fg
            };

            if (Registry.Contains(registration))
                Registry.Remove(registration);
        }


        /// <summary>
        /// Clears all registrations from the registry. This will not delete the rigid bodies
        /// or the force generators themselves, just the records of their connections.
        /// </summary>
        public void Clear()
        {
            Registry = new List<RigidBodyForceRegistration>();
        }

        /// <summary>
        /// Calls all the force generators to update the forces of their corresponding rigid bodies.
        /// </summary>
        /// <param name="duration"></param>
        public void UpdateForces(double duration)
        {
            if (Registry.Count == 0 || Registry == null) return;
            for (int i = 0; i < Registry.Count; i++)
            {
                Registry[i].ForceGenerator.UpdateForce(Registry[i].Body, duration);
            }
        }

        #endregion
    }
}
