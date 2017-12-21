using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace gifer.Utils {
    public class NaturalSortingComparer : IComparer<string> {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y) {
            return StrCmpLogicalW(x, y);
        }
    }
}
