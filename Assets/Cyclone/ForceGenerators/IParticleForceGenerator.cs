using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator can be asked to add a force to one or more particles.
    /// </summary>
    public interface IParticleForceGenerator : IForceGenerator
    {
        /// <summary>
        /// A generator implementing this function needs to calculate and update the force
        /// applied to the given particle.
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="duration"></param>
        void UpdateForce(Particle particle, double duration);
    }
}
