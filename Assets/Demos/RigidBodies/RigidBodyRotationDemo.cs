using Assets.Cyclone.ForceGenerators;
using Assets.Cyclone.RigidBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vec3 = Cyclone.Core.Vector3;

namespace Assets.Demos.RigidBodies
{
    public class RigidBodyRotationDemo : MonoBehaviour
    {
        #region Fields

        private int counter = 0;
        private RigidBody _rigidBody = new RigidBody();
        private RigidBodyForceRegistry _registry = new RigidBodyForceRegistry();
        private Gravity _gravityGenerator = new Gravity(new Vec3(0, -9.8, 0));

        #endregion

        #region Unity Properties

        /// <summary>
        /// The Position of the rigid body.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The Velocity of the rigid body.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// The Acceleration of the rigid body.
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// The angular acceleration of the rigid body.
        /// </summary>
        public Vector3 AngularAcceleration { get; set; }

        /// <summary>
        /// The orientation of the rigid body.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The linear damping of the rigid body.
        /// </summary>
        public float Damping { get; set; }


        /// <summary>
        /// The angular damping of the rigid body.
        /// </summary>
        public float AngularDamping { get; set; }

        #endregion

        private void Start()
        {
            _rigidBody.SetMass(1.0);
            double[] inertiaTensor = new double[9] { 2.0/3, -1.0/4, -1.0/4, -1.0/4, 2.0/3, -1.0/4,
            -1.0/4, -1.0/4, 2.0/3};
            _rigidBody.Position = new Vec3(0, 10, 0);
            _rigidBody.Velocity = new Vec3(0, 0, 0);
            _rigidBody.Acceleration = new Vec3(0, 0, 0);
            _rigidBody.SetInertiaTensor(new Cyclone.Core.Matrix3(inertiaTensor));

            //_registry.AddForceGenerator(_rigidBody, _gravityGenerator);

        }

        private void Update()
        {
            _rigidBody.CalculateDerivedData();
            _registry.UpdateForces(Time.deltaTime);
            
            while(counter < 100)
            {
                _rigidBody.AddForceAtBodyPoint(new Vec3(-1, 0, 0), new Vec3(0.5, 0, 0));
                counter++;
            }
            

            //Integrate too calculate new position and velocity.
            _rigidBody.Integrate(Time.deltaTime);

            //Ground is at 1 m
            if (_rigidBody.Position.Y <= 1)
            {
                _rigidBody.Position = new Vec3(_rigidBody.Position.X, 1, _rigidBody.Position.Z);
            }

            //Update unity objects position.
            HelperFunctions.SetObjectPosition(_rigidBody.Position, transform);

            //Update unity object orientation.
            HelperFunctions.SetObjectOrientation(_rigidBody.Orientation, transform);
        }
    }
}
