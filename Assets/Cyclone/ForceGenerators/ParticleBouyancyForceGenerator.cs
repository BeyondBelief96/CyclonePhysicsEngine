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
    /// A force generator that applies a bouyancy force for a plane of
    /// liquid parallel to the XZ plane.
    /// </summary>
    public class ParticleBouyancyForceGenerator : IParticleForceGenerator
    {
        /// <summary>
        /// The maximum submersion depth of the object before it generates its maximum bouyancy
        /// force.
        /// </summary>
        private double _maxDepth;

        /// <summary>
        /// The volume of the object
        /// </summary>
        private double _volume;

        /// <summary>
        /// The height of the water plane above y = 0. The plane will be 
        /// parallel to the XZ plane.
        /// </summary>
        private double _waterHeight;


        /// <summary>
        /// The density of the liquid. Water has a density of 1000kg per cubic meter.
        /// </summary>
        private double _liquidDensity;

        /// <summary>
        /// Creates a new bouyancy force with the given parameters.
        /// </summary>
        public ParticleBouyancyForceGenerator(double maxDepth, double volume,
            double waterHeight, double liquidDensity = 1000)
        {
            _maxDepth = maxDepth;
            _volume = volume;
            _waterHeight = waterHeight;
            _liquidDensity = liquidDensity;
        }

        /// <summary>
        /// Applies the bouyancy force witht he given parameters.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateForce(Particle particle, double duration)
        {
            //Calculate the submersion depth.

            double depth = particle.Position.Y;

            //Check if we're out of the water.
            if (depth >= _waterHeight + _maxDepth) return;
            Vector3 force = Vector3.ZeroVector;

            //Check if were at the max depth.
            if(depth <= _waterHeight - _maxDepth)
            {
                force.Y = _liquidDensity * _volume;
                particle.AddForce(force);
                return;
            }

            //Otherwise we're partly submerged.
            force.Y = _liquidDensity * _volume * (depth - _maxDepth - _waterHeight) / (2 * _maxDepth);
            particle.AddForce(force);
        }
    }
}
