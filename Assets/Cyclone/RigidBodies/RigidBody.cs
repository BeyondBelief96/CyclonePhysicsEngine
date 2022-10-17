using Assets.Cyclone.Core;
using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.Cyclone.RigidBodies
{
    /// <summary>
    /// A rigid body is the basic simulation object in the physics engine.
    /// </summary>
    public class RigidBody
    {
        #region Fields

        /// <summary>
        /// Holds the accumulated force to be applied at the next
        /// simulation iteration only. This value is zeroed out at each
        /// integration time step.
        /// </summary>
        private Vector3 ForceAccumulated;

        /// <summary>
        /// Holds the accumulated torque to be applied at the next
        /// simulation iteration only. This value is zeroed out at each
        /// integration time step.
        /// </summary>
        private Vector3 TorqueAccumulated;

        /// <summary>
        /// Holds the inverse mass of the Rigid Body. It is more useful to hold the
        /// inverse mass because integration is simpler, and in double-time
        /// simulation it is more useful to have objects with infinite mass (immovable)
        /// than zero mass (unstable in numerical simulation).
        /// When InverseMass = 0, object is immovable. (1/M).
        /// </summary>
        private double InverseMass;

        #endregion

        #region Properties

        /// <summary>
        /// Holds the linear position of the Rigid Body in world space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Holds the linear velocity of the Rigid Body in world space.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Holds the acceleration of the Rigid Body. This value can be used
        /// to set the acceleration due to gravity or any other constant acceleration.
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// Holds the angular orientation of the rigid body in world space.
        /// </summary>
        public Quaternion Orientation { get; set; }

        /// <summary>
        /// Holds the angular velocity, or rotation, of the rigid body
        /// in world space.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Holds the transformation matrix for converting body space into
        /// world space and vice versa. This can be achieved by calling the 
        /// GetPointInSpace functions.
        /// </summary>
        public Matrix4 TransformMatrix { get; set; }

        /// <summary>
        /// Holds the inverse of the body's inertia tensor (In body coordinates). The
        /// inertia tensor provided must not be degenerate
        /// (that would mean the body had zero inertia for spinning
        /// along one axis). As long as the tensor is finite, it will be invertible.
        /// The inverse tensor is used for similar reasons we use the inverse of mass.
        /// </summary>
        public Matrix3 InverseInertiaTensor { get; set; }

        /// <summary>
        /// Holds the inverse of the body's inertia tensor (In world coordinates). The
        /// inertia tensor provided must not be degenerate
        /// (that would mean the body had zero inertia for spinning
        /// along one axis). As long as the tensor is finite, it will be invertible.
        /// The inverse tensor is used for similar reasons we use the inverse of mass.
        /// </summary>
        public Matrix3 InverseInertiaTensorWorld { get; set; }

        /// <summary>
        /// A quantity used to store the amount of damping applied
        /// to linear motion. Damping is required to remove energy added through
        /// numerical instability in the integrator.
        /// </summary>
        public double Damping { get; set; } = 0.99f;

        /// <summary>
        /// Holds the amount of damping applied to angular motion.
        /// Damping is required to remove energy added through numerical
        /// instability in the integrator.
        /// </summary>
        public double AngularDamping { get; set; } = 0.99f;

        /// <summary>
        /// Returns true if the mass of the Rigid Body is finite.
        /// </summary>
        public bool HasFiniteMass => InverseMass != 0;

        /// <summary>
        /// Return true if the mass of the Rigid Body is infinite.
        /// </summary>
        public bool HasInfiniteMass => InverseMass == 0;

        public bool IsAwake { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the mass of the rigid body.
        /// </summary>
        /// <returns></returns>
        public double GetMass()
        {
            return 1.0 / InverseMass;
        }

        /// <summary>
        /// Calculates internal data from state data. This should be called after
        /// a bodys state is altered directly (it is called automatically during integration).
        /// If you change the body's state and then intend to integrate before querying any data
        /// (such as the transform matrix), then you can omit this step.
        /// </summary>
        public void CalculateDerivedData()
        {
            Orientation.Normalize();
            
            //Calculate the transform matrix for the body.
            CalculateTransformMatrix(TransformMatrix, Position, Orientation);

            //Calculate the inertia tensor in world space.
            TransformInertiaTensor(Orientation, InverseInertiaTensor, TransformMatrix);
        }

        public void SetInertiaTensor(Matrix3 inertiaTensor)
        {
            if (!inertiaTensor.Invertible) return;
            InverseInertiaTensor = inertiaTensor.Inverse();
        }

        /// <summary>
        /// Transforms the given point on the rigid body from body coordinates
        /// to world coordinates.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3 GetPointInWorldSpace(Vector3 point)
        {
            return point.LocalToWorld(point, TransformMatrix);
        }

        /// <summary>
        /// Adds the given force to the center of mass of the rigid body.
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3 force)
        {
            ForceAccumulated += force;
            IsAwake = true;
        }

        /// <summary>
        /// Adds the given force to the given point on the rigid body.
        /// Both the force and the application point are given in world space.
        /// Because the force is not applied at the center of mass, it may be split into 
        /// both a force and torque.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="point"></param>
        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            //Convert to coordinates relative to center of mass.
            Vector3 pt = point;
            pt -= Position;

            ForceAccumulated += force;

            //Recall % is overrided to do the cross product.
            TorqueAccumulated += pt % force;

            IsAwake = true;
        }

        /// <summary>
        /// Adds the given force tot he given point on the rigid body.
        /// The direction of the force is given in world coordinates,
        /// but the application point is given in body space. This is useful
        /// for spring forces, or other forces fixed to the body.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="point"></param>
        public void AddForceAtBodyPoint(Vector3 force, Vector3 point)
        {
            //Convert to coordinates relative to center of mass
            //Vector3 pt = GetPointInWorldSpace(point);
            //AddForceAtPoint(force, pt);

            //IsAwake = true;
        }

        /// <summary>
        /// Clears the forces and torques in the accumulators. This will
        /// be called automatically after each integration step.
        /// </summary>
        public void ClearAccumulators()
        {
            ForceAccumulated = Vector3.ZeroVector;
            TorqueAccumulated = Vector3.ZeroVector;
        }

        /// <summary>
        /// Integrates the rigid body forward in time by the given time-interval "dt".
        /// This function uses a Newton-Euler integration method, which is a linear 
        /// approximation to the correct integral. For this reason it may be inaccurate
        /// in some cases.
        /// </summary>
        /// <param name="dt"></param>
        public void Integrate(double duration)
        {
            //Calculate the linear acceleration from force inputs.
            var lastFrameAcceleration = Acceleration;
            lastFrameAcceleration.AddScaledVector(ForceAccumulated, InverseMass);

            //Calculate the angular acceleration from torque inputs.
            Vector3 angularAcceleration = InverseInertiaTensorWorld.Transform(TorqueAccumulated);

            //Adjust velocities
            //Update linear velocity from both acceleration and impulse.
            Velocity = Velocity.AddScaledVector(lastFrameAcceleration, duration);

            //Update angular velocity from both acceleration and impulse.
            Rotation = Rotation.AddScaledVector(angularAcceleration, duration);

            //Impose drag
            Velocity *= Math.Pow(Damping, duration);
            Rotation *= Math.Pow(AngularDamping, duration);

            //Adjust positions
            //Update linear position
            Position = Position.AddScaledVector(Velocity, duration);

            //Update angular Position
            Orientation.AddScaledVector(Rotation, duration);

            //Normalize the orientation, and update the matrices with the new position and orientation.
            CalculateDerivedData();

            //Clear Accumulators
            ClearAccumulators();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Function that creates a transform matrix from a position and orientation.
        /// </summary>
        /// <param name="transformMatrix"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        private void CalculateTransformMatrix(Matrix4 transformMatrix,
            Vector3 position, Quaternion orientation)
        {
            transformMatrix.Data[0] = 1 - 2 * orientation.J * orientation.J -
            2 * orientation.K * orientation.K;
            transformMatrix.Data[1] = 2 * orientation.I * orientation.J -
            2 * orientation.R * orientation.K;
            transformMatrix.Data[2] = 2 * orientation.I * orientation.K +
            2 * orientation.R * orientation.J;
            transformMatrix.Data[3] = position.X;
            transformMatrix.Data[4] = 2 * orientation.I * orientation.J +
            2 * orientation.R * orientation.K;
            transformMatrix.Data[5] = 1 - 2 * orientation.I * orientation.I -
            2 * orientation.K * orientation.K;
            transformMatrix.Data[6] = 2 * orientation.J * orientation.K -
            2 * orientation.R * orientation.I;
            transformMatrix.Data[7] = position.Y;
            transformMatrix.Data[8] = 2 * orientation.I * orientation.K -
            2 * orientation.R * orientation.J;
            transformMatrix.Data[9] = 2 * orientation.J * orientation.K +
            2 * orientation.R * orientation.I;
            transformMatrix.Data[10] = 1 - 2 * orientation.I * orientation.I -
            2 * orientation.J * orientation.J;
            transformMatrix.Data[11] = position.Z;

            TransformMatrix = transformMatrix;
        }

        public void TransformInertiaTensor(Quaternion q, Matrix3 iitBody,
            Matrix4 rotMat)
        {
            double t4 = rotMat.Data[0] * iitBody.Data[0] +
            rotMat.Data[1] * iitBody.Data[3] +
            rotMat.Data[2] * iitBody.Data[6];

            double t9 = rotMat.Data[0] * iitBody.Data[1] +
            rotMat.Data[1] * iitBody.Data[4] +
            rotMat.Data[2] * iitBody.Data[7];

            double t14 = rotMat.Data[0] * iitBody.Data[2] +
            rotMat.Data[1] * iitBody.Data[5] +
            rotMat.Data[2] * iitBody.Data[8];

            double t28 = rotMat.Data[4] * iitBody.Data[0] +
            rotMat.Data[5] * iitBody.Data[3] +
            rotMat.Data[6] * iitBody.Data[6];

            double t33 = rotMat.Data[4] * iitBody.Data[1] +
            rotMat.Data[5] * iitBody.Data[4] +
            rotMat.Data[6] * iitBody.Data[7];

            double t38 = rotMat.Data[4] * iitBody.Data[2] +
            rotMat.Data[5] * iitBody.Data[5] +
            rotMat.Data[6] * iitBody.Data[8];

            double t52 = rotMat.Data[8] * iitBody.Data[0] +
            rotMat.Data[9] * iitBody.Data[3] +
            rotMat.Data[10] * iitBody.Data[6];

            double t57 = rotMat.Data[8] * iitBody.Data[1] +
            rotMat.Data[9] * iitBody.Data[4] +
            rotMat.Data[10] * iitBody.Data[7];

            double t62 = rotMat.Data[8] * iitBody.Data[2] +
            rotMat.Data[9] * iitBody.Data[5] +
            rotMat.Data[10] * iitBody.Data[8];

            InverseInertiaTensorWorld.Data[0] = t4 * rotMat.Data[0] +
            t9 * rotMat.Data[1] +
            t14 * rotMat.Data[2];

            InverseInertiaTensorWorld.Data[1] = t4 * rotMat.Data[4] +
            t9 * rotMat.Data[5] +
            t14 * rotMat.Data[6];

            InverseInertiaTensorWorld.Data[2] = t4 * rotMat.Data[8] +
            t9 * rotMat.Data[9] +
            t14 * rotMat.Data[10];

            InverseInertiaTensorWorld.Data[3] = t28 * rotMat.Data[0] +
            t33 * rotMat.Data[1] +
            t38 * rotMat.Data[2];

            InverseInertiaTensorWorld.Data[4] = t28 * rotMat.Data[4] +
            t33 * rotMat.Data[5] +
            t38 * rotMat.Data[6];

            InverseInertiaTensorWorld.Data[5] = t28 * rotMat.Data[8] +
            t33 * rotMat.Data[9] +
            t38 * rotMat.Data[10];

            InverseInertiaTensorWorld.Data[6] = t52 * rotMat.Data[0] +
            t57 * rotMat.Data[1] +
            t62 * rotMat.Data[2];

            InverseInertiaTensorWorld.Data[7] = t52 * rotMat.Data[4] +
            t57 * rotMat.Data[5] +
            t62 * rotMat.Data[6];

            InverseInertiaTensorWorld.Data[8] = t52 * rotMat.Data[8] +
            t57 * rotMat.Data[9] +
            t62 * rotMat.Data[10];
        }

        #endregion
    }
}
