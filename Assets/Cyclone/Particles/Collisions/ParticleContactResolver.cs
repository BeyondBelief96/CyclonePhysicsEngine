using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Particles.Collisions
{
    /// <summary>
    /// A contact resolution routine for particle contacts.
    /// One resolver instance can be shared for the entire simulation.
    /// </summary>
    public class ParticleContactResolver
    {
        /// <summary>
        /// Holds the number of iterations allowed.
        /// </summary>
        public uint Iterations { get; set; }

        /// <summary>
        /// This is a performance tracking value; we keep a record
        /// of the actual number of iterations used.
        /// </summary>
        public uint IterationsUsed { get; set; }

        /// <summary>
        /// Creates a new contact resolver with the given
        /// number of iterations.
        /// </summary>
        /// <param name="iterations"></param>
        public ParticleContactResolver(uint iterations)
        {
            Iterations = iterations;
        }

        /// <summary>
        /// Creates a new contact resolver.
        /// </summary>
        public ParticleContactResolver()
        {

        }

        /// <summary>
        /// Sets the number of iterations that can be used.
        /// </summary>
        /// <param name="iterations"></param>
        public void SetIterations(uint iterations)
        {
            Iterations = iterations;
        }

        /// <summary>
        /// Resolves a set of particle contacts for both penetration and velocity.
        /// </summary>
        /// <param name="contacts"></param>
        public void ResolveContacts(ParticleContact[] contacts, uint numContacts, double duration)
        {
            uint i;

            IterationsUsed = 0;
            while(IterationsUsed < Iterations)
            {
                //Find the contact with the largest closing velocity.
                double max = double.MaxValue;
                uint maxIndex = numContacts;
                for(i = 0; i < numContacts; i++)
                {
                    double sepVelocity = contacts[i].CalculateSeparatingVelocity();
                    if(sepVelocity < max && (sepVelocity < 0 || contacts[i].Penetration > 0))
                    {
                        max = sepVelocity;
                        maxIndex = i;
                    }
                }

                //Do we have anything worth resolving?
                if (maxIndex == numContacts) break;

                //Resolve this contact.
                contacts[maxIndex].Resolve(duration);
                IterationsUsed++;
            }
        }

    }
}
