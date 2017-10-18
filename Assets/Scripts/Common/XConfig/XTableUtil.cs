namespace XTable
{

    /// <summary>
    /// 所有扩展表格的方法都写在这里
    /// </summary>
    public static class ExtTable
    {
        

        public static XEntityPresentation.RowData GetItemID(this XEntityPresentation input, uint id)
        {
            return input.GetByUID((int)id);
        }
        

        public static XEntityStatistics.RowData GetByID(this XEntityStatistics input, int id)
        {
            return input.GetByUID(id);
        }
        
    }

}