using System;

namespace gifer {
    public class AnimationHelper {
        public static float GetEnlargementValue(float ratio) => Math.Abs(ratio > 0 ? ratio : 1 / ratio);
    }
}
