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
using BaseOutWall.Utils;

namespace BaseOutWall
{
    public class CommandDemo
    {
        [CommandMethod("WQ")]
        //通过框选拾取梁
        public void WQ()
        {
            using (var tr = CadDrawing.Current.StartTrans())
            {

                CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)1, "0S-WALL-LINE", false);
                CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "0S-WALL-DIMS", false);
                CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "0S-BEAM-TEXT", false);
                CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)4, "0S-DETL-RBAR", false);
                //CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)5, "", false);
                //CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "", false);
                //CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)7, "", false);
                //CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)8, "", false);
                CadFileIoUtils.CadDrawingUtils_AddDimStyle(CadDrawing.Current, (short)1, "dims-100");

                tr.Commit();
            }

            //创建新窗口对象
            Form form_jiemian = new Form1();
            //显示新窗口
            form_jiemian.Show();

        }

    }
}
