using Assets.Cyclone.RigidBodies;
using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators
{
    /// <summary>
    /// A force generator that applies a spring force.
    /// </summary>
    public class Spring : IForceGenerator
    {
        #region Fields

        /// <summary>
        /// The point of connection of the spring in local coordinates.
        /// </summary>
        Vector3 connectionPoint;

        /// <summary>
        /// The point of connection of the spring to the other object
        /// in that objects local coordinates.
        /// </summary>
        Vector3 otherConnectionPoint;

        /// <summary>
        /// The rigid body at the other end of the spring.
        /// </summary>
        RigidBody other;

        /// <summary>
        /// Holds the spring constant.
        /// </summary>
        double springConstant;

        /// <summary>
        /// Holds the rest length of the spring.
        /// </summary>
        double restLength;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new spring with the given parameters.
        /// </summary>
        /// <param name="localConnectionPt"></param>
        /// <param name="otherBody"></param>
        /// <param name="otherConnectionPt"></param>
        /// <param name="sc"></param>
        /// <param name="rl"></param>
        public Spring(Vector3 localConnectionPt,
            RigidBody otherBody, Vector3 otherConnectionPt,
            double sc, double rl)
        {
            connectionPoint = localConnectionPt;
            otherConnectionPoint = otherConnectionPt;
            other = otherBody;
            springConstant = sc;
            restLength = rl;
        }

        #endregion

        #region IForceGenerator Implementation

        /// <summary>
        /// Applies the spring force  to the given rigid body.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        public void UpdateForce(RigidBody body, double duration)
        {
            //Calculate the two ends in world space.
            Vector3 lws = body.GetPointInWorldSpace(connectionPoint);
            Vector3 ows = other.GetPointInWorldSpace(otherConnectionPoint);

            //Calculate the vector of the spring
            Vector3 force = lws - ows;

            //Calculate the magnitude of the force.
            double magnitude = force.Magnitude();
            magnitude = Math.Abs(magnitude - restLength);
            magnitude *= springConstant;

            //Calculate the final force and apply it.
            force.Normalize();
            force *= -magnitude;
            body.AddForceAtPoint(force, lws);
        }

        #endregion
    }
}
