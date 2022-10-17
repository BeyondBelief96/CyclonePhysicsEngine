using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Core
{
    /// <summary>
    /// Holds a three-degrees-of-freedom orientation.
    /// </summary>
    public class Quaternion
    {
        #region Properties

        
        /// <summary>
        /// Holds the real component of the quaternion.
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Holds the first complex component of the quaternion.
        /// </summary>
        public double I { get; set; }

        /// <summary>
        /// Holds the second complex component of the quaternion.
        /// </summary>
        public double J { get; set; }

        /// <summary>
        /// Holds the third complex component of the quaternion.
        /// </summary>
        public double K { get; set; }

        /// <summary>
        /// Holds the quaternion data in array form.
        /// </summary>
        public double[] Data { get; set; }

        #endregion

        #region Ctor

        public Quaternion(double r, double i, double j, double k)
        {
            R = r;
            I = i;
            J = j;
            K = k;
            Data = new double[4] { r, i, j, k };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalizes the quaternion to unit length, making it a valid
        /// orientation quaternion.
        /// </summary>
        public void Normalize()
        {
            double d = R * R + I * I + J * J * K * K;

            //Check for zero length quaternion, and use the no-rotation quaternion in that case.
            if (d == 0)
            {
                R = 1;
                return;
            }


            d = ((double)1.0)/Mathematics.SafeSqrt(d);
            R *= d;
            I *= d;
            J *= d;
            K *= d;
        }

        /// <summary>
        /// Rotates this quaternion by the given vector.
        /// </summary>
        /// <param name="vector"></param>
        public void RotateByVector(Vector3 vector)
        {
            Quaternion q = new Quaternion(0, vector.X, vector.Y, vector.Z);
            Quaternion result = this * q;
            R = result.R;
            I = result.I;
            J = result.J;
            K = result.K;
        }

        public void AddScaledVector(Vector3 vector, double scale)
        {
            Quaternion q = new Quaternion(0, vector.X * scale, vector.Y * scale, vector.Z * scale);
            q = q * this;
            R += q.R * 0.5;
            I += q.I * 0.5;
            J += q.J * 0.5;
            K += q.K * 0.5;
        }

        #endregion

        #region Operator Overloads

        public static Quaternion operator *(Quaternion q,Quaternion multiplier)
        {
            return new Quaternion(q.R = q.R * multiplier.R - q.I * multiplier.I -
            q.J * multiplier.J - q.K * multiplier.K, q.I = q.R * multiplier.I + q.I * multiplier.R +
            q.J * multiplier.K - q.K * multiplier.J, q.J = q.R * multiplier.J + q.J * multiplier.R +
            q.K * multiplier.I - q.I * multiplier.K, q.K = q.R * multiplier.K + q.K * multiplier.R +
            q.I * multiplier.J - q.J * multiplier.I);
        }


        #endregion
    }
}
