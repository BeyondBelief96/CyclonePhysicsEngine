using Cyclone.Core;
using Cyclone.Particles;
using System;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator that applies a gravitational force. One instance can be used
    /// for multiple particles.
    /// </summary>
    public class ParticleGravityForceGenerator : IParticleForceGenerator
    {
        /// <summary>
        /// Holds the acceleration due to gravity.
        /// </summary>
        private Vector3 _gravity;

        /// <summary>
        /// Creates a force generator with the value for gravity given in the y direction.
        /// </summary>
        /// <param name="gravity"></param>
        public ParticleGravityForceGenerator(double gravity)
        {
            _gravity = new Vector3(0, gravity, 0);
        }

        /// <summary>
        /// Creates a force generator with the given direction and magnitude of the given
        /// gravity vector.
        /// </summary>
        /// <param name="gravity"></param>
        public ParticleGravityForceGenerator(Vector3 gravity)
        {
            _gravity = gravity;
        }


        /// Applies the gravitational force to the given particle
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        public void UpdateForce(Particle particle, double duration)
        {
            //Check that we do not have infinite mass.
            if (!particle.HasFiniteMass)
                return;

            //Apply the mass-scaled force to the particle.
            particle.AddForce(_gravity * particle.GetMass());
        }
    }
}
