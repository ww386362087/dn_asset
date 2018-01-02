using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct VectorArr
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    float[] arr;

    public Vector3 TVector()
    {
        if (arr.Length == 3)
            return new Vector3(arr[0], arr[1], arr[2]);
        else
            return Vector3.zero;
    }
}

