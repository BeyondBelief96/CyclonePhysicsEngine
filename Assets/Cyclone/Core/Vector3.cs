using Assets.Cyclone.Core;
using System;
using Real = System.Double;

namespace Cyclone.Core
{
    /// <summary>
    /// Represents a vector in three dimensions.
    /// </summary>

    [Serializable]
    public struct Vector3
    {
        #region Static Initializers

        public static Vector3 ZeroVector => new Vector3(0, 0, 0);

        #endregion

        #region Properties

        /// <summary>
        /// Holds value along the X axis.
        /// </summary>
        public Real X { get; set; }

        /// <summary>
        /// Holds value along the Y axis.
        /// </summary>
        public Real Y { get; set; }

        /// <summary>
        /// Holds value along the Z axis.
        /// </summary>
        public Real Z { get; set; }

        /// <summary>
        /// Gets the squared magnitude of this vector.
        /// </summary>
        public Real SquareMagnitude
        {
            get => X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Returns the normalized version of this vector.
        /// </summary>
        public Vector3 Normalized
        {
            get
            {
                Real inverseMagnitude = Mathematics.SafeInvSqrt(1.0, SquareMagnitude);
                return new Vector3(X * inverseMagnitude, Y * inverseMagnitude, Z * inverseMagnitude);
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Explicit constructor creates a vector with the given
        /// components.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Explicit constructor that creates a two dimensional vector.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector3(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Flips all components of the vector with a negative sign.
        /// </summary>
        public void Invert()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Gets the magnitude of this vector.
        /// </summary>
        /// <returns></returns>
        public Real Magnitude()
        {
            return Mathematics.SafeSqrt(SquareMagnitude);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize()
        {
            Real invLength = Mathematics.SafeInvSqrt(1.0, SquareMagnitude);
            X *= invLength;
            Y *= invLength;
            Z *= invLength;
        }

        /// <summary>
        /// Calculates and returns a component-wise product of this
        /// vector with the given vector "vector".
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 ComponentProduct(Vector3 vector)
        {
            return new Vector3(X * vector.X, Y * vector.Y, Z * vector.Z);
        }

        /// <summary>
        /// Preforms a component wise product with the given vector
        /// "vector" and sets this vector to the result.
        /// </summary>
        /// <param name="vector"></param>
        public void ComponentProductUpdate(Vector3 vector)
        {
            X*= vector.X;
            Y*= vector.Y;
            Z*= vector.Z;
        }

        public static double Distance(Vector3 a, Vector3 b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculates and returns the dot (scalar) products between
        /// this vector and the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Real DotProduct(Vector3 vector)
        {
            return X * vector.X + Y * vector.Y + Z * vector.Z;
        }

        /// <summary>
        /// Calculates and returns the cross (vector) product
        /// of this vector and the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 CrossProduct(Vector3 vector)
        {
            return new Vector3(Y * vector.Z - Z * vector.Y,
                Z * vector.X - X * vector.Z,
                X * vector.Y - Y * vector.X);
        }

        /// <summary>
        /// Calculates the cross product between the two given
        /// vectors and returns the resulting vector.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);
        }

        /// <summary>
        /// Creates a set of orthonormal vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public static void MakeOrthonormalBasis(ref Vector3 a, ref Vector3 b,  out Vector3 c)
        {
            //Ensure a is unit length
            a.Normalize();

            //Creates a vector perpendicular to a and b.
            c = a.CrossProduct(b);

            //Ensure a and b weren't parallel to begin with.
            if (c.SquareMagnitude == 0)
            {
                throw new ArgumentException("a and b are parallel");
            }

            //Ensure our new vector is of unit length
            c.Normalize();

            //Use both a and b (which are unit length) to find b that is perpendicular to both.
            b = a.CrossProduct(c);
        }

        public Vector3 LocalToWorld(Vector3 local, Matrix4 transform)
        {
            return transform.Transform(local);
        }

        public Vector3 WorldToLocal(Vector3 world, Matrix4 transform)
        {
            return transform.TransformInverse(world);
        }

        public Vector3 LocalToWorldDirection(Vector3 local, Matrix4 transform)
        {
            return transform.TransformDirection(local);
        }

        public Vector3 WorldToLocalDirection(Vector3 world, Matrix4 transform)
        {
            return transform.TransformDirection(world);
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Multiply vector v1 by a scalar value d.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 v1, Real d)
        {
            return new Vector3(v1.X * d, v1.Y * d, v1.Z * d);
        }

        /// <summary>
        /// Multiply vector v1 by a scalar value d.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Vector3 operator *(Real d, Vector3 v1)
        {
            return new Vector3(v1.X * d, v1.Y * d, v1.Z * d);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        /// <summary>
        /// Substracts two vectors.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        /// <summary>
        /// Preforms a dot-product (scalar product).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Real operator *(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        /// <summary>
        /// Preforms a cross (vector) product.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 operator %(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);
        }

        /// <summary>
        /// Returns a string representation of the vector.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z:{Z}";
        }

        #endregion
    }
}
