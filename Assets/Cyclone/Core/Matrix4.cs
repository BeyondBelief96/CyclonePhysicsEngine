using Cyclone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cyclone.Core
{
    /// <summary>
    /// Holds a transformation matriX, consisting of a rotation matriX
    /// and a position. The matriX has 12 elements, and it is assumed that the
    /// remaining four are (0, 0, 0, 1), producing a homogeneous matriX.
    /// </summary>
    public class Matrix4
    {
        #region Properties

        /// <summary>
        /// Holds the matriX elements. This matriX has 3 rows by 4 columns.
        /// </summary>
        public double[] Data = new double[12];

        /// <summary>
        /// The determinant of this matriX.
        /// </summary>
        public double Determinant => CalculateDeterminant();

        /// <summary>
        /// Returns whether or not the matriX is invertible based on the value
        /// of the determinant.
        /// </summary>
        public bool Invertible => Determinant == 0 ? false : true;

        #endregion

        /// <summary>
        /// Initializes and empty 3X4 matriX.
        /// </summary>
        public Matrix4()
        {

        }

        #region Ctor

        /// <summary>
        /// Creates a 3X4 matriX using the passsed in values.
        /// </summary>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m13"></param>
        /// <param name="m14"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="m23"></param>
        /// <param name="m24"></param>
        /// <param name="m31"></param>
        /// <param name="m32"></param>
        /// <param name="m33"></param>
        /// <param name="m34"></param>
        public Matrix4(double m11, double m12, double m13, double m14,
            double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34)
        {
            Data[0] = m11;
            Data[1] = m12;
            Data[2] = m13;
            Data[3] = m14;
            Data[4] = m21;
            Data[5] = m22;
            Data[6] = m23;
            Data[7] = m24;
            Data[8] = m31;
            Data[9] = m32;
            Data[10] = m33;
            Data[11] = m34;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transform the given vector 'v' by this matriX.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3 Transform(Vector3 v)
        {
            return this * v;
        }

        /// <summary>
        /// Sets this matriX to the inverse of itself if the 4th row were [0, 0, 0, 1].
        /// </summary>
        public void SetInverse()
        {
            if (!Invertible) return;

            var invd = 1.0 / Determinant;

            Data[0] = (-Data[9] * Data[6] + Data[5] * Data[10]) * invd;
            Data[4] = (Data[8] * Data[6] - Data[4] * Data[10]) * invd;
            Data[8] = (-Data[8] * Data[5] + Data[4] * Data[9] * Data[15]) * invd;
            Data[1] = (Data[9] * Data[2] - Data[1] * Data[10]) * invd;
            Data[5] = (-Data[8] * Data[2] + Data[0] * Data[10]) * invd;
            Data[9] = (Data[8] * Data[1] - Data[0] * Data[9] * Data[15]) * invd;
            Data[2] = (-Data[5] * Data[2] + Data[1] * Data[6] * Data[15]) * invd;

            Data[6] = (+Data[4] * Data[2] - Data[0] * Data[6] * Data[15]) * invd;
            Data[10] = (-Data[4] * Data[1] + Data[0] * Data[5] * Data[15]) * invd;

            Data[3] = (Data[9] * Data[6] * Data[3] - Data[5] * Data[10] * Data[3]
            - Data[9] * Data[2] * Data[7] + Data[1] * Data[10] * Data[7]
            + Data[5] * Data[2] * Data[11] - Data[1] * Data[6] * Data[11]) * invd;

            Data[7] = (-Data[8] * Data[6] * Data[3]+ Data[4] * Data[10] * Data[3]
            + Data[8] * Data[2] * Data[7]- Data[0] * Data[10] * Data[7]
            - Data[4] * Data[2] * Data[11]+ Data[0] * Data[6] * Data[11]) * invd;

            Data[11] = (Data[8] * Data[5] * Data[3]- Data[4] * Data[9] * Data[3]
            - Data[8] * Data[1] * Data[7]+ Data[0] * Data[9] * Data[7]
            + Data[4] * Data[1] * Data[11]- Data[0] * Data[5] * Data[11]) * invd;
        }

        /// <summary>
        /// Returns a new matriX which is the inverse of this matriX.
        /// </summary>
        /// <returns></returns>
        public Matrix4 Inverse()
        {
            Matrix4 result = this;
            result.SetInverse();
            return result;
        }

        public void SetOrientation(Quaternion q, Vector3 pos)
        {
            Data[0] = 1 - (2 * q.J * q.J + 2 * q.K * q.K);
            Data[1] = 2 * q.I * q.J + 2 * q.K * q.R;
            Data[2] = 2 * q.I * q.K - 2 * q.J * q.R;
            Data[3] = pos.X;
            Data[4] = 2 * q.I * q.J - 2 * q.K * q.R;
            Data[5] = 1 - (2 * q.I * q.I + 2 * q.K * q.K);
            Data[6] = 2 * q.J * q.K + 2 * q.I * q.R;
            Data[7] = pos.Y;
            Data[8] = 2 * q.I * q.K + 2 * q.J * q.R;
            Data[9] = 2 * q.J * q.K - 2 * q.I * q.R;
            Data[10] = 1 - (2 * q.I * q.I + 2 * q.J * q.J);
            Data[11] = pos.Z;
        }

        /// <summary>
        /// Transform the given vector by the transformational inverse of this matrix.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 TransformInverse(Vector3 vector)
        {
            Vector3 tmp = vector;
            tmp.X -= Data[3];
            tmp.Y -= Data[7];
            tmp.Z -= Data[11];
            return new Vector3(
            tmp.X * Data[0] +
            tmp.Y * Data[4] +
            tmp.Z * Data[8],
            tmp.X * Data[1] +
            tmp.Y * Data[5] +
            tmp.Z * Data[9],
            tmp.X * Data[2] +
            tmp.Y * Data[6] +
            tmp.Z * Data[10]
            );
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the determinant of this matriX as if it were a 4X4 matriX
        /// with [0, 0, 0 ,1] as the last row.
        /// </summary>
        /// <returns></returns>
        public double CalculateDeterminant()
        {
            return Data[8] * Data[5] * Data[2] +
            Data[4] * Data[9] * Data[2] +
            Data[8] * Data[1] * Data[6] -
            Data[0] * Data[9] * Data[6] -
            Data[4] * Data[1] * Data[10] +
            Data[0] * Data[5] * Data[10];
        }

        /// <summary>
        /// Transform the given direction vector by this matrix.
        /// </summary>
        /// <param name="vector"></param>
        public Vector3 TransformDirection(Vector3 vector)
        {
            return new Vector3(
            vector.X * Data[0] +
            vector.Y * Data[1] +
            vector.Z * Data[2],
            vector.X * Data[4] +
            vector.Y * Data[5] +
            vector.Z * Data[6],
            vector.X * Data[8] +
            vector.Y * Data[9] +
            vector.Z * Data[10]
);
        }

        /// <summary>
        /// Transform the given direction vector by the
        /// transformational inverse of this matrix.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 TransformInverseDirection(Vector3 vector)
        {
            return new Vector3(
            vector.X * Data[0] +
            vector.Y * Data[4] +
            vector.Z * Data[8],
            vector.X * Data[1] +
            vector.Y * Data[5] +
            vector.Z * Data[9],
            vector.X * Data[2] +
            vector.Y * Data[6] +
            vector.Z * Data[10]
);
        }

        #endregion

        #region Operator Overloads

        public static Vector3 operator *(Matrix4 m, Vector3 v)
        {
            return new Vector3(v.X * m.Data[0] + v.Y * m.Data[1] + v.Z * m.Data[2] + m.Data[3],
                v.X * m.Data[4] + v.Y * m.Data[5] + v.Z * m.Data[6] + m.Data[7],
                v.X * m.Data[8] + v.Y * m.Data[9] + v.Z * m.Data[10] + m.Data[11]);
        }

        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            Matrix4 result = new Matrix4();
            result.Data[0] = m2.Data[0] * m1.Data[0] + m2.Data[4] * m1.Data[1] +
            m2.Data[8] * m1.Data[2];

            result.Data[4] = m2.Data[0] * m1.Data[4] + m2.Data[4] * m1.Data[5] +
            m2.Data[8] * m1.Data[6];

            result.Data[8] = m2.Data[0] * m1.Data[8] + m2.Data[4] * m1.Data[9] +
            m2.Data[8] * m1.Data[10];

            result.Data[1] = m2.Data[1] * m1.Data[0] + m2.Data[5] * m1.Data[1] +
            m2.Data[9] * m1.Data[2];

            result.Data[5] = m2.Data[1] * m1.Data[4] + m2.Data[5] * m1.Data[5] +
            m2.Data[9] * m1.Data[6];

            result.Data[9] = m2.Data[1] * m1.Data[8] + m2.Data[5] * m1.Data[9] +
            m2.Data[9] * m1.Data[10];

            result.Data[2] = m2.Data[2] * m1.Data[0] + m2.Data[6] * m1.Data[1] +
            m2.Data[10] * m1.Data[2];

            result.Data[6] = m2.Data[2] * m1.Data[4] + m2.Data[6] * m1.Data[5] +
            m2.Data[10] * m1.Data[6];

            result.Data[10] = m2.Data[2] * m1.Data[8] + m2.Data[6] * m1.Data[9] +
            m2.Data[10] * m1.Data[10];

            result.Data[3] = m2.Data[3] * m1.Data[0] + m2.Data[7] * m1.Data[1] +
            m2.Data[11] * m1.Data[2] + m1.Data[3];

            result.Data[7] = m2.Data[3] * m1.Data[4] + m2.Data[7] * m1.Data[5] +
            m2.Data[11] * m1.Data[6] + m1.Data[7];

            result.Data[11] = m2.Data[3] * m1.Data[8] + m2.Data[7] * m1.Data[9] +
            m2.Data[11] * m1.Data[10] + m1.Data[11];

            return result;
        }

        #endregion
    }
}
