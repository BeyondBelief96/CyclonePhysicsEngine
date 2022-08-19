using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Particles.Collisions
{
    public class ParticleRod : ParticleLink
    {
        /// <summary>
        /// Holds the length of the rod.
        /// </summary>
        public double Length { get; set; }

        public override double CurrentLength()
        {
            return Length;
        }

        /// <summary>
        /// Fills the given contact structure with the contact needed to keep the rod
        /// from extending or compressing.
        /// </summary>
        /// <param name="Contact"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override uint AddContact(ParticleContact Contact, uint limit)
        {
            double currentLength = CurrentLength();

            //Check if we're overextended.
            if(currentLength == Length)
            {
                return 0;
            }

            //Otherwise return the contact.
            Contact.Particles[0] = Particles[0];
            Contact.Particles[1] = Particles[1];

            //Calculate the normal.
            Vector3 normal = Particles[1].Position - Particles[0].Position;
            normal.Normalize();

            //The contact normal depends on whether we're extending or compressing.
            if(currentLength > Length)
            {
                Contact.ContactNormal = normal;
                Contact.Penetration = currentLength - Length;
            }
            else
            {
                Contact.ContactNormal = -1*normal;
                Contact.Penetration = Length = currentLength;
            }

            //Always use zero restitution (no bounciness)
            Contact.Restitution = 0;

            return 1;

            
        }


    }
}
