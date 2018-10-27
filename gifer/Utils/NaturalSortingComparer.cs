using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace gifer {
    // https://stackoverflow.com/questions/39342479/sorting-numbers-in-stringsarray
    public class NaturalSortingComparer : IComparer<string> {        
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y) {
            return StrCmpLogicalW(x, y);
        }
    }
}
