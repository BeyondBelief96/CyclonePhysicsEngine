using Cyclone.Core;
using System;
using Vec3 = Cyclone.Core.Vector3;

namespace Assets.Cyclone.CollisionDetection.BVH
{
    public class BoundingSphere : BoundingVolume
    {
        public override double Size { get; set; }

        /// <summary>
        /// The position of the center of this bounding sphere.
        /// </summary>
        public Vec3 Center { get; set; }

        /// <summary>
        /// The radius of this bounding sphere.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Creates a new bounding sphere with the given center and radius.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public BoundingSphere(Vec3 center, double radius)
        {
            Center = center;
            Radius = radius;
            Size = 4 / 3f * Mathematics.PI * radius * radius * radius;
        }

        /// <summary>
        /// Creates a bounding sphere to enclose the two given bounding spheres.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        public BoundingSphere(BoundingSphere one, BoundingSphere two)
        {
            Vec3 centerOffset = two.Center - one.Center;
            double distance = centerOffset.SquareMagnitude;
            double radiusDiff = two.Radius - one.Radius;

            //Check whether the larger sphere encloses the small one.
            if(radiusDiff*radiusDiff >= distance)
            {
                if(one.Radius > two.Radius)
                {
                    Center = one.Center;
                    Radius = one.Radius;
                }
                else
                {
                    Center = two.Center;
                    Radius = two.Radius;
                }
            }

            //Otherwise we need to work with partially overlapping spheres.
            else
            {
                distance = Math.Sqrt(distance);
                Radius = (distance + one.Radius + two.Radius) * 0.5;
                //The new center is based on one's center, moved towards two's center
                //by an amount proportional to the spheres' radii.
                Center = one.Center;
                if(distance > 0)
                {
                    Center += centerOffset * ((Radius - one.Radius) / distance);
                }
            }
        }

        /// <summary>
        /// Checks whether the bounding sphere overlaps with the other given bounding volume.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Overlaps(BoundingVolume other)
        {
            var sphere = other as BoundingSphere;
            double distanceSquared = (Center - sphere.Center).SquareMagnitude;
            return distanceSquared < ((Radius + sphere.Radius) * (Radius + sphere.Radius));
        }

        public override double GetGrowth(BoundingVolume newVolume)
        {
            var sphere = newVolume as BoundingSphere;
            //Calculate the growth of this volume to incorporate the new volume.
            BoundingSphere newSphere = new BoundingSphere(this, sphere);
            return newSphere.Size;
        }

        public override BoundingVolume RecalculateVolume<TBoundingVolume>(BVHNode<TBoundingVolume> node1, BVHNode<TBoundingVolume> node2)
        {
            return new BoundingSphere(node1.Volume as BoundingSphere, node2.Volume as BoundingSphere);
        }
    }
}
