using Assets.Cyclone.RigidBodies;
using Cyclone.Core;
using System;

namespace Assets.Cyclone.CollisionDetection
{
    /// <summary>
    /// A contact represents two bodies in contact. Resolving a
    /// contact removes their interpenetration, and applies sufficient
    /// impulse to keep them apart. Colliding bodies may also rebound.
    /// Contacts can be used to represent positional joints, by making
    /// the contact constraint keep the bodies in their correct
    /// orientation.
    /// </summary>
    public class Contact
    {
        #region Properties

        /// <summary>
        /// Holds the bodies that are involved in the contact.
        /// The second of these can be NULL, for contacts with the environment.
        /// </summary>
        public RigidBody[] Body = new RigidBody[2];

        /// <summary>
        /// Holds the position of the contact in world coordinates.
        /// </summary>
        public Vector3 ContactPoint;

        /// <summary>
        /// Holds the direction of the contact in world coordinates.
        /// </summary>
        public Vector3 ContactNormal;

        /// <summary>
        /// Holds the depth of penetration at the contact point. If both
        /// bodies are specified then the contact point should be midway
        /// between the inter-penetrating points.
        /// </summary>
        public double Penetration;

        /// <summary>
        /// Holds the lateral friction coefficient at the contact.
        /// </summary>
        public double Friction;

        /// <summary>
        /// Holds the normal restitution coefficient at the contact.
        /// </summary>
        public double Restitution;

        public void SetBodyData(RigidBody body1, RigidBody body2, double friction, double restitution)
        {
            Body[0] = body1;
            Body[1] = body2;
            Friction = friction;
            Restitution = restitution;
        }

        #endregion
    }
}
