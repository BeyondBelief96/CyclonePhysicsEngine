using Cyclone.Particles;
using System.Collections.Generic;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// Holds all the force generators and the particles they apply to.
    /// </summary>
    public class ParticleForceRegistry
    {
        #region Protected Entities

        protected struct ParticleForceRegistration
        {
            public Particle Particle { get; set; }

            public IParticleForceGenerator ForceGenerator { get; set; }
        }

        protected List<ParticleForceRegistration> Registry { get; set; }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Registers the given force generator with the given particle.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="fg"></param>
        public void AddForceGenerator(Particle particle, IParticleForceGenerator fg)
        {
            Registry.Add(new ParticleForceRegistration()
            {
                Particle = particle,
                ForceGenerator = fg
            });
        }

        /// <summary>
        /// Removes the given registered pair from the registry. If the pair is not registered,
        /// the method will have no effect.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="fg"></param>
        public void RemoveForceGenerator(Particle particle, IParticleForceGenerator fg)
        {
            ParticleForceRegistration registration = new ParticleForceRegistration()
            {
                Particle = particle,
                ForceGenerator = fg
            };

            if(Registry.Contains(registration))
                Registry.Remove(registration);
        }


        /// <summary>
        /// Clears all registrations from the registry. This will not delete the particles
        /// or the force generators themselves, just the records of their connections.
        /// </summary>
        public void Clear()
        {
            Registry = new List<ParticleForceRegistration>();
        }

        /// <summary>
        /// Calls all the force generators to update the forces of their corresponding particles.
        /// </summary>
        /// <param name="duration"></param>
        public void UpdateForces(double duration)
        {
            for(int i = 0; i < Registry.Count; i++)
            {
                Registry[i].ForceGenerator.UpdateForce(Registry[i].Particle, duration);
            }
        }

        #endregion

    }
}
