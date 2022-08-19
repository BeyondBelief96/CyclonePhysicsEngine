using Cyclone.Core;
using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Particles.Collisions
{
    public class ParticleRod : ParticleContactGenerator
    {
        /// <summary>
        /// Holds the pair of particles that are connected by this link.
        /// </summary>
        public Particle[] Particles;

        /// <summary>
        /// Holds the length of the rod.
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// Creates a particle rod.
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="length"></param>
        public ParticleRod(Particle[] particles, double length)
        {
            Particles = particles;
            Length = length;
        }

        /// <summary>
        /// Fills the given contact structure with the contact needed to keep the rod
        /// from extending or compressing.
        /// </summary>
        /// <param name="Contact"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override uint AddContact(IList<Particle> particles, IList<ParticleContact> contacts,
            uint next)
        {
            double currentLength = Vector3.Distance(Particles[0].Position, Particles[1].Position);

            //Check if we're overextended.
            if (currentLength == Length) return 0;

            var contact = contacts[(int) next];

            //Otherwise return the contact.
            contact.Particles[0] = Particles[0];
            contact.Particles[1] = Particles[1];

            //Calculate the normal.
            Vector3 normal = Particles[1].Position - Particles[0].Position;
            normal.Normalize();

            //The contact normal depends on whether we're extending or compressing.
            if(currentLength > Length)
            {
                contact.ContactNormal = normal;
                contact.Penetration = currentLength - Length;
            }
            else
            {
                contact.ContactNormal = -1*normal;
                contact.Penetration = Length = currentLength;
            }

            //Always use zero restitution (no bounciness)
            contact.Restitution = 0;

            return 1;

            
        }


    }
}
