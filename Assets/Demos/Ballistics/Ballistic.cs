using System.Collections;
using System.Collections.Generic;
using Cyclone.Particles;
using Vec3 = Cyclone.Core.Vector3;
using UnityEngine;
using Assets.Demos;

/// <summary>
/// A class representing a particle that has some initial position, velocity, and acceleration
/// and models it's trajectory through the game world.
/// </summary>
public class Ballistic : MonoBehaviour
{
    #region Private Fields

    private Particle _particle = new Particle();

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

    // Start is called before the first frame update
    void Start()
    {
        //Set the initial velocity
        Velocity = new Vector3(0.0f, 10.0f, 30.0f);

        //Set up acceleration due to gravity.
        Acceleration = new Vector3(0, -9.80f, 0.0f);

        //Give object enough damping to reduce numerical errors.
        Damping = 0.99f;

        //Give particle 2.0kg of mass.
        _particle.SetMass(2.0f);

        //Set up our physics engines particle with the initial position of the object in unity.
        _particle.Position = new Vec3(transform.position.x, transform.position.y, transform.position.z);

        //Set up our physics engines particle with the initial velocity.
        _particle.Velocity = new Vec3(Velocity.x, Velocity.y, Velocity.z);

        //Setup our physics engines initial acceleration.
        _particle.Acceleration = new Vec3(Acceleration.x, Acceleration.y, Acceleration.z);

        //Set up our physics engines damping parameter.
        _particle.Damping = Damping;

        //Initialize unity objects position.
        HelperFunctions.SetObjectPosition(_particle.Position, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(_particle.Position.Y <= 0)
        {
            HelperFunctions.SetObjectPosition(new Vec3(transform.position.x, 0, transform.position.z), transform);
        }
        else
        {
            _particle.Integrate(Time.deltaTime);
            HelperFunctions.SetObjectPosition(_particle.Position, transform);
        }

    }
}
