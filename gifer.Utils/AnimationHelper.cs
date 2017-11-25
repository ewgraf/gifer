﻿using System;

namespace gifer.Utils {
    public class AnimationHelper {
        public static double GetEnlargementValue(double ratio) => Math.Abs(ratio > 0 ? ratio : 1 / ratio);
    }
}