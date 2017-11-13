using System;
using System.IO;
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
            BuildCode.Build(this);


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


        private string path = @"D:\projects\dn_asset\tools_proj\XCPP\XCPP\a.txt";
        private void write_Click(object sender, EventArgs e)
        {
            ClearContent();
            AppendContent("Start write cpp file");
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(10);
            writer.Write(false);
            string s = "";
            for (int i = 0; i < 31; i++)
            {
                s += "abc";
            }
            writer.Write(s);
            writer.Write(true);
            AppendContent("write succ");
            stream.Close();
        }

        private void read_Click(object sender, EventArgs e)
        {
            ClearContent();
            AppendContent("Start read cpp file");
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            int num = reader.ReadInt32();
            bool b = reader.ReadBoolean();
            string str = reader.ReadString();
            bool b2 = reader.ReadBoolean();
            AppendContent("read num:" + num + " str: " + str+" b1: "+b+" b2: "+b2);
            stream.Close();
        }
    }
}
