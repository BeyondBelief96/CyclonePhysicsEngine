using Cyclone.Core;
using Cyclone.Particles;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator that applies a drag force. One instance can be used
    /// for multiple particles.
    /// </summary>
    public class ParticleDragForceGenerator : IParticleForceGenerator
    {
        /// <summary>
        /// Holds the velocity drag coefficient.
        /// </summary>
        private double _k1;

        /// <summary>
        /// Holds the velocity squared drag coefficient.
        /// </summary>
        private double _k2;

        /// <summary>
        /// Creates the generator with the given coefficients.
        /// Drag equation is modeled as: f_drag = -V(k1*|V| + k2*|V|^2)
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        public ParticleDragForceGenerator(double k1, double k2)
        {
            _k1 = k1;
            _k2 = k2;
        }

        /// <summary>
        /// Applies the drag force to the given particle
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        public void UpdateForce(Particle particle, double duration)
        {
            Vector3 force = particle.Velocity;

            //Calculate the total drag coefficient
            double velcoityMagnitude = force.Magnitude();
            double dragCoeff = _k1 * velcoityMagnitude + _k2 * velcoityMagnitude * velcoityMagnitude;

            //Calculate the final force and apply it.
            force.Normalize();
            force *= -dragCoeff;
            particle.AddForce(force);
        }
    }
}
