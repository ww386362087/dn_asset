using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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
                    string path = Application.StartupPath;
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

        public static string unity_marshal_path
        {
            get { return unity_proj_path + @"Assets\Scripts\Marshal\"; }
        }

        public static string unity_bytes_path
        {
            get { return unity_proj_path + @"Assets\StreamingAssets\Table\"; }
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
                ClearContent();
                AppendContent("正在生成表格中.");
                string dest = unity_bytes_path;
                GenerateBytes.sington.GenerateAllBytes(this);
                string str = "生成表格完毕!";
                AppendContent(str);
                // MessageBox.Show(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "  \n" + ex.StackTrace, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void makeCodeBtn_Click(object sender, EventArgs e)
        {
            ClearContent();
            AppendContent("正在生成代码中.");
            GenerateCode.sington.GenerateAll(this);
            string str = "生成代码完毕!";
            AppendContent(str);
            //MessageBox.Show(str);
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearContent();
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

        private void build_Click(object sender, EventArgs e)
        {
            ClearContent();
            AppendContent("开始编译表格代码.");
            CompileCode.Build(this);


        }
        
        private void AppendContent(string str)
        {
            _content = _content.Append(str);
            _content = _content.Append("\n");
            contentLbl.Text = _content.ToString();
        }


        private void ClearContent()
        {
            _content.Length = 0;
            contentLbl.Text = "";
        }

        private void marshal_Click(object sender, EventArgs e)
        {
            GenerateMarshalCode.sington.GenerateAll(this);
        }

        private void cpp_Click(object sender, EventArgs e)
        {
            GenerateCppCode.sington.GenerateAll(this);
        }
    }
}
