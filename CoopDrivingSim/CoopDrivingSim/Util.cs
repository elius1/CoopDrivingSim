using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoopDrivingSim
{
    public static class Util
    {
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
