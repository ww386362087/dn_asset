using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator
{
    private MapPresent _present;

    public float _grid_size = 0.2f;
    public float _inaccuracy = 0.01f;

    public Vector3 _min, _max;
    public int _row, _col;
    public int _data_row, _data_col;

    private List<float> _raw_data = new List<float>();
    private List<int> _data_idx = new List<int>();
    private List<short> _data_value = new List<short>();

    private string _filePath;

    public MapGenerator()
    {
        _present = new MapPresent(this);
    }

    public void Generate(string path)
    {
        if (_grid_size <= 0.0f || _grid_size > 1) return;

        Reset();
        _filePath = path;
        GameObject dynamic = GameObject.Find("DynamicScene");
        if (dynamic != null) dynamic.SetActive(false);
        GetMapBound();
        GenerateMapdata();
        RefineData();
        if (dynamic != null) dynamic.SetActive(true);
        _present.DrawGrids();
        SaveToFile();
    }

    public void Reset()
    {
        _row = 0;
        _col = 0;

        _data_row = 0;
        _data_col = 0;

        _raw_data.Clear();
        _data_idx.Clear();
        _data_value.Clear();

        _raw_data.Capacity = 0;
        _data_idx.Capacity = 0;
        _data_value.Capacity = 0;

        _present.Reset();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void LoadFromFile(string path)
    {
        Reset();
        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

        _row = reader.ReadInt32();
        _col = reader.ReadInt32();

        _min.x = reader.ReadSingle();
        _min.y = 0;
        _min.z = reader.ReadSingle();

        _max.x = reader.ReadSingle();
        _max.y = 0;
        _max.z = reader.ReadSingle();

        _grid_size = reader.ReadSingle();

        _data_row = (_row + 31) / 32;
        _data_col = (_col + 31) / 32;

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            _data_idx.Add(reader.ReadInt32());
        }

        for (int i = 0; i < count; i++)
        {
            short s = reader.ReadInt16();
            if (s < 0)
            {
                _data_value.Add(-100);

                if (i + 1 < count)
                {
                    if (s == short.MinValue)
                        s = 0;
                    else
                        s = (short)-s;

                    _data_value.Add(s);
                    i++;
                }
            }
            else
                _data_value.Add(s);
        }
        reader.Close();
        _present.DrawGrids();
    }

    public float GetHeight(int key)
    {
        int len = _data_idx.Count;

        int lower = 0;
        int upper = len - 1;

        while (lower <= upper)
        {
            int mid = (lower + upper) >> 1;
            int idx = _data_idx[mid];

            if (key == idx)
            {
                return _data_value[mid] / 100.0f;
            }
            else if (key < idx)
            {
                upper = mid - 1;
            }
            else if (key > idx)
            {
                lower = mid + 1;
            }
        }
        return _data_value[upper] / 100.0f;
    }

    private void GenerateMapdata()
    {
        for (int _z = 0; _z < _row; _z++)
        {
            for (int _x = 0; _x < _col; _x++)
            {
                Vector3 grid_center = new Vector3(_min.x + _x * _grid_size + _grid_size / 2, 100, _min.z + _z * _grid_size + _grid_size / 2);
                float gridheight = Terrain.activeTerrain.SampleHeight(grid_center);

                int layer_mask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("AirWall"));

                RaycastHit hitInfo;

                Vector3[] lines = new Vector3[4];
                lines[0] = new Vector3(_min.x + _x * _grid_size, 100, _min.z + _z * _grid_size);
                lines[1] = new Vector3(_min.x + _x * _grid_size + _grid_size, 100, _min.z + _z * _grid_size);
                lines[2] = new Vector3(_min.x + _x * _grid_size, 100, _min.z + _z * _grid_size + _grid_size);
                lines[3] = new Vector3(_min.x + _x * _grid_size + _grid_size, 100, _min.z + _z * _grid_size + _grid_size);

                float height = 0.0f;
                float min_height = 0;

                int LineColliderCount = 0;
                int LineColliderAirWall = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (Physics.SphereCast(lines[i], 0.005f, Vector3.down, out hitInfo, 101, layer_mask))
                    {
                        LineColliderCount++;

                        if (hitInfo.collider.gameObject.tag == "AirWall" || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("AirWall"))
                        {
                            LineColliderAirWall++;
                        }
                        else
                        {
                            if (height == 0 || hitInfo.point.y < min_height) min_height = hitInfo.point.y;
                            height += hitInfo.point.y;
                        }
                    }
                }

                //average
                height = height > 0 ? height / (LineColliderCount - LineColliderAirWall) : 0;
                if (height == float.MaxValue) throw new Exception();

                float temp = 0; bool collide = false;
                // -1: not walkable
                // -2: not walkable for enemy not for role
                if (LineColliderCount == 0)
                {
                    temp = gridheight;
                }
                else if (LineColliderCount == 4)
                {
                    if (LineColliderAirWall == 4)
                    {
                        collide = true;
                    }
                    else
                    {
                        temp = height;
                    }
                }
                else
                {
                    temp = min_height;
                }

                if (collide)
                    temp = -1;
                else if (temp < 0)
                {
                    temp = gridheight;
                }

                _raw_data.Add((temp >= 0 && temp < gridheight) ? gridheight : temp);
            }
        }
    }

    private void SaveToFile()
    {
        BinaryWriter writer = new BinaryWriter(File.Open(_filePath, FileMode.Create));
        writer.Write(_row);
        writer.Write(_col);
        writer.Write(_min.x);
        writer.Write(_min.z);
        writer.Write(_max.x);
        writer.Write(_max.z);
        writer.Write(_grid_size);
        writer.Write(_data_idx.Count);

        for (int i = 0; i < _data_idx.Count; i++)
        {
            writer.Write(_data_idx[i]);
        }

        for (int i = 0; i < _data_value.Count; i++)
        {
            short value = _data_value[i];

            if (value < 0)
            {
                if (i + 1 < _data_value.Count)
                {
                    value = _data_value[i + 1];

                    if (value == 0)
                        value = short.MinValue;
                    else
                        value = (short)-value;
                    i++;
                }
            }

            writer.Write(value);
        }

        writer.Close();
    }

    private void GetMapBound()
    {
        _min = Vector3.zero;
        _max = Terrain.activeTerrain.terrainData.size;

        _row = (int)((_max.z - _min.z) / _grid_size) + 1;
        _col = (int)((_max.x - _min.x) / _grid_size) + 1;

        _data_row = (_row + 31) / 32;
        _data_col = (_col + 31) / 32;

        _raw_data.Clear();
    }

    private void RefineData()
    {
        _data_idx.Clear();
        _data_value.Clear();

        float last = _raw_data[0];
        AddCompressedData(0, last);

        for (int i = 1; i < _raw_data.Count; i++)
        {
            if (Mathf.Abs(last - _raw_data[i]) > _inaccuracy)
            {
                last = _raw_data[i];
                AddCompressedData(i, last);
            }
        }
    }

    private void AddCompressedData(int idx, float value)
    {
        _data_idx.Add(idx);
        _data_value.Add((short)(value * 100));
    }
}



