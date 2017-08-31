using System.Collections.Generic;
using UnityEngine;

namespace XEditor
{
    public class MapPresent
    {
        private MapGenerator _generator = null;

        private Material _grid_material;
        private GameObject[] _grid_go;

        public MapPresent(MapGenerator generator)
        {
            _generator = generator;
        }

        public void Reset()
        {
            if (_grid_go != null)
            {
                for (int i = 0; i < _grid_go.Length; i++)
                {
                    GameObject.DestroyImmediate(_grid_go[i]);
                    _grid_go[i] = null;
                }
                _grid_go = null;
            }
        }

        public void DrawGrids()
        {
            _grid_go = new GameObject[_generator._data_row * _generator._data_col];
            for (int z = 0; z < _generator._data_row; z++)
            {
                for (int x = 0; x < _generator._data_col; x++)
                {
                    BuildGridMesh(z, x);
                    DrawGridMesh(z, x);
                }
            }
        }

        private void BuildGridMesh(int z, int x)
        {
            _grid_go[z * _generator._data_col + x] = new GameObject();
            GameObject go = _grid_go[z * _generator._data_col + x];
            go.name = z + "_" + x;

            if (_grid_material == null)
            {
                _grid_material = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Editor/MapEditor/Res/GridMat.mat", typeof(Material)) as Material;
            }
            go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterials = new Material[] { _grid_material };
        }

        private void DrawGridMesh(int gz, int gx)
        {
            GameObject go = _grid_go[gz * _generator._data_col + gx];
            MeshFilter mf = go.GetComponent<MeshFilter>();
            Mesh mMesh = new Mesh();
            mMesh.hideFlags = HideFlags.DontSave;
            mMesh.name = go.name;

            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color32> cols = new List<Color32>();
            List<int> indecies = new List<int>();

            int index = 0;
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (z + 32 * gz < _generator._row && x + 32 * gx < _generator._col)
                    {
                        int debug = _generator._col * 32 * gz + _generator._col * z + gx * 32 + x;
                        float height = _generator.GetHeight(debug);

                        float pos_x = _generator._min.x + gx * 32 * _generator._grid_size + x * _generator._grid_size;
                        float pos_y = _generator._min.y + height + 0.02f;
                        float pos_z = _generator._min.z + gz * 32 * _generator._grid_size + z * _generator._grid_size;
                        verts.Add(new Vector3(pos_x, pos_y, pos_z));
                        verts.Add(new Vector3(pos_x, pos_y, pos_z + _generator._grid_size * 0.95f));
                        verts.Add(new Vector3(pos_x + _generator._grid_size * 0.95f, pos_y, pos_z + _generator._grid_size * 0.95f));
                        verts.Add(new Vector3(pos_x + _generator._grid_size * 0.95f, pos_y, pos_z));

                        indecies.Add(index);
                        indecies.Add(index + 1);
                        indecies.Add(index + 2);

                        indecies.Add(index + 2);
                        indecies.Add(index + 3);
                        indecies.Add(index);

                        index += 4;

                        uvs.Add(new Vector2(0f, 0f));
                        uvs.Add(new Vector2(0f, 1f));
                        uvs.Add(new Vector2(1f, 1f));
                        uvs.Add(new Vector2(1f, 0f));

                        Color color = height < 0 ? Color.red : Color.green;
                        cols.Add(color);
                        cols.Add(color);
                        cols.Add(color);
                        cols.Add(color);
                    }
                }
            }

            mMesh.vertices = verts.ToArray();
            mMesh.triangles = indecies.ToArray();
            mMesh.uv = uvs.ToArray();
            mMesh.colors32 = cols.ToArray();

            mf.mesh = mMesh;
        }
    }
}