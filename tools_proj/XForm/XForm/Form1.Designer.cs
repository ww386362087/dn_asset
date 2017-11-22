namespace XForm
{
    partial class XCForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.pathLbl = new System.Windows.Forms.Label();
            this.makeByteBtn = new System.Windows.Forms.Button();
            this.contentLbl = new System.Windows.Forms.RichTextBox();
            this.makeCodeBtn = new System.Windows.Forms.Button();
            this.clearBtn = new System.Windows.Forms.Button();
            this.buildBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "表格路径：";
            // 
            // pathLbl
            // 
            this.pathLbl.AutoSize = true;
            this.pathLbl.Location = new System.Drawing.Point(126, 32);
            this.pathLbl.Name = "pathLbl";
            this.pathLbl.Size = new System.Drawing.Size(71, 12);
            this.pathLbl.TabIndex = 1;
            this.pathLbl.Text = "d://Assets/";
            // 
            // makeByteBtn
            // 
            this.makeByteBtn.Location = new System.Drawing.Point(37, 73);
            this.makeByteBtn.Name = "makeByteBtn";
            this.makeByteBtn.Size = new System.Drawing.Size(75, 23);
            this.makeByteBtn.TabIndex = 2;
            this.makeByteBtn.Text = "生成Bytes";
            this.makeByteBtn.UseVisualStyleBackColor = true;
            this.makeByteBtn.Click += new System.EventHandler(this.bytesBtn_Click);
            // 
            // contentLbl
            // 
            this.contentLbl.Location = new System.Drawing.Point(37, 116);
            this.contentLbl.Name = "contentLbl";
            this.contentLbl.Size = new System.Drawing.Size(490, 273);
            this.contentLbl.TabIndex = 3;
            this.contentLbl.Text = "";
            // 
            // makeCodeBtn
            // 
            this.makeCodeBtn.Location = new System.Drawing.Point(151, 73);
            this.makeCodeBtn.Name = "makeCodeBtn";
            this.makeCodeBtn.Size = new System.Drawing.Size(75, 23);
            this.makeCodeBtn.TabIndex = 4;
            this.makeCodeBtn.Text = "生成代码";
            this.makeCodeBtn.UseVisualStyleBackColor = true;
            this.makeCodeBtn.Click += new System.EventHandler(this.makeCodeBtn_Click);
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(412, 73);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(75, 23);
            this.clearBtn.TabIndex = 5;
            this.clearBtn.Text = "清除";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // buildBtn
            // 
            this.buildBtn.Location = new System.Drawing.Point(283, 73);
            this.buildBtn.Name = "buildBtn";
            this.buildBtn.Size = new System.Drawing.Size(75, 23);
            this.buildBtn.TabIndex = 6;
            this.buildBtn.Text = "Build";
            this.buildBtn.UseVisualStyleBackColor = true;
            this.buildBtn.Click += new System.EventHandler(this.build_Click);
            // 
            // XCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 417);
            this.Controls.Add(this.buildBtn);
            this.Controls.Add(this.clearBtn);
            this.Controls.Add(this.makeCodeBtn);
            this.Controls.Add(this.contentLbl);
            this.Controls.Add(this.makeByteBtn);
            this.Controls.Add(this.pathLbl);
            this.Controls.Add(this.label1);
            this.Name = "XCForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label pathLbl;
        private System.Windows.Forms.Button makeByteBtn;
        private System.Windows.Forms.RichTextBox contentLbl;
        private System.Windows.Forms.Button makeCodeBtn;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Button buildBtn;
    }
}

