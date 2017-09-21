namespace XTable
{

    /// <summary>
    /// 所有扩展表格的方法都写在这里
    /// </summary>
    public static class ExtTable
    {

        public static EquipSuit.RowData GetByProfID(this EquipSuit input, int proID)
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

        public static SceneList.RowData GetItemID(this SceneList input, uint id)
        {
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].SceneID == id)
                {
                    return input.Table[i];
                }
            }
            return null;
        }

        public static XNpcList.RowData GetItemID(this XNpcList input, int id)
        {
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].NPCID == id)
                {
                    return input.Table[i];
                }
            }
            return null;
        }


        public static XEntityStatistics.RowData GetByID(this XEntityStatistics input, int id)
        {
            for (int i = 0, max = input.Table.Length; i < max; i++)
            {
                if (input.Table[i].id == id)
                {
                    return input.Table[i];
                }
            }
            return null;
        }


        public static QteStatusList.RowData GetByID(this QteStatusList input,int qte)
        {
            for(int i =0,max=input.Table.Length;i<max;i++)
            {
                if(input.Table[i].Value == qte)
                {
                    return input.Table[i];
                }
            }
            return null;
        }
    }

}