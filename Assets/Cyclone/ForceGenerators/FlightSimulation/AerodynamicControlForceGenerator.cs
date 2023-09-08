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
    /// A force generator with a control aerodynamic surface. This requires three inertia tensors,
    /// for the two extremes and 'resting' position of the control surface. The latter tensor is
    /// the one inherited from the base class, while the two extremes are defined in this class.
    /// </summary>
    public class AerodynamicControlForceGenerator : AerodynamicForceGenerator
    {
        #region Fields

        /// <summary>
        /// The aerodynamic tensor for the surface when the control is at its maximum value.
        /// </summary>
        private Matrix3 _maxTensor;

        /// <summary>
        /// The aerodynamic tensor for the surface when the control is at its minimum value.
        /// </summary>
        private Matrix3 _minTensor;

        /// <summary>
        /// The current position of the control for this surface. This should range between -1
        /// (in which case the minTensor value is used) through 0 (where the base class tensor is used)
        /// to +1 (where the maxTensor is used).
        /// </summary>

        private double _controlSetting;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new aerodynamic control surface with the given properties.
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="position"></param>
        /// <param name="windSpeed"></param>
        public AerodynamicControlForceGenerator(Matrix3 baseTensor, Matrix3 min, Matrix3 max,
            Vector3 position, Vector3 windSpeed) : base(baseTensor, position, windSpeed)
        {
            _maxTensor = max;
            _minTensor = min;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the control position of this control. This should be on the range of -1, 0, +1.
        /// </summary>
        /// <param name="value"></param>
        public void SetControl(double value)
        {
            _controlSetting = value;
        }

        /// <summary>
        /// Applies the force to the given rigid body.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        public new virtual void UpdateForce(RigidBody body, double duration)
        {
            Matrix3 tensor = GetTensor();
            UpdateForceFromTensor(body, duration, tensor);
        }

        /// <summary>
        /// Returns the tensor that should be used based on the current value of the control setting.
        /// </summary>
        /// <returns></returns>
        public Matrix3 GetTensor()
        {
            if (_controlSetting <= -1.0f) return _minTensor;
            else if (_controlSetting >= 1.0f) return _maxTensor;
            else if (_controlSetting < 0)
            {
                return Matrix3.LinearInterpolate(_minTensor, _tensor, _controlSetting + 1.0f);
            }
            else if (_controlSetting > 0)
            {
                return Matrix3.LinearInterpolate(_tensor, _maxTensor, _controlSetting + 1.0f);
            }

            else return _tensor;
        }

        #endregion
    }
}
