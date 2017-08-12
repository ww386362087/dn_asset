using UnityEngine;
using System.Collections;

public class XMeshTexData : MonoBehaviour
{
    [SerializeField]
    public Mesh _mesh;
    [SerializeField]
    public Texture2D _tex;
    [SerializeField]
    public string _offset;

    public Mesh mesh { get { return _mesh; } }
    public Texture2D tex { get { return _tex; } }
    public string offset { get { return _offset; } }
}
