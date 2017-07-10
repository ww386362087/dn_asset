using System;
using System.Collections.Generic;
using XTable;

namespace XTable
{

    /// <summary>
    /// 所有扩展表格的方法都写在这里
    /// </summary>
    public static class ExtTable
    {

        public static EquipSuit.RowData GetByProfID(this EquipSuit input,int proID)
        {
            for (int i = 0, max = EquipSuit.Table.Length; i < max; i++)
            {
                if (EquipSuit.Table[i].ProfID == proID)
                    return EquipSuit.Table[i];
            }
            return null;
        }


        public static DefaultEquip.RowData GetByProfID(this DefaultEquip input, int proID)
        {
            for (int i = 0, max = DefaultEquip.Table.Length; i < max; i++)
            {
                if (DefaultEquip.Table[i].ProfID == proID)
                    return DefaultEquip.Table[i];
            }
            return null;
        }


        public static FashionList.RowData GetByItemID(this FashionList input, int fashioid)
        {
            for (int i = 0, max = FashionList.Table.Length; i < max; i++)
            {
                if (FashionList.Table[i].ItemID == fashioid)
                    return FashionList.Table[i];
            }
            return null;
        }

        public static XEntityPresentation.RowData GetItemID(this XEntityPresentation input, uint id)
        {
            for (int i = 0, max = XEntityPresentation.Table.Length; i < max; i++)
            {
                if (XEntityPresentation.Table[i].ID == id)
                {
                    return XEntityPresentation.Table[i];
                }
            }
            return null;
        }


    }

}