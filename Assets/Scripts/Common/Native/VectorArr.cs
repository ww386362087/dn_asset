using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VectorArr
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    float[] arr;

    Vector3 GetVector()
    {
        if (arr.Length == 3)
            return new Vector3(arr[0], arr[1], arr[2]);
        else
            return Vector3.zero;
    }
}

