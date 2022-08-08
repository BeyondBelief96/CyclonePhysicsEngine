using Vec3 = Cyclone.Core.Vector3;
using UnityEngine;

namespace Assets.Demos
{
    public static class HelperFunctions
    {
        /// <summary>
        /// Helper method to convert a Cyclone.Math.Vector3 to a UnityEngine.Vector3 position.
        /// </summary>
        /// <param name="position">The position.</param>
        public static void SetObjectPosition(Vec3 position, Transform transform)
        {
            transform.position = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
        }
    }
}
