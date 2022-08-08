using Assets.Cyclone.ForceGenerators;
using Cyclone.Particles;
using UnityEngine;
using Vec3 = Cyclone.Core.Vector3;

namespace Assets.Demos.PushingBlock
{
    public class PushingBlock : MonoBehaviour
    {
        #region Private Fields

        private Particle _particle = new Particle();
        private ParticleForceRegistry pfg = new ParticleForceRegistry();
        private readonly IParticleForceGenerator dfg = new ParticleDragForceGenerator(0.4, 1.0);
        private readonly IParticleForceGenerator gravity = new ParticleGravityForceGenerator(-9.8);

        #endregion

        #region Unity Properties

        /// <summary>
        /// The Position of the particle.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The Velocity of the particle
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// The Acceleration of the particle
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// The damping of the particle
        /// </summary>
        public float Damping { get; set; }

        #endregion

        private void Start()
        {
            //Set up particles initial values.
            Velocity = new Vector3(0.0f, 0.0f, 0.0f);
            Acceleration = new Vector3(0.0f, 0.0f, 0.0f);
            _particle.SetMass(1.0); //1.0 kg
            _particle.Position = new Vec3(transform.position.x, transform.position.y, transform.position.z);
            _particle.Velocity = new Vec3(Velocity.x, Velocity.y, Velocity.z);
            _particle.Acceleration = new Vec3(Acceleration.x, Acceleration.y, Acceleration.z);
            Damping = 0.99f;

            //Set up drag force generator
            pfg.AddForceGenerator(_particle, dfg);
            pfg.AddForceGenerator(_particle, gravity);
        }

        private void Update()
        {
            //Apply a constant 1N force in the z direction.
            _particle.AddForce(new Vec3(0, 0, 5.0));

            //Update external forces applied to particle (such as drag in this case).
            pfg.UpdateForces(Time.deltaTime);

            //Integrate too calculate new position and velocity.
            _particle.Integrate(Time.deltaTime);

            //Ground is at 1 m
            if(_particle.Position.Y <= 1)
            {
                _particle.Position = new Vec3(_particle.Position.X, 1, _particle.Position.Z);
            }

            //Update unity objects position.
            HelperFunctions.SetObjectPosition(_particle.Position, transform);
        }
    }
}
