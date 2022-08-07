using System.Collections;
using System.Collections.Generic;
using Cyclone.Particles;
using Vec3 = Cyclone.Core.Vector3;
using UnityEngine;

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
        Velocity = new Vector3(0.0f, 0.0f, 0.0f);
        Acceleration = new Vector3(0, -9.80f, 0.0f);
        Damping = 0.99f;
        _particle.SetMass(2.0f);
        _particle.Position = new Vec3(transform.position.x, transform.position.y, transform.position.z);
        _particle.Velocity = new Vec3(Velocity.x, Velocity.y, Velocity.z);
        _particle.Acceleration = new Vec3(Acceleration.x, Acceleration.y, Acceleration.z);
        _particle.Damping = Damping;
        SetObjectPosition(_particle.Position);
    }

    // Update is called once per frame
    void Update()
    {
        if(_particle.Position.Y <= 0)
        {
            SetObjectPosition(new Vec3(transform.position.x, 0, transform.position.z));
        }
        else
        {
            _particle.Integrate(Time.deltaTime);
            SetObjectPosition(_particle.Position);
        }

    }

    /// <summary>
    /// Helper method to convert a Cyclone.Math.Vector3 to a UnityEngine.Vector3 position.
    /// </summary>
    /// <param name="position">The position.</param>
    private void SetObjectPosition(Vec3 position)
    {
        transform.position = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
    }
}
