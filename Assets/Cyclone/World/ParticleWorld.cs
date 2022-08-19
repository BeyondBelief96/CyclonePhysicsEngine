using Assets.Cyclone.ForceGenerators;
using Assets.Cyclone.Particles.Collisions;
using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.World
{
    /// <summary>
    /// Keeps track of a set of particles, and provides a means to update them all.
    /// </summary>
    public class ParticleWorld
    {
        #region Fields

        /// <summary>
        /// Holds the list of particles in this world.
        /// </summary>
        public List<Particle> Particles;

        /// <summary>
        /// Holds the force generators for the particles in this world.
        /// </summary>
        public ParticleForceRegistry Registry;

        /// <summary>
        /// Holds the resolver for contacts.
        /// </summary>
        public ParticleContactResolver Resolver;

        /// <summary>
        /// Contact Generators
        /// </summary>
        public List<ParticleContactGenerator> ContactGenerators;

        /// <summary>
        /// Holds the list of contacts
        /// </summary>
        public ParticleContact[] Contacts;

        /// <summary>
        /// Holds the maximum number of contact allowed (i.e., the 
        /// size of the contacts araray).
        /// </summary>
        private uint MaxContacts => (uint) Contacts.Length;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new particle simulator that can handle up to the
        /// given number of contacts per frame. You can also optionally
        /// give a number of contact-resolution iterations to use. if you don't
        ///  give a number of iterations, then twice the number of contacts will
        ///  be used.
        /// </summary>
        public ParticleWorld()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the world for a simulation frame. This clears
        /// the force accumulators for particles in the world. After calling this, the
        /// particles can have their forces for this frame added.
        /// </summary>
        public void StartFrame()
        {
            foreach(var p in Particles)
            {
                p.ClearAccumulator();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calls each of the registered contact generators to report
        /// their contacts. Returns the number of generated contacts.
        /// </summary>
        /// <returns></returns>
        private uint GenerateContacts()
        {
            ClearContacts();

            uint limit = (uint) Contacts.Length;
            uint nextContact = 0;

            foreach(var g in ContactGenerators)
            {
                uint used = g.AddContact(Particles, Contacts, nextContact);
                limit -= used;
                nextContact += used;

                //We've run out of contacts to fill. This means we're missing contacts.
                if (limit <= 0) break;
            }

            return MaxContacts - limit;
        }

        /// <summary>
        /// Integrates all the particles in this world forward in time by the given
        /// duration.
        /// </summary>
        /// <param name="duration"></param>
        private void Integrate(double duration)
        {
            for(int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Integrate(duration);
            }
        }

        /// <summary>
        /// Processes all the physics for the particle world.
        /// </summary>
        /// <param name="duration"></param>
        private void RunPhysics(double duration)
        {
            //First apply the force generators.
            Registry.UpdateForces(duration);

            //Then integrate the objects.
            Integrate(duration);

            //Generate Contacts.
            uint usedContacts = GenerateContacts();

            //Process contacts.
            if(usedContacts > 0)
            {
                Resolver.SetIterations(usedContacts * 2);
                Resolver.ResolveContacts(Contacts, usedContacts, duration);
            }
        }

        private void ClearContacts()
        {
            foreach(var contact in Contacts)
            {
                contact.Clear();
            }
        }

        #endregion
    }
}
