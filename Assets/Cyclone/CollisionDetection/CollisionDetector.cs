using Assets.Cyclone.CollisionDetection.Primitives;
using Cyclone.Core;

namespace Assets.Cyclone.CollisionDetection
{
    /// <summary>
    /// A wrapper class that holds the fine grained collision detection
    /// routines.
    ///
    /// Each of the functions has the same format: it takes the details
    /// of two objects, and a pointer to a contact array to fill.It
    /// returns the number of contacts it wrote into the array.
    /// </summary>
    public static class CollisionDetector
    {
        public static void SphereAndSphere(Sphere one, Sphere two, CollisionData data)
        {
            // Make sure we have contacts
            if (data.NoMoreContacts()) return;

            // Cache the sphere positions
            Vector3 positionOne = one.GetAxis(3);
            Vector3 positionTwo = two.GetAxis(3);

            // Find the vector between the objects
            Vector3 midline = positionOne - positionTwo;
            double size = midline.Magnitude();

            // See if it is large enough.
            if (size <= 0.0f || size >= one.Radius + two.Radius)
                return;

            // We manually create the normal, because we have the
            // size to hand.
            Vector3 normal = midline * (1.0 / size);

            var contact = data.GetContact();
            contact.ContactNormal = normal;
            contact.ContactPoint = positionOne + midline * 0.5;
            contact.Penetration = one.Radius + two.Radius - size;
            contact.SetBodyData(one.Body, two.Body, data.Friction, data.Restitution);
        }
    }
}
