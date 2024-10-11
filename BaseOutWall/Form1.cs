using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BaseOutWall
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //dataGridView1.RowHeadersVisible = false;
            dataGridView1.Rows[0].Cells[0].Value = "-1";
            dataGridView1.Rows.Add("-1", "-3","3000","200","400","2000","1500");
            dataGridView1.Rows.Add("-1", "-6", "3000", "200","400", "2000", "1500");
            this.comboBox1.SelectedIndex = 4;
            this.comboBox2.SelectedIndex = 1;
            this.Boundaryconditions.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 5;
            this.comboBox5.SelectedIndex = 2;
            this.comboBox6.SelectedIndex = 2;
            this.comboBox7.SelectedIndex = 2;
            this.comboBox8.SelectedIndex = 0;
            this.comboBox9.SelectedIndex = 0;
            this.comboBox10.SelectedIndex = 0;
            // 两墙合一
            this.label4.Visible = false;
            this.textBox3.Visible = false;
            //地下室顶板刚接
            this.radioButton6.Visible = false;
            this.radioButton7.Visible = false;
            this.label14.Visible = false;
            this.textBox7.Visible = false;
            this.label15.Visible = false;
            this.textBox8.Visible = false;


        }


        private void Form1_Load(object sender, EventArgs e)
        {


        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void AddRowButton_Click(object sender, EventArgs e)//添加层
        {
            // 创建新行
            DataGridViewRow newRow = new DataGridViewRow();

            // 添加新行到 DataGridView
            dataGridView1.Rows.Add(newRow);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 检查是否是目标列的索引
            if (e.ColumnIndex == 0 )
            {
                // 设置单元格的显示值
                e.Value = (-(e.RowIndex + 1)).ToString();
                e.FormattingApplied = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)//绘图
        {
            
            //固定参数
            double dianceng = Double.Parse(text_dianceng.Text);
            double dingbiaogao = Double.Parse(text_dingbiaogao.Text);
            double waiqiangkuan = Double.Parse(text_waiqiangkuan.Text); 
            double dibanhou = Double.Parse(text_dibanhou.Text);

            //将随数据表的可变参数，每一行是一个对象
            List<Story> storyList = new List<Story>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    Story story = new Story();
                    story.StoryNum = row.Cells["Column1"].FormattedValue.ToString();
                    story.StoryElevation = row.Cells["Column2"].Value.ToString();
                    story.StoryHeight = row.Cells["Column3"].Value.ToString();
                    story.SlabT = row.Cells["Column4"].Value.ToString();
                    story.WallThickness = row.Cells["Column5"].Value.ToString();
                    story.SlabRebarArea = row.Cells["Column6"].Value.ToString();
                    story.MidwallRebarArea = row.Cells["Column7"].Value.ToString();
                    storyList.Add(story);
                }
            }
            double neiqiangkuan = Double.Parse(storyList[0].WallThickness);
            double xianeiqiangkuan = Double.Parse(storyList[storyList.Count - 1].WallThickness);

            this.Hide();

            //运行画图函数
            OutWall.CreateOutWall(dianceng, dingbiaogao, neiqiangkuan, xianeiqiangkuan,waiqiangkuan, dibanhou,storyList);
        }

        private void button2_Click(object sender, EventArgs e)//删除层
        {
            // 检查是否有选中的行
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // 获取选中行的索引
                int rowIndex = dataGridView1.SelectedRows[0].Index;

                // 检查选中行是否为新行
                if (dataGridView1.Rows[rowIndex].IsNewRow)
                {
                    // 如果是新行，直接取消添加
                    dataGridView1.CancelEdit();
                }
                else
                {
                    // 如果不是新行，删除选中行
                    dataGridView1.Rows.RemoveAt(rowIndex);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)//计算按钮
        {
            // listView_Reinforcement.Items.Clear();
            //固定参数
            double dianceng = Double.Parse(text_dianceng.Text);
            double dingbiaogao = Double.Parse(text_dingbiaogao.Text);
            double waiqiangkuan = Double.Parse(text_waiqiangkuan.Text);
            double dibanhou = Double.Parse(text_dibanhou.Text);

            //将随数据表的可变参数，每一行是一个对象
            List<Story> storyList = new List<Story>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    Story story = new Story();
                    story.StoryNum = row.Cells["Column1"].FormattedValue.ToString();
                    story.StoryElevation = row.Cells["Column2"].Value.ToString();
                    story.StoryHeight = row.Cells["Column3"].Value.ToString();
                    story.SlabT = row.Cells["Column4"].Value.ToString();
                    story.SlabRebarArea = row.Cells["Column6"].Value.ToString();
                    story.MidwallRebarArea = row.Cells["Column7"].Value.ToString();
                    storyList.Add(story);
                }
            }

            //枚举法配通长筋
            List<PeiJin> potentialResults = new List<PeiJin>();//potentialResults是所有配筋方案的清单
            double maxMidwallRebarArea = Double.Parse(storyList.Max(story => story.MidwallRebarArea));

            for (int i = 8; i <= 28; i += 2)
            {
                for (int j = 100; j <= 200; j += 50)
                {
                    // 在这里使用 i 和 j 进行需要的配筋操作
                    if (i * j> maxMidwallRebarArea)
                    {
                        double duoyu = i * j - maxMidwallRebarArea;
                        PeiJin peijin = new PeiJin(i,j, duoyu);
                        potentialResults.Add(peijin);  
                    }                    
                }
            }
            var sortedResults = potentialResults.OrderBy(result => result.Duoyu);
            //结果1：最省的内侧配筋方案
            var minDuoyuPeijin = sortedResults.FirstOrDefault();
            if (minDuoyuPeijin==null)
            {
                MessageBox.Show("寻找方案失败！");
            }
            // 创建一个新的 ListViewItem 对象
            ListViewItem item1 = new ListViewItem();
            // 将 minDuoyuPeijin 对象的属性添加到第一行的第一格
            item1.Text = minDuoyuPeijin.Zhijin.ToString()+"@"+ minDuoyuPeijin.Jianju.ToString();
            // 将 ListViewItem 添加到 ListView 的项集合中
            // listView_Reinforcement.Items.Add(item1);

            //假设：外侧的通长钢筋和内侧的是一样的
            double tongchanggangjinmianji = minDuoyuPeijin.Zhijin * minDuoyuPeijin.Jianju;

            //枚举法找到需要加设附加筋的楼板
            List<WaiPeiJin> geceng_minDuoyuPeijin = new List<WaiPeiJin>();
            int jishuqi=0;
            for (int i = 0; i < storyList.Count; i++)
            {
                double fujiagangjinmianji = Double.Parse(storyList[i].SlabRebarArea) - tongchanggangjinmianji;
                if (fujiagangjinmianji < 0)
                {
                    continue; // 当 fujiagangjinmianji 小于 0 时跳过当前循环
                }
                //枚举法配附加筋
                List<WaiPeiJin> potentialResults3 = new List<WaiPeiJin>();
                for (int k = 8; k <= 28; k += 2)
                {
                    for (int m = 100; m <= 200; m += 50)
                    {
                        // 在这里使用 i 和 j 进行需要的配筋操作
                        if (k * m > fujiagangjinmianji)
                        {
                            double duoyu = k * m - fujiagangjinmianji;
                            WaiPeiJin waipeijin = new WaiPeiJin(k, m, duoyu);
                            potentialResults3.Add(waipeijin);
                        }
                    }
                }
                var sortedResults3 = potentialResults3.OrderBy(result => result.Duoyu);
                var minDuoyuPeijin3 = sortedResults3.FirstOrDefault();
                if (minDuoyuPeijin3 == null)
                {
                    jishuqi++;
                }
                //结果2：最省的外侧附加筋方案清单
                geceng_minDuoyuPeijin.Add(minDuoyuPeijin3);

            }
            if (jishuqi >0)
            {
                MessageBox.Show("寻找方案失败！");
            }
            // 遍历 geceng_minDuoyuPeijin 列表
            foreach (var obj in geceng_minDuoyuPeijin)
            {
                // 创建一个新的 ListViewItem 对象
                ListViewItem item = new ListViewItem();

                // 将 obj 对象的属性添加到第二行的每个格子中
                item.Text=obj.Zhijin.ToString() + "@" + obj.Jianju.ToString();
                // 添加更多属性...

                // 将 ListViewItem 添加到 ListView 的项集合中
               // listView_Reinforcement.Items.Add(item);
            }


        }

        private void 保存数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.Title = "保存数据";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                // 将数据保存到文本文件
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write("底层垫层厚度" + ":" + text_dianceng.Text + "\n");
                    writer.Write("地下室顶标高" + ":" + text_dingbiaogao.Text + "\n");
                    writer.Write("底层板厚" + ":" + text_dibanhou.Text + "\n");
                    writer.Write("外墙宽" + ":" + text_waiqiangkuan.Text + "\n");
                }
                MessageBox.Show("数据保存成功！");
            }

        }

        private void 导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.Title = "选择导入数据源";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // 读取文本文件并填充数据到窗体
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("底层垫层厚度"))
                        {
                            text_dianceng.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("地下室顶标高"))
                        {
                            text_dingbiaogao.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("底层板厚"))
                        {
                            text_dibanhou.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("外墙宽"))
                        {
                            text_waiqiangkuan.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                    }
                }
                MessageBox.Show("数据导入成功！");
            }
        }

        private void Form1_unLoad(object sender, EventArgs e)
        {
            this.Close();
            FlushMemory();

        }

        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        private static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }


        private void button3_Click(object sender, EventArgs e)//取消
        {
            this.Dispose();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int BoundaryIndex = this.Boundaryconditions.SelectedIndex = 1;
          
            if (BoundaryIndex==1)
            {
                this.radioButton6.Visible =true;
                this.radioButton7.Visible = true;
                this.label14.Visible = true;
                this.textBox7.Visible = true;
                this.label15.Visible = true;
                this.textBox8.Visible = true;
            }
            else if (BoundaryIndex ==  0)
            {
                this.radioButton6.Visible = false;
                this.radioButton7.Visible = false;
                this.label14.Visible = false;
                this.textBox7.Visible = false;
                this.label15.Visible = false;
                this.textBox8.Visible = false;
            }
            else if (BoundaryIndex == 2)
            {
                this.radioButton6.Visible = false;
                this.radioButton7.Visible = false;
                this.label14.Visible = false;
                this.textBox7.Visible = false;
                this.label15.Visible = false;
                this.textBox8.Visible = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.label4.Visible = false;
            this.textBox3.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.label4.Visible = false;
            this.textBox3.Visible = false;
        }
    }
}
