using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.IO;

namespace BaseOutWall
{
    public class CommandDemo
    {
        [CommandMethod("WQ")]
        //通过框选拾取梁
        public void WQ()
        {
            //创建新窗口对象
            Form form_jiemian = new Form1();
            //显示新窗口
            form_jiemian.Show();

        }

    }
}
