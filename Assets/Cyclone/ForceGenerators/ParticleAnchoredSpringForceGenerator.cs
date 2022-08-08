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
    /// A force generator that applies a spring force, where one end is attatched
    /// to a fixed point in space.
    /// </summary>
    public class ParticleAnchoredSpringForceGenerator : IParticleForceGenerator
    {
        #region Fields

        /// <summary>
        /// The location of the anchored end of the spring
        /// </summary>
        private Vector3 _anchor;

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

        public ParticleAnchoredSpringForceGenerator(Vector3 anchor, double sprintConstant, double restLength)
        {
            _anchor = anchor;
            _springConstant = sprintConstant;
            _restLength = restLength;
        }

        #endregion

        /// <summary>
        /// Calculates and applies the force to the particle.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateForce(Particle particle, double duration)
        {
            //Calculate the vector of the spring.
            Vector3 forceVector;
            Vector3 particlePosition = particle.Position;
            forceVector = particlePosition - _anchor;

            //Calculate the magnitude of the force.
            double magnitude = forceVector.Magnitude();
            magnitude = (_restLength - magnitude) * _springConstant;

            //Calculate the final force and apply it.
            forceVector.Normalize();
            forceVector *= magnitude;
            particle.AddForce(forceVector);
        }
    }
}
