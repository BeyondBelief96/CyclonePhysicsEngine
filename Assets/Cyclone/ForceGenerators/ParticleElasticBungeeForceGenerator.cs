using Cyclone.Core;
using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator that applies a spring force only when extended.
    /// </summary>
    public class ParticleElasticBungeeForceGenerator : IParticleForceGenerator
    {
        #region Fields

        /// <summary>
        /// The particle at the other end of the spring.
        /// </summary>
        private Particle _other;

        /// <summary>
        /// Holds the sprint constant.
        /// </summary>
        private double _springConstant;

        /// <summary>
        /// Holds the rest length of the spring.
        /// </summary>
        private double _restLength;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new bungee with the given parameters.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sprintConstant"></param>
        /// <param name="restLength"></param>
        public ParticleElasticBungeeForceGenerator(Particle other, double sprintConstant, double restLength)
        {
            _other = other;
            _springConstant = sprintConstant;
            _restLength = restLength;
        }

        #endregion

        /// <summary>
        /// Calculates and applies the force to the given particle.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateForce(Particle particle, double duration)
        {
            //Calculate the vector of the spring.
            Vector3 forceVector;
            Vector3 particlePosition = particle.Position;
            forceVector = particlePosition - _other.Position;

            //Check if the bungee is compressed
            double magnitude = forceVector.Magnitude();
            if (magnitude <= _restLength) return;

            //Calculate the magnitude of the spring force.
            magnitude = _springConstant * (_restLength - magnitude);

            //Calculate the final force and apply it.
            forceVector.Normalize();
            forceVector *= -magnitude;
            particle.AddForce(forceVector);
        }
    }
}
