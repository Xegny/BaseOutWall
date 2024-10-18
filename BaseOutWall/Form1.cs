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
            dataGridView1.Rows.Add("-1", "-3", "3000", "200", "400", "2000", "1500");
            dataGridView1.Rows.Add("-1", "-6", "3000", "200", "400", "2000", "1500");
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
            this.text_dibantuchuwaiqiangchicun.Visible = false;
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
            if (e.ColumnIndex == 0)
            {
                // 设置单元格的显示值
                e.Value = (-(e.RowIndex + 1)).ToString();
                e.FormattingApplied = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)//绘图
        {
            try
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

                var data = new WallData
                {
                    dianceng = dianceng,
                    dingbiaogao = dingbiaogao,
                    neiqiangkuan = neiqiangkuan,
                    xianeiqiangkuan = xianeiqiangkuan,
                    waiqiangkuan = waiqiangkuan,
                    dibanhou = dibanhou,
                    storyList = storyList
                };

                //运行画图函数
                OutWall.CreateOutWall(data);
                //OutWall.CreateOutWall(dianceng, dingbiaogao, neiqiangkuan, xianeiqiangkuan, waiqiangkuan, dibanhou, storyList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
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
                    if (i * j > maxMidwallRebarArea)
                    {
                        double duoyu = i * j - maxMidwallRebarArea;
                        PeiJin peijin = new PeiJin(i, j, duoyu);
                        potentialResults.Add(peijin);
                    }
                }
            }
            var sortedResults = potentialResults.OrderBy(result => result.Duoyu);
            //结果1：最省的内侧配筋方案
            var minDuoyuPeijin = sortedResults.FirstOrDefault();
            if (minDuoyuPeijin == null)
            {
                MessageBox.Show("寻找方案失败！");
            }
            // 创建一个新的 ListViewItem 对象
            ListViewItem item1 = new ListViewItem();
            // 将 minDuoyuPeijin 对象的属性添加到第一行的第一格
            item1.Text = minDuoyuPeijin.Zhijin.ToString() + "@" + minDuoyuPeijin.Jianju.ToString();
            // 将 ListViewItem 添加到 ListView 的项集合中
            // listView_Reinforcement.Items.Add(item1);

            //假设：外侧的通长钢筋和内侧的是一样的
            double tongchanggangjinmianji = minDuoyuPeijin.Zhijin * minDuoyuPeijin.Jianju;

            //枚举法找到需要加设附加筋的楼板
            List<WaiPeiJin> geceng_minDuoyuPeijin = new List<WaiPeiJin>();
            int jishuqi = 0;
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
            if (jishuqi > 0)
            {
                MessageBox.Show("寻找方案失败！");
            }
            // 遍历 geceng_minDuoyuPeijin 列表
            foreach (var obj in geceng_minDuoyuPeijin)
            {
                // 创建一个新的 ListViewItem 对象
                ListViewItem item = new ListViewItem();

                // 将 obj 对象的属性添加到第二行的每个格子中
                item.Text = obj.Zhijin.ToString() + "@" + obj.Jianju.ToString();
                // 添加更多属性...

                // 将 ListViewItem 添加到 ListView 的项集合中
                // listView_Reinforcement.Items.Add(item);
            }


        }

        private void 保存数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void 导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

            if (BoundaryIndex == 1)
            {
                this.radioButton6.Visible = true;
                this.radioButton7.Visible = true;
                this.label14.Visible = true;
                this.textBox7.Visible = true;
                this.label15.Visible = true;
                this.textBox8.Visible = true;
            }
            else if (BoundaryIndex == 0)
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
            this.text_dibantuchuwaiqiangchicun.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.label4.Visible = false;
            this.text_dibantuchuwaiqiangchicun.Visible = false;
        }

        private void ClearTableButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void ImportTableButton_Click(object sender, EventArgs e)
        {
            #region 选择文件

            var FilePath = string.Empty;
            var openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = false;//允许同时选择多个文件 
            //openFileDialog1.InitialDirectory = "e:\\";
            openFileDialog1.Filter = "构件列表(*.csv)|*.csv";// "图纸目录(*.txt)|*.txt|图纸目录(*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog1.FileNames[0].ToString();
            }

            #endregion

            if (File.Exists(FilePath))
            {
                FromCsv(dataGridView1, FilePath);
                MessageBox.Show("表格导入成功");
            }
        }

        private void ExportTableButton_Click(object sender, EventArgs e)
        {
            #region 选择文件

            var FilePath = string.Empty;
            var saveFileDialog1 = new SaveFileDialog();
            //设置文件类型 
            saveFileDialog1.Filter = "构件列表(*.csv)|*.csv";//|构件列表(*.*)|*.*"; 
            //设置默认文件类型显示顺序 
            saveFileDialog1.FilterIndex = 1;
            //保存对话框是否记忆上次打开的目录 
            saveFileDialog1.RestoreDirectory = true;
            //设置默认的文件名
            saveFileDialog1.DefaultExt = "构件列表";// in wpf is  sfd.FileName = "YourFileName";
            //点了保存按钮进入 
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = saveFileDialog1.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                FilePath = localFilePath;

            }

            ToCsv(dataGridView1, FilePath);

            if (!File.Exists(FilePath)) MessageBox.Show("表格导出失败");

            #endregion
        }

        private void FromCsv(DataGridView dv, string path)
        {
            var list = ReadFile(path);

            dv.Rows.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var r = list[i];
                dv.Rows.Add(r);
            }
        }

        private void ToCsv(DataGridView dv1, string path)
        {
            var list = new List<string[]>();

            for (int i = 0; i < dv1.RowCount; i++)
            {
                var ro = dv1.Rows[i];
                var array = new List<string>();
                for (int j = 0; j < ro.Cells.Count; j++)
                {
                    var value = ro.Cells[j].Value;
                    if (value == null)
                        array.Add("");
                    else
                        array.Add(value.ToString());
                }
                list.Add(array.ToArray());
            }

            SaveFile(path, list);
        }

        /// <summary>
        /// 生成CSV文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="list"></param>
        public static void SaveFile(string fileName, List<string[]> list)
        {
            try
            {
                string path = fileName;
                if (File.Exists(path)) File.Delete(path);
                //创建StreamWriter 类的实例
                StreamWriter streamWriter = new StreamWriter(path, true, Encoding.Default);

                //streamWriter.WriteLine("小张");
                foreach (var row in list)
                {
                    var value = string.Empty;
                    foreach (var col in row)
                    {
                        value += col + ",";
                    }
                    streamWriter.WriteLine(value);
                }

                //刷新缓存
                streamWriter.Flush();
                //关闭流
                streamWriter.Close();
            }
            catch
            { MessageBox.Show("导出失败"); }

        }
        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string[]> ReadFile(string fileName)
        {
            var list = new List<string[]>();
            if (File.Exists(fileName))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fileName, Encoding.Default))
                    {
                        //tbInput.Text = sr.ReadToEnd();
                        //byte[] mybyte = Encoding.UTF8.GetBytes(tbInput.Text); 
                        while (true)
                        {
                            var line = sr.ReadLine(); if (line == null) break;
                            var array = line.Split(',');
                            if (array.Length != 0) list.Add(array);
                        }
                    }
                    //MessageBox.Show(list.Count.ToString());
                }
                catch
                { MessageBox.Show("先关闭打开的文件"); }
            }
            else
            {
                MessageBox.Show("文件不存在");
            }
            return list;
        }

        private void 批量导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
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
                        if (line.Contains("外墙厚"))
                        {
                            text_waiqiangkuan.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("止水钢板"))
                        {
                            text_zhishuigangban.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("底板突出外墙尺寸"))
                        {
                            text_dibantuchuwaiqiangchicun.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("地下室顶标高"))
                        {
                            text_dingbiaogao.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("底层板厚"))
                        {
                            text_dibanhou.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("底层垫层厚度"))
                        {
                            text_dianceng.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                        else if (line.Contains("室外地坪标高"))
                        {
                            text_shiwaidipingbiaogao.Text = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                        }
                    }
                }
                MessageBox.Show("数据导入成功！");
            }
        }

        private void 保存批量数据ToolStripMenuItem_Click(object sender, EventArgs e)
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
                    writer.Write("外墙厚" + ":" + text_waiqiangkuan.Text + "\n");
                    writer.Write("止水钢板" + ":" + text_zhishuigangban.Text + "\n");
                    writer.Write("底板突出外墙尺寸" + ":" + text_dibantuchuwaiqiangchicun.Text + "\n");
                    writer.Write("地下室顶标高" + ":" + text_dingbiaogao.Text + "\n");
                    writer.Write("底层板厚" + ":" + text_dibanhou.Text + "\n");
                    writer.Write("底层垫层厚度" + ":" + text_dianceng.Text + "\n");
                    writer.Write("室外地坪标高" + ":" + text_shiwaidipingbiaogao.Text + "\n");


                }
                MessageBox.Show("数据保存成功！");
            }
        }

    }
}
