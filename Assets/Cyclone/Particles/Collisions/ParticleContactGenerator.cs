using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Particles.Collisions
{
    /// <summary>
    /// This is the basic polymorphic interface for contact generators applying to particles.
    /// </summary>
    public abstract class ParticleContactGenerator
    {
        /// <summary>
        /// Fills the given contact structure with the generated
        /// contact. The contact pointer should point to the first
        /// available contact in a contact array, where limit is the
        /// maximum number of contacts in the array that can be written
        /// to.The method returns the number of contacts that have
        /// been written.
        /// </summary>
        public abstract uint AddContact(IList<Particle> particles, IList<ParticleContact> contacts, uint next);


    }
}
