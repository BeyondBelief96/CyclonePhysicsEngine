using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Particles.Collisions
{
    public abstract class ParticleLink
    {
        /// <summary>
        /// Holds the pair of particles that are connected by this link.
        /// </summary>
        public Particle[] Particles = new Particle[2];

        /// <summary>
        /// Returns the current length of the link.
        /// </summary>
        public abstract double CurrentLength();

        /// <summary>
        /// Generates the contacts to keep this link from being
        /// violated.This class can only ever generate a single
        /// contact, so the pointer can be a pointer to a single
        /// element, the limit parameter is assumed to be at least 1
        /// (0 isn’t valid), and the return value is 0 if the
        /// cable wasn’t overextended, or 1 if a contact was needed.
        /// 
        /// NB: This method is declared in the same way (as pure
        /// virtual) in the parent class, but is replicated here for
        /// documentation purposes.
        /// </summary>
        /// <param name="Contacts"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public abstract uint AddContact(ParticleContact Contact, uint limit);


    }
}
