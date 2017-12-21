using System;
using System.Runtime.InteropServices;

namespace gifer.Utils {
    public static class NativeMethods {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
    }
}
