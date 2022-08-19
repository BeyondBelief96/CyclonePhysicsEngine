using Cyclone.Core;
using Cyclone.Particles;

namespace Assets.Cyclone.Particles.Collisions
{
    /// <summary>
    /// A contact represents two objects in contact (in this case, it represents
    /// two particles). Resolving a contact removes their interpenetration, and applies
    /// sufficient impulse to keep them apart. Colliding bodies may also be rebound.
    /// This contact provides no callable functions. It simply exists as a data structure
    /// for the contact details. To resolve a set of contacts, use the particle contact resolver.
    /// </summary>
    public class ParticleContact
    {
        #region Properties

        /// <summary>
        /// Holds the two particles that are involved in the collision. The 2nd
        /// particle can be null for the case of contact with an immovable environment object.
        /// </summary>
        public Particle[] Particles { get; set; }

        /// <summary>
        /// Holds the normal restitution coefficient at the contact.
        /// </summary>
        public double Restitution { get; set; }

        /// <summary>
        /// Holds the direction of the contact in world coordinates.
        /// </summary>
        public Vector3 ContactNormal { get; set; }

        /// <summary>
        /// Holds the depth of penetration at the contact.
        /// </summary>
        public double Penetration { get; set; }

        #endregion

        #region Ctor

        public ParticleContact()
        {
            Particles = new Particle[2];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resolves this contact for both velocity and interpenetration.
        /// </summary>
        /// <param name="duration"></param>
        public void Resolve(double duration)
        {
            ResolveVelocity(duration);
            ResolveInterpenetration(duration);
        }

        /// <summary>
        /// Calculates the separating velocity at this contact.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public double CalculateSeparatingVelocity()
        {
            Vector3 relativeVelocity = Particles[0].Velocity;
            if (Particles[1] != null)
            {
                relativeVelocity -= Particles[1].Velocity;
            }

            //dot product notation
            return relativeVelocity * ContactNormal;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the impulse calculations for this collision.
        /// </summary>
        /// <param name="duration"></param>
        private void ResolveVelocity(double duration)
        {
            //Find the velocity in the direction of the contact.
            double separatingVelocity = CalculateSeparatingVelocity();

            if(separatingVelocity > 0)
            {
                //The contact is either separating or stationary.
                return;
            }

            //Calculate the new separating velocity
            double newSepVelocity = -separatingVelocity * Restitution;

            //Check the velocity buildup due to acceleration only.
            Vector3 accCausedVelocity = Particles[0].Acceleration;
            if (Particles[1] != null)
            {
                accCausedVelocity -= Particles[1].Acceleration;
            }

            double accCausedSeparatingVelocity = accCausedVelocity * ContactNormal * duration;

            //If we've got a closing velocity due to acceleration buildup,
            //remove it from the new separating velocity.

            if(accCausedSeparatingVelocity < 0)
            {
                newSepVelocity += Restitution * accCausedSeparatingVelocity;

                //Make sure we haven't removed more than was there to remove.
                if (newSepVelocity < 0)
                    newSepVelocity = 0;
            }

            double deltaVelocity = newSepVelocity - separatingVelocity;

            //Apply the change in velocity to each object in proportion to their inverse
            //mass (i.e those with lower inverse mass [higher actual mass] get less change
            //in velocity.
            double totalInverseMass = Particles[0].GetInverseMass();
            if (Particles[1] != null)
                totalInverseMass += Particles[1].GetInverseMass();

            //If all particles have infinite mass, then impulses have no effect.
            if (totalInverseMass <= 0) return;

            //Calculate the impulse too apply.
            double impulse = deltaVelocity / totalInverseMass;

            //Find the amount of impulse per unit of inverse mass.
            Vector3 impulsePerMass = ContactNormal * impulse;

            //Apply impulses: they are applied in the direction of the contact,
            //and are proportional to the inverse mass.
            Particles[0].Velocity = Particles[0].Velocity + impulsePerMass * Particles[0].GetInverseMass();

            if (Particles[1] != null)
            {
                Particles[1].Velocity = Particles[1].Velocity + impulsePerMass * Particles[1].GetInverseMass();
            }
        }

        private void ResolveInterpenetration(double duration)
        {
            //If we don't have any penetration, skip this step.
            if (Penetration <= 0) return;

            double totalInverseMass = Particles[0].GetInverseMass();
            //The movement of each object is based on their inverse mass, so we find the total.
            if (Particles[1] != null)
            {
                totalInverseMass += Particles[1].GetInverseMass();
            }

            //If all particles have infinite mass, then do nothing.
            if (totalInverseMass <= 0) return;

            //Find the amount of penetration resolution per unit of inverse mass.
            Vector3 movementPerInverseMass = ContactNormal * (Penetration / totalInverseMass);

            //Calculate the movement amounts.
            Vector3[] particleMovements = new Vector3[2];

            particleMovements[0] = movementPerInverseMass * Particles[0].GetInverseMass();
            if (Particles[1] != null)
            {
                particleMovements[1] = movementPerInverseMass * -Particles[1].GetInverseMass();
            }

            //Apply the penetration resolution.
            Particles[0].Position = Particles[0].Position + particleMovements[0];

            if (Particles[1] != null)
                Particles[1].Position = Particles[1].Position + particleMovements[1];
        }

        #endregion
    }
}
