using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone.Core
{
    public class Mathematics
    {
        #region Constants

        public const double EPS = 1e-18;

        public const double PI = Math.PI;

        public const double SQRT2 = 1.414213562373095;

        public const double Rad2Deg = 180.0 / PI;

        public const double Deg2Rad = PI / 180.0;

        #endregion

        #region Methods

        /// <summary>
        /// Returns the square root of a number if it is greater than or
        /// equal to zero.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double SafeSqrt(double v)
        {
            if (v <= 0.0) return 0.0;
            return Math.Sqrt(v);
        }
        /// <summary>
        /// Divides the numerator "n" by the square root of the denominator "d"
        /// and returns the result. 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static double SafeInvSqrt(double n, double d, double eps = EPS)
        {
            if (d <= 0.0) return 0.0;
            d = Math.Sqrt(d);
            if (d < eps) return 0.0;
            return n / d;
        }

        #endregion
    }
}
