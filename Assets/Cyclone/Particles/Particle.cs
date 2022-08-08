using Cyclone.Core;
using System;
using Real = System.Double;

namespace Cyclone.Particles
{
    /// <summary>
    /// A representation of a particle, the simplest object that can be simulated
    /// in the physics engine.
    /// </summary>
    public class Particle
    {
        #region Properties

        /// <summary>
        /// Holds the linear position of the particle in world space.
        /// </summary>
        public Vector3 Position { get; set; }


        /// <summary>
        /// Holds the linear velocity of the particle in world space.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Holds the acceleration of the particle. This value can be used
        /// to set the acceleration due to gravity or any other constant acceleration.
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// A quantity used to store the amount of damping applied
        /// to linear motion. Damping is required to remove energy added through
        /// numerical instability in the integrator.
        /// </summary>
        public Real Damping { get; set; } = 0.99f;

        /// <summary>
        /// Holds the accumulated force to be applied at the next
        /// simulation iteration only. This value is zeroed out at each
        /// integration time step.
        /// </summary>
        public Vector3 ForceAccumulated { get; set; }

        /// <summary>
        /// Returns true if the mass of the particle is finite.
        /// </summary>
        public bool HasFiniteMass => InverseMass != 0;

        /// <summary>
        /// Return true if the mass of the particle is infinite.
        /// </summary>
        public bool HasInfiniteMass => InverseMass == 0;

        /// <summary>
        /// Holds the inverse mass of the particle. It is more useful to hold the
        /// inverse mass because integration is simpler, and because in real-time
        /// simulation it is more useful to have objects with infinite mass (immovable)
        /// than zero mass (unstable in numerical simulation).
        /// When InverseMass = 0, object is immovable. (1/M).
        /// </summary>
        protected Real InverseMass { get; set; }

        #endregion

        #region Ctor

        public Particle()
        {
            Position = Vector3.ZeroVector;
            Velocity = Vector3.ZeroVector;
            Acceleration = Vector3.ZeroVector;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the mass of the particle.
        /// </summary>
        /// <returns></returns>
        public double GetMass()
        {
            if (InverseMass == 0)
                return double.PositiveInfinity;
            else
                return 1.0 / InverseMass;
        }

        /// <summary>
        /// Set the mass of the particle. 
        /// </summary>
        public void SetMass(double mass)
        {
            if (mass <= 0)
                InverseMass = 0;
            else
                InverseMass = 1.0 / mass;
        }

        /// <summary>
        /// Setter function to set the InverseMass of the particle directly.  
        /// </summary>
        /// <param name="m">The inverse of the mass you're trying to set.</param>
        public void SetInverseMass(Real m)
        {
            InverseMass = m;
        }

        /// <summary>
        /// Adds the given force to the particle to be applied at the next iteration only.
        /// Based on D'alemberts Principle.
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3 force)
        {
            ForceAccumulated += force;
        }

        /// <summary>
        /// Clears out the accumulated force from the previous time iteration.
        /// </summary>
        public void ClearAccumulator()
        {
            ForceAccumulated = new Vector3();
        }

        /// <summary>
        /// Integrates the particle forward in time by the given time-interval "dt".
        /// This function uses a Newton-Euler integration method, which is a linear 
        /// approximation to the correct integral. For this reason it may be inaccurate
        /// in some cases.
        /// </summary>
        /// <param name="dt"></param>
        public void Integrate(Real dt)
        {
            //Don't integrate objects with infinite mass.
            if (InverseMass <= 0.0f) return;

            //Update linear position
            Position += Velocity * dt;

            //Work out the acceleration from the force.
            Vector3 resultingAcceleration = Acceleration + ForceAccumulated * InverseMass;

            //Update linear velocity from the acceleration.
            Velocity += resultingAcceleration * dt;

            //Impose drag to reduce accumulation error.
            Velocity *= Math.Pow(Damping, dt);

            //Clear out previous accumulated force.
            ClearAccumulator();
        }

        /// <summary>
        /// Method to easily print the position vector of a particle to the console. 
        /// Mainly used for testing.
        /// </summary>
        /// <returns></returns>
        public void LogPosition()
        {
            Console.WriteLine($"Position: <{Position.X}, {Position.Y}, {Position.Z}>");
        }

        /// <summary>
        /// Method to easily print the velocity vector of a particle to the console. 
        /// Mainly used for testing.
        /// </summary>
        /// <returns></returns>
        public void LogVelocity()
        {
            Console.WriteLine($"Velocity: <{Velocity.X}, {Velocity.Y}, {Velocity.Z}>");
        }

        /// <summary>
        /// Method to easily print the acceleration vector of a particle to the console. 
        /// Mainly used for testing.
        /// </summary>
        /// <returns></returns>
        public void LogAcceleration()
        {
            Console.WriteLine($"Acceleration: <{Acceleration.X}, {Acceleration.Y}, {Acceleration.Z}>");
        }

        #endregion
    }
}
