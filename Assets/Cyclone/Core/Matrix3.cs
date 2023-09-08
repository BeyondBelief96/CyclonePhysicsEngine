using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Core
{
    /// <summary>
    /// Holds a 3x3 row major matrix representing a transformation in
    /// 3D space that does not include a translational component. This matrix
    /// is not padded to produce an aligned structure.
    /// </summary>
    public class Matrix3
    {
        #region Fields

        public static Matrix3 ZeroMatrix => new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Holds the matrix elements. This matrix is 3 rows by 3 columns.
        /// </summary>
        public double[] Data = new double[9];

        /// <summary>
        /// Returns the determinant of this matrix.
        /// </summary>
        public double Determinant => CalculateDeterminant(this);

        /// <summary>
        /// Returns whether or not the matrix is invertible based on the value
        /// of the determinant.
        /// </summary>
        public bool Invertible => Determinant == 0 ? false : true;
        #endregion

        #region Ctor

        /// <summary>
        /// Creates a 3x3 matrix using the 9 values passed in.
        /// </summary>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m13"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="m23"></param>
        /// <param name="m31"></param>
        /// <param name="m32"></param>
        /// <param name="m33"></param>
        public Matrix3(double m11, double m12, double m13, double m21, double m22,
            double m23, double m31, double m32, double m33)
        {
            Data[0] = m11;
            Data[1] = m12;
            Data[2] = m13;
            Data[3] = m21;
            Data[4] = m22;
            Data[5] = m23;
            Data[6] = m31;
            Data[7] = m32;
            Data[8] = m33;
        }

        /// <summary>
        /// Creates a 3x3 matrix using the given array.
        /// </summary>
        /// <param name="Data"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Matrix3(double[] data)
        {
            if(data.Length != 9)
                throw new ArgumentOutOfRangeException("Passed in array must contain exactly 9 values.");

            Data = data;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transform the given vector by this matrix.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 Transform(Vector3 v)
        {
            return this * v;
        }

        /// <summary>
        /// Sets this matrix to be the inverse of itself.
        /// </summary>
        public void SetInverse()
        {
            if (!Invertible) return;

            double t1 = Data[0] * Data[4];
            double t2 = Data[0] * Data[5];
            double t3 = Data[1] * Data[3];
            double t4 = Data[2] * Data[3];
            double t5 = Data[1] * Data[6];
            double t6 = Data[2] * Data[6];

            double invd = 1.0 / Determinant;

            Data[0] = (Data[4] * Data[8] - Data[5] * Data[7]) * invd;
            Data[1] = -(Data[1] * Data[8] - Data[2] * Data[7]) * invd;
            Data[2] = (Data[1] * Data[5] - Data[2] * Data[4]) * invd;
            Data[3] = -(Data[3] * Data[8] - Data[5] * Data[6]) * invd;
            Data[4] = (Data[0] * Data[8] - t6) * invd;
            Data[5] = -(t2 - t4) * invd;
            Data[6] = (Data[3] * Data[7] - Data[4] * Data[6]) * invd;
            Data[7] = -(Data[0] * Data[7] - t5) * invd;
            Data[8] = (t1 - t3) * invd;

        }

        /// <summary>
        /// Returns a new matrix containg the inverse of this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix3 Inverse()
        {
            Matrix3 result = this;
            result.SetInverse();
            return result;
        }

        /// <summary>
        /// Sets this matrix to be the transpose of itself.
        /// </summary>
        public void SetTranspose()
        {
            Data[0] = Data[0];
            Data[1] = Data[3];
            Data[2] = Data[6];
            Data[3] = Data[1];
            Data[4] = Data[4];
            Data[5] = Data[7];
            Data[6] = Data[2];
            Data[7] = Data[5];
            Data[8] = Data[8];
        }

        /// <summary>
        /// Returns a new matrix which is the transpose of this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix3 Transpose()
        {
            Matrix3 result = this;
            result.SetTranspose();
            return result;
        }

        /// <summary>
        /// Sets this matrix to be the rotation matrix corresponding to
        /// the given quaternion.
        /// </summary>
        public void SetOrientation(Quaternion q)
        {
            Data[0] = 1 - (2 * q.J * q.J + 2 * q.K * q.K);
            Data[1] = 2 * q.I * q.J + 2 * q.K * q.R;
            Data[2] = 2 * q.I * q.K - 2 * q.J * q.R;
            Data[3] = 2 * q.I * q.J - 2 * q.K * q.R;
            Data[4] = 1 - (2 * q.I * q.I + 2 * q.K * q.K);
            Data[5] = 2 * q.J * q.K + 2 * q.I * q.R;
            Data[6] = 2 * q.I * q.K + 2 * q.J * q.R;
            Data[7] = 2 * q.J * q.K - 2 * q.I * q.R;
            Data[8] = 1 - (2 * q.I * q.I + 2 * q.J * q.J);
        }

        /// <summary>
        /// Interpolates a couple of matrices.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static Matrix3 LinearInterpolate(Matrix3 a, Matrix3 b, double prop)
        {
            Matrix3 result = Matrix3.ZeroMatrix;
            double omp = 1.0 - prop;
            for (uint i = 0; i < 9; i++)
            {
                result.Data[i] = a.Data[i] * omp + b.Data[i] * prop;
            }

            return result;
        }

        #endregion

        #region Private Methods


        private double CalculateDeterminant(Matrix3 m)
        {
            double t1 = m.Data[0] * m.Data[4];
            double t2 = m.Data[0] * m.Data[5];
            double t3 = m.Data[1] * m.Data[3];
            double t4 = m.Data[2] * m.Data[3];
            double t5 = m.Data[1] * m.Data[6];
            double t6 = m.Data[2] * m.Data[6];

            double det = (t1 * m.Data[8] - t2 * m.Data[7] - t3 * m.Data[8] +
                t4 * m.Data[7] + t5 * m.Data[5] - t6 * m.Data[4]);

            return det;
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// operator overload for matrix-vector multiplication.
        /// v' = M*v
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 operator *(Matrix3 m, Vector3 v)
        {
            return new Vector3(v.X * m.Data[0] + v.Y * m.Data[1] + v.Z * m.Data[2],
                v.X * m.Data[3] + v.Y * m.Data[4] + v.Z * m.Data[5],
                v.X * m.Data[6] + v.Y * m.Data[7] + v.Z * m.Data[8]);
        }

        /// <summary>
        /// Returns a matrix, which is this one multiplied by the other given matrix.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3(
                m1.Data[0] * m2.Data[0] + m1.Data[1] * m2.Data[3] + m1.Data[2] * m2.Data[6],
                m1.Data[0] * m2.Data[1] + m1.Data[1] * m2.Data[4] + m1.Data[2] * m2.Data[7],
                m1.Data[0] * m2.Data[2] + m1.Data[1] * m2.Data[5] + m1.Data[2] * m2.Data[8],
                m1.Data[3] * m2.Data[0] + m1.Data[4] * m2.Data[3] + m1.Data[5] * m2.Data[6],
                m1.Data[3] * m2.Data[1] + m1.Data[4] * m2.Data[4] + m1.Data[5] * m2.Data[7],
                m1.Data[3] * m2.Data[2] + m1.Data[4] * m2.Data[5] + m1.Data[5] * m2.Data[8],
                m1.Data[6] * m2.Data[0] + m1.Data[7] * m2.Data[3] + m1.Data[8] * m2.Data[6],
                m1.Data[6] * m2.Data[1] + m1.Data[7] * m2.Data[4] + m1.Data[8] * m2.Data[7],
                m1.Data[6] * m2.Data[2] + m1.Data[7] * m2.Data[5] + m1.Data[8] * m2.Data[8]
            );
        }

        #endregion

    }
}
