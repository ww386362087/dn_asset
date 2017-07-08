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
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].ProfID == proID)
                    return input.Table[i];
            }
            return null;
        }


        public static DefaultEquip.RowData GetByProfID(this DefaultEquip input, int proID)
        {
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].ProfID == proID)
                    return input.Table[i];
            }
            return null;
        }


        public static FashionList.RowData GetByItemID(this FashionList input, int fashioid)
        {
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].ItemID == fashioid)
                    return input.Table[i];
            }
            return null;
        }

        public static XEntityPresentation.RowData GetItemID(this XEntityPresentation input, uint id)
        {
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].ID == id)
                {
                    return input.Table[i];
                }
            }
            return null;
        }


    }

}