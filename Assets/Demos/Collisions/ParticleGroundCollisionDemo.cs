using Assets.Cyclone.ForceGenerators;
using Assets.Cyclone.Particles.Collisions;
using Assets.Demos;
using Cyclone.Particles;
using UnityEngine;
using Vec3 = Cyclone.Core.Vector3;

public class ParticleGroundCollisionDemo : MonoBehaviour
{
    private IParticleForceGenerator _gravityForceGenerator;
    private ParticleForceRegistry _pfg;
    private ParticleContactResolver _contactResolver;
    private ParticleContact _contact;
    private Particle _particle;

    // Start is called before the first frame update
    void Start()
    {
        //Set up particles initial values.
        _particle = new Particle();
        _particle.SetMass(10);
        _particle.Position = new Vec3(0, 10, 0);
        _particle.Velocity = Vec3.ZeroVector;
        _particle.Acceleration = Vec3.ZeroVector;

        //Initialize gravity force generator.
        _gravityForceGenerator = new ParticleGravityForceGenerator(-9.8);
        _pfg = new ParticleForceRegistry();
        _pfg.AddForceGenerator(_particle, _gravityForceGenerator);

        //Set up contact
        _contact = new ParticleContact();
        _contact.Particles[0] = _particle;
        _contact.Restitution = 1;
        _contact.ContactNormal = new Vec3(0, 1, 0);
        _contact.Penetration = 0;

        //Initialize contact resolver.
        _contactResolver = new ParticleContactResolver();
        _contactResolver.Iterations = 5;
    }

    // Update is called once per frame
    void Update()
    {
        _pfg.UpdateForces(Time.deltaTime);
        _particle.Integrate(Time.deltaTime);

        if(_particle.Position.Y <= 0)
        {
            _contactResolver.ResolveContacts(new ParticleContact[] { _contact }, 1, Time.deltaTime);
        }
        HelperFunctions.SetObjectPosition(_particle.Position, transform);
    }
}
