using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{
    /// <summary>
    /// A collection of utility methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Checks if the length of the specified vector exceeds the specified maximum and
        /// if so makes the vector that max length.
        /// </summary>
        /// <param name="value">The vector that should be checked.</param>
        /// <param name="max">The maximum length of the vector.</param>
        /// <returns>The vector with new length.</returns>
        public static Vector2 Truncate(Vector2 value, float max)
        {
            if (value.Length() > max)
            {
                value = Vector2.Normalize(value) * max;
            }

            return value;
        }
    }
}
