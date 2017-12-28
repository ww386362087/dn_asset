using System;
using System.IO;
using System.Windows.Forms;

namespace XForm
{
    class GenerateBytes
    {
        private static GenerateBytes _s = null;

        public static GenerateBytes sington
        {
            get
            {
                if (_s == null)
                    _s = new GenerateBytes();
                return _s;
            }
        }


        public void GenerateAllBytes(XCForm f)
        {
            DirectoryInfo dir = new DirectoryInfo(XCForm.unity_table_path);
            FileInfo[] files = dir.GetFiles("*.csv");
            for (int i = 0, max = files.Length; i < max; i++)
            {
                FileInfo file = files[i];
                int indx = file.Name.LastIndexOf('.');
                string destname = XCForm.unity_bytes_path + file.Name.Substring(0, indx) + ".bytes";
                WriteBytes(file, destname, System.Text.Encoding.UTF8);
                f.PCB(file.FullName);
            }
        }

        public void GenerateXFormBytes(XCForm f)
        {
            DirectoryInfo dir = new DirectoryInfo(XCForm.unity_table_path);
            FileInfo[] files = dir.GetFiles("*.csv");
            for (int i = 0, max = files.Length; i < max; i++)
            {
                FileInfo file = files[i];
                int indx = file.Name.LastIndexOf('.');
                string destname = XCForm.xform_bytes_path + file.Name.Substring(0, indx) + ".bytes";
                WriteBytes(file, destname, System.Text.Encoding.Default);
                f.PCB(file.FullName);
            }
        }


        public void WriteByte(string path)
        {
            FileInfo file = new FileInfo(path);
            int indx = file.Name.LastIndexOf('.');
            string destname = XCForm.unity_bytes_path + file.Name.Substring(0, indx) + ".bytes";
            WriteBytes(file, destname,System.Text.Encoding.UTF8);
        }


        private void WriteBytes(FileInfo src, string dest, System.Text.Encoding coding)
        {
            CSVTable table = CSVUtil.sington.UtilCsv(src);
            try
            {
                using (FileStream fs = new FileStream(dest, FileMode.Create))
                {
                    BinaryWriter write = new BinaryWriter(fs, coding);
                    //先预留一个long记录文件大小
                    write.Seek(8, SeekOrigin.Begin);
                    write.Write(table.rowCnt);
                    for (int i = 0, max = table.sortlist.Count; i < max; i++)
                    {
                        for (int j = 0, len = table.sortlist[i].row.Length; j < len; j++)
                        {
                            CSVStruct st = table.sortlist[i].row[j];
                            st.parse.Write(write, st.content);
                            st.parse.title = st.title;
                        }
                    }

                    fs.Seek(0, SeekOrigin.Begin);
                    write.Write(fs.Length);
                    fs.Seek(0, SeekOrigin.End);

                    write.Seek(0, SeekOrigin.Begin);
                    write.Write(fs.Length);
                    write.Flush();
                    write.Close();
                    XDebug.Log(dest);
                }
            }
            catch (Exception ex)
            {
                XDebug.LogError("解析表格" + table.name + "失败," + ex.Message + "  \n" + ex.StackTrace);
                MessageBox.Show("解析表格" + table.name + "失败," + ex.Message + "  \n" + ex.StackTrace, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

}

