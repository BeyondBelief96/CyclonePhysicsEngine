using Assets.Cyclone.ForceGenerators;
using Cyclone.Particles;
using Vec3 = Cyclone.Core.Vector3;
using UnityEngine;

namespace Assets.Demos.Springs
{
    public class SpringBehaivor : MonoBehaviour
    {
        #region Fields

        //Particle at one end of the spring.
        private Particle _particle = new Particle();

        //Particle Force Registry
        private ParticleForceRegistry pfg = new ParticleForceRegistry();

        //Spring Force Generator.
        private IParticleForceGenerator _anchoredSpringForceGenerator;
        private IParticleForceGenerator _dragForceGenerator;

        #endregion

        #region Unity Functions

        private void Start()
        {
            //Set up particles initial values at end of spring.
            _particle.SetMass(1.0); //1.0 kg
            _particle.Position = new Vec3(transform.position.x, transform.position.y, transform.position.z);
            _particle.Velocity = new Vec3(0.0f, 0.0f, 0.0f);
            _particle.Acceleration = new Vec3(0.0f, 0.0f, 0.0f);


            //Spring Anchor is located 10 meters high.
            var springAnchorPosition = new Vec3(0, 10, 0);
            var springConstant = 5.0;
            var restLength = 5.0;
            _anchoredSpringForceGenerator = new ParticleAnchoredSpringForceGenerator(springAnchorPosition, springConstant, restLength);
            _dragForceGenerator = new ParticleDragForceGenerator(0.3, 0.0);
            pfg.AddForceGenerator(_particle, _anchoredSpringForceGenerator);
            pfg.AddForceGenerator(_particle, _dragForceGenerator);
            
        }

        private void Update()
        {
            pfg.UpdateForces(Time.deltaTime);
            _particle.Integrate(Time.deltaTime);
            HelperFunctions.SetObjectPosition(_particle.Position, transform);
        }

        #endregion
    }
}
