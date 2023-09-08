using Cyclone.Core;

namespace Assets.Cyclone.CollisionDetection.Primitives
{
    /// <summary>
    /// A sphere primitive. It is completely defined by its center and radius.
    /// The offset contains the information for where the center of the sphere is.
    /// </summary>
    public class Sphere : Primitive
    {
        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public double Radius { get; set; }

        public Sphere(double radius)
        {
            Radius = radius;
        }
    }
}
