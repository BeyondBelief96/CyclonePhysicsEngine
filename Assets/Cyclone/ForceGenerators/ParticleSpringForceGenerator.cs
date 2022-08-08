using Cyclone.Core;
using Cyclone.Particles;
using System;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator that applies a spring force.
    /// Only applies to a single particle. If you want to link two particles, you need too create
    /// and register a generator for each particle.
    /// </summary>
    public class ParticleSpringForceGenerator : IParticleForceGenerator
    {
        #region Fields

        /// <summary>
        /// The particle at the other end of the screen.
        /// </summary>
        private Particle _otherParticle;

        /// <summary>
        /// Holds the spring constant
        /// </summary>
        private double _springConstant;

        /// <summary>
        /// Holds the rest length of the spring.
        /// </summary>
        private double _restLength;

        #endregion

        #region Ctor

        public ParticleSpringForceGenerator(Particle other, double sprintConstant, double restLength)
        {
            _otherParticle = other;
            _springConstant = sprintConstant;  
            _restLength = restLength;
        }

        #endregion

        /// <summary>
        /// Applies the spring force to the given particle
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateForce(Particle particle, double duration)
        {
            //Calculate the vector of the spring.
            Vector3 forceVector;
            Vector3 particlePosition = particle.Position;
            forceVector = particlePosition - _otherParticle.Position;

            //Calculate the magnitude of the force.
            double magnitude = forceVector.Magnitude();
            magnitude = Math.Abs(magnitude - _restLength);
            magnitude *= _springConstant;

            //Calculate the final force and apply it.
            forceVector.Normalize();
            forceVector *= -magnitude;
            particle.AddForce(forceVector);
        }
    }
}
