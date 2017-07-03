using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text;


namespace XForm
{

    public partial class XCForm : Form
    {
        private static string _proj_path = string.Empty;

        private StringBuilder _content = new StringBuilder();

        public delegate void ProgressCallback(string str);

        public ProgressCallback PCB;

        public static string unity_proj_path
        {
            get
            {
                if (string.IsNullOrEmpty(_proj_path))
                {
                    string path = System.Windows.Forms.Application.StartupPath;
                    int indx = path.IndexOf("tools_proj");
                    path = path.Substring(0, indx);
                    _proj_path = path;
                }
                return _proj_path;
            }
        }


        public static string unity_table_path
        {
            get { return unity_proj_path + @"Assets\Table\"; }
        }

        public static string unity_bytes_path
        {
            get { return unity_proj_path + @"Assets\Resources\Table\"; }
        }


        public XCForm()
        {
            InitializeComponent();

            pathLbl.Text = unity_table_path;
            contentLbl.Text = "";
            PCB = new ProgressCallback(AppendContent);
        }

        private void bytesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                _content.Clear();
                AppendContent("正在生成表格中.");
                string dest = unity_bytes_path;
                GenerateByte.sington.GenerateAllBytes(this);
                string str = "生成表格完毕!";
                AppendContent(str);
                MessageBox.Show(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "  \n" + ex.StackTrace, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void makeCodeBtn_Click(object sender, EventArgs e)
        {
            _content.Clear();
            AppendContent("正在生成代码中.");
            GenerateCode.sington.GenerateAll(this);
            string str = "生成代码完毕!";
            AppendContent(str);
            MessageBox.Show(str);
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            _content.Clear();
            AppendContent("正在清除表格和代码中.");
            DirectoryInfo dir = new DirectoryInfo(unity_bytes_path);
            FileInfo[] files = dir.GetFiles();
            for (int i = 0, max = files.Length; i < max; i++)
            {
                AppendContent(files[i].FullName);
                File.Delete(files[i].FullName);
            }
            GenerateCode.sington.CleanAll(this);
            string str = "清除完毕!";
            AppendContent(str);
            MessageBox.Show(str);
        }


        private void AppendContent(string str)
        {
            _content = _content.Append(str);
            _content = _content.Append("\n");
            contentLbl.Text = _content.ToString();
        }

    }

}
