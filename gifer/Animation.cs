using System;

namespace gifer
{
    public class Animation
    {
        public static double GetEnlargementValue(double ratio) => Math.Abs(ratio > 0 ? ratio : 1 / ratio);
    }
}
