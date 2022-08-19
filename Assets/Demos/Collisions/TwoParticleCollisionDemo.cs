using Assets.Cyclone.ForceGenerators;
using Assets.Cyclone.Particles.Collisions;
using Cyclone.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vec3 = Cyclone.Core.Vector3;

namespace Assets.Demos.Collisions
{
    public class TwoParticleCollisionDemo : MonoBehaviour
    {
        private ParticleContactResolver _contactResolver;
        private ParticleContact _contact;
        private ParticleContact[] _contacts;
        private Particle _particle1;
        private Particle _particle2;
        private GameObject _particle2GameObject;

        private void Start()
        {
            _particle2GameObject = GameObject.Find("Particle2");
            _particle1 = new Particle();
            _particle1.SetMass(10);
            _particle1.Position = new Vec3(15, 0, 0);
            _particle1.Velocity = new Vec3(-10, 0, 0);
            _particle1.Acceleration = Vec3.ZeroVector;

            _particle2 = new Particle();
            _particle2.SetMass(20);
            _particle2.Position = new Vec3(-15, 0, 0);
            _particle2.Velocity = new Vec3(8, 0, 0);
            _particle2.Acceleration = Vec3.ZeroVector;

            _contact = new ParticleContact();
            _contact.Particles[0] = _particle1;
            _contact.Particles[1] = _particle2;
            _contact.Restitution = 1;
            _contact.ContactNormal = new Vec3(1, 0, 0);
            _contact.Penetration = 0;

            _contacts = new ParticleContact[1] { _contact };

            //Initialize contact resolver.
            _contactResolver = new ParticleContactResolver();
            _contactResolver.Iterations = 5;
        }

        private void Update()
        {
            _particle1.Integrate(Time.deltaTime);
            _particle2.Integrate(Time.deltaTime);

            if (_particle1.Position.X == _particle2.Position.X)
            {
                _contactResolver.ResolveContacts(_contacts, (uint)_contacts.Length, Time.deltaTime);
            }
            HelperFunctions.SetObjectPosition(_particle1.Position, transform);
            HelperFunctions.SetObjectPosition(_particle2.Position, _particle2GameObject.transform);
        }
    }
}
