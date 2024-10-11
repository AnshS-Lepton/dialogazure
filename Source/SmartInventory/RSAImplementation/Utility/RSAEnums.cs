using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAImplementation.Utility
{
    public enum AlphaEnum
    {
        B = 2,
        E = 1,
        L = 8,
        N = 6,
        Q = 9,
        R = 7,
        S = 4,
        V = 5,
        X = 0,
        Z = 3
    }
    public enum DtEnum
    {
        D = 0,
        M = 1,
        Y = 2
    }
    public enum SpecialCharacterSet
    {
        SpChar1 = 1, // "!@#"   //indicated by this will store month    --start section
        SpChar2 = 2, // "#@!"   //indicated by this will store month    --end section
        SpChar3 = 3, // "$%^"   //indicated by this will store year     --start section
        SpChar4 = 4, // "^%$"   //indicated by this will store year     --end section
        SpChar5 = 5, // "&*("   //indicated by this will store day      --start section
        SpChar6 = 6  // "(*&"   //indicated by this will store day      --end section
    }
}
