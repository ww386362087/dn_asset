using UnityEngine;
using System.Collections;

public class XMeshMultiTexData : MonoBehaviour
{
    [SerializeField]
    public Mesh _mesh;
    [SerializeField]
    public Texture2D _tex0;
    [SerializeField]
    public Texture2D _tex1;

    public Mesh mesh { get { return _mesh; } }
    public Texture2D tex0 { get { return _tex0; } }
    public Texture2D tex1 { get { return _tex1; } }

}
