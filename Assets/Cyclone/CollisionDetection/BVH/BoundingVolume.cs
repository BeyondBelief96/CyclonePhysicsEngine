using System;

namespace Assets.Cyclone.CollisionDetection.BVH
{
    public abstract class BoundingVolume
    {
        public abstract double Size { get; set; }

        public abstract bool Overlaps(BoundingVolume other);

        public abstract double GetGrowth(BoundingVolume newVolume);

        public abstract BoundingVolume RecalculateVolume<TBoundingVolume>(BVHNode<TBoundingVolume> node1, BVHNode<TBoundingVolume> node2);
    }
}
