﻿using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Particles.Collisions
{
    public class ParticleCable : ParticleLink
    {
        /// <summary>
        /// Holds the max length of the cable.
        /// </summary>
        public double MaxLength { get; set; }

        /// <summary>
        /// Holds the restitution (bounciness) of the cable.
        /// </summary>
        public double Restitution { get; set; }

        public override double CurrentLength()
        {
            Vector3 relativePosition = Particles[0].Position - Particles[1].Position;
            return relativePosition.Magnitude();
        }

        /// <summary>
        /// Fills the given contact structure with the contact needed to
        /// keep the cable from over extending.
        /// </summary>
        /// <param name="Contacts"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override uint AddContact(ParticleContact contact, uint limit)
        {
            //Find the length of the cable.
            double length = CurrentLength();

            //Check if the cable has overextended.
            if(length < MaxLength)
            {
                return 0;
            }

            //Otherwise, return the contact.
            contact.Particles[0] = Particles[0];
            contact.Particles[1] = Particles[1];

            //Calculate the contact normal.
            Vector3 normal = Particles[1].Position - Particles[0].Position;
            normal.Normalize();
            contact.ContactNormal = normal;
            contact.Penetration = length - MaxLength;
            contact.Restitution = Restitution;

            return 1;
        }
    }
}
