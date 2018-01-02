using UnityEngine;
using XTable;

public interface IEquip
{
    SkinnedMeshRenderer skin { get; set; }

    MaterialPropertyBlock mpb { get; set; }

    DefaultEquip.RowData data { get; }

    GameObject EntityObject { get; }
}
