using Assets.Cyclone.Core;
using Assets.Cyclone.RigidBodies;
using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.ForceGenerators.FlightSimulation
{
    /// <summary>
    /// A force generator that applies an aerodynamic force.
    /// </summary>
    public class AerodynamicForceGenerator : IForceGenerator
    {
        #region Fields

        /// <summary>
        /// Holds the aerodynamic tensor for the surface in body space.
        /// </summary>
        protected Matrix3 _tensor;

        /// <summary>
        /// Holds the relative position of the aerodynamic surface in body coordinates.
        /// </summary>
        private Vector3 _position;

        /// <summary>
        /// Holds a vector containing the wind speed of the environment. This is easier than
        /// managing a separate wind speed vector per generator and having to update it manually
        /// as the wind changes.
        /// </summary>
        private Vector3 _windSpeed;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new aerodynamic force generator with the given properties.
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="position"></param>
        /// <param name="windSpeed"></param>
        public AerodynamicForceGenerator(Matrix3 tensor, Vector3 position, Vector3 windSpeed)
        {
            _tensor = tensor;
            _position = position;
            _windSpeed = windSpeed;
        }

        #endregion

        #region IForceGenerator Implementation

        /// <summary>
        /// Applies the force to the given rigid body.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateForce(RigidBody body, double duration)
        {
            UpdateForceFromTensor(body, duration, _tensor);
        }

        /// <summary>
        /// Uses an explicit tensor matrix to update the force on the given rigid body. This is
        /// exactly the same as for UpdateForce, except that it takes an explicit tensor.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        /// <param name="tensor"></param>
        public void UpdateForceFromTensor(RigidBody body, double duration, Matrix3 tensor)
        {
            //Calculate total velocity (wind speed and body's velocity).
            Vector3 velocity = body.Velocity;
            velocity += _windSpeed;

            //Calculate the velocity in body coordinates.
            Vector3 bodyVel = body.TransformMatrix.TransformInverseDirection(velocity);

            //Calculate the force in body coordinates.
            Vector3 bodyForce = tensor.Transform(bodyVel);
            Vector3 force = body.TransformMatrix.TransformDirection(bodyForce);

            //Apply the force.
            body.AddForceAtPoint(force, _position);
        }

        #endregion
    }
}
