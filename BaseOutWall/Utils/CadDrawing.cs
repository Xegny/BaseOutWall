using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Text;
using MgdDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using MgdApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using MgdPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;

namespace BaseOutWall.Utils
{
    public class CadDrawing
    {
        /// <summary>
        /// 当前唯一活动实例
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        public static CadDrawing HostDrawing(Transaction tr)
        {
            CadDrawing drawing = new CadDrawing();
            drawing.Start();
            drawing.StartTrans(tr);
            return drawing;
        }

        #region 实例
        //一个项目只有唯一的配置信息
        //private bool IsStart = false;
        private static CadDrawing _instance;
        private CadDrawing() { } //    IsStart = false;
        public static CadDrawing Current//Default
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = new CadDrawing();
                    }
                    catch
                    { }
                }
                return _instance;
            }
            set { _instance = value; }
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }

        #endregion 实例
        #region 对象
        /// <summary>
        /// 当前DWG文档
        /// </summary>
        public Document Cad_Doc { get; private set; }
        /// <summary>
        /// 当前图纸DWG文档数据库
        /// </summary>
        public Autodesk.AutoCAD.DatabaseServices.Database Cad_Database { get; private set; }
        /// <summary>
        /// 当前命令行
        /// </summary>
        public Editor Cad_Editor { get; private set; }

        /// <summary>
        /// 当前序列
        /// </summary>
        public int IndexUser { get; set; }
        /// <summary>
        /// 当前操作对象
        /// </summary>
        public Entity EntityUser { get; set; }
        /// <summary>
        /// 当前对象
        /// </summary>
        public DBObjectCollection EntityCollection { get; set; }


        /// <summary>
        /// 当前事务（必须被提交或释放）
        /// </summary>
        public Transaction Cad_Trans { get; private set; }

        ///// <summary>
        ///// 当前块表
        ///// </summary>
        //private BlockTable Cad_BlockTable;
        ///// <summary>
        ///// 当前块表记录
        ///// </summary>
        //private BlockTableRecord Cad_BlockTableRecord;
        ///// <summary>
        ///// 当前层表
        ///// </summary>
        //private LayerTable Cad_LayerTable;


        /// <summary>
        /// 图纸识别规则 
        /// </summary>
        //public P_DataNodeInfo Cad_Rule { get; set; }
        #endregion
        /// <summary>
        /// 初始化是否成功
        /// </summary>
        /// <returns></returns>
        public bool IsActiveDrawing()
        {
            return Cad_Doc != null && Cad_Database != null && Cad_Editor != null ? true : false;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="application"></param>
        public void Start()
        {
            if (MgdApplication.DocumentManager.MdiActiveDocument != null)
            { Cad_Doc = MgdApplication.DocumentManager.MdiActiveDocument; }
            if (Cad_Doc != null) Start(Cad_Doc);
        }
        public void Start(Document doc)
        {
            if (doc != null) Cad_Database = Cad_Doc.Database;
            if (doc != null) Cad_Editor = doc.Editor;
            EntityCollection = new DBObjectCollection();
        }
        public Transaction StartTrans()
        {
            Cad_Trans = Cad_Database.TransactionManager.StartTransaction();
            return Cad_Trans;
        }
        public Transaction StartTrans(Transaction trans)
        {
            Cad_Trans = trans;
            return Cad_Trans;
        }
        public void CommitTrans()
        {
            if (Cad_Trans != null) Cad_Trans.Commit();
        }
        public void DisposeTrans()
        {
            if (Cad_Trans != null) Cad_Trans.Dispose();
        }

        /// <summary>
        /// 当前文档文件名
        /// </summary>
        /// <returns></returns>
        public string ReturnDrawingPath()
        {
            return Cad_Doc.Name;
        }

        /// <summary>
        /// 当前规则
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //public P_DataNodeInfo ReturnDrawingRule()
        //{ return Cad_Rule; }
        //public P_DataNodeInfo ReturnDrawingRule(string path)
        //{
        //    string errMsg;
        //    return SerializationUtils.FromXml(path, out errMsg);
        //}

        /// <summary>
        /// 输出提示信息
        /// </summary>
        /// <param name="msg"></param>
        public void Log(string msg)
        {
            Cad_Editor.WriteMessage("Info: " + msg + "\n\t");
        }
        /// <summary>
        /// 输出警告信息
        /// </summary>
        /// <param name="errmsg"></param>
        public void LogFail(string errmsg)
        {
            Cad_Editor.WriteMessage("Error: " + errmsg + "\n\t");
        }

        /// <summary>
        /// 打开DWG文件
        /// </summary>
        /// <param name="filename"></param>
        public Document Open(CadDrawing drawing, string filename)
        {
            //后台打开
            //if (!IsStart)
            //    CadDrawingUtils.SendCommandLine(drawing, new string[] { "filedia", "0" });
            //IsStart = true;
            try
            {
                Document doc = MgdApplication.DocumentManager.Open(filename, false);
                MgdApplication.DocumentManager.MdiActiveDocument = doc;
                CadDrawing.Current.Cad_Doc = doc;
                CadDrawing.Current.Start(doc);
                //}

                //using (DocumentLock doclock = doc.LockDocument())
                //{
                //    //if (CadDrawing.Current.IsActiveDrawing())
                //    //{
                //    CadDrawing.Current.Cad_Editor.WriteMessage(CadDrawing.Current.Cad_Doc.Name + "Info_打开文件：" + filename + "\n\t");
                //    //}
                //};
                return doc;

            }
            catch
            {
                //CadDrawing.Current.Cad_Editor.WriteMessage(drawing.Cad_Doc.Name + "Fail_打开文件：" + filename + "失败\n\t");
                return null;
            }
        }

        //public void ProgressBar()
        //{
        //    string[] number = new string[1000];
        //    var cadbarH = new CadProgressBar();
        //    var barLength = number;//.Propertys.Select(c => c.p_Name.ToString()).ToArray();
        //    var pmH = cadbarH.ProgressBarStart(barLength, "生成进度");

        //    for (int i = 0; i < 1000; i++)
        //    {
        //        //进度条增加
        //        cadbarH.ProgressBarManaged(pmH);
        //    }
        //    //结束进度条
        //    cadbarH.ProgressBarStop(pmH);
        //}

    }

    public class RectJig : DrawJig
    {
        public Line line_1;
        public Line line_2;
        public Line line_3;
        public Line line_4;
        public Point3d BasePnt;
        public Point3d AcquirePnt;
        public RectJig(Point3d _basePnt)
        {
            line_1 = new Line();
            line_2 = new Line();
            line_3 = new Line();
            line_4 = new Line();
            BasePnt = _basePnt;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions JPPO = new JigPromptPointOptions();
            JPPO.Message = "输入矩形另一个对角点：";
            JPPO.UserInputControls = (UserInputControls.Accept3dCoordinates | UserInputControls.NullResponseAccepted | UserInputControls.AnyBlankTerminatesInput);
            PromptPointResult PR = prompts.AcquirePoint(JPPO);
            if (PR.Status != PromptStatus.OK) return SamplerStatus.Cancel;
            if (PR.Value == AcquirePnt) return SamplerStatus.NoChange;
            AcquirePnt = PR.Value; return SamplerStatus.OK;
        }
        protected override bool WorldDraw(WorldDraw draw)
        {
            Point3d point_1 = new Point3d(BasePnt.X, BasePnt.Y, 0);
            Point3d point_2 = new Point3d(AcquirePnt.X, BasePnt.Y, 0);
            line_1.StartPoint = point_1;
            line_1.EndPoint = point_2;
            point_1 = new Point3d(BasePnt.X, BasePnt.Y, 0);
            point_2 = new Point3d(BasePnt.X, AcquirePnt.Y, 0);
            line_2.StartPoint = point_1;
            line_2.EndPoint = point_2;
            point_1 = new Point3d(BasePnt.X, AcquirePnt.Y, 0);
            point_2 = new Point3d(AcquirePnt.X, AcquirePnt.Y, 0);
            line_3.StartPoint = point_1;
            line_3.EndPoint = point_2;
            point_1 = new Point3d(AcquirePnt.X, BasePnt.Y, 0);
            point_2 = new Point3d(AcquirePnt.X, AcquirePnt.Y, 0);
            line_4.StartPoint = point_1; line_4.EndPoint = point_2;
            draw.Geometry.Draw(line_1); draw.Geometry.Draw(line_2);
            draw.Geometry.Draw(line_3); draw.Geometry.Draw(line_4);
            return true;
        }
    }

    public class CADDraw
    {
        public static void AddEntity(Database db, Entity ent)
        {
            var doc = MgdApplication.DocumentManager.MdiActiveDocument;  //var db = doc.Database;
            var ed = doc.Editor;
            using (DocumentLock doclock = doc.LockDocument())
            {
                //MgdDatabase db = HostApplicationServices.WorkingDatabase;
                ObjectId entId;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    entId = btr.AppendEntity(ent); trans.AddNewlyCreatedDBObject(ent, true); trans.Commit();
                }
                //return entId;
            }

        }
    }

    public static class CadProgressBarUtils
    {
        public static ProgressMeter Star(this ProgressMeter pm, string name)
        {
            pm = new ProgressMeter();
            pm.Start(name);
            return pm;
        }

        public static void Max(this ProgressMeter pm, int count)
        {
            pm.SetLimit(count * 1);
        }

        public static void Run(this ProgressMeter pm)
        {
            ////// This allows AutoCAD to repaint
            ////System.Windows.Forms.Application.DoEvents();
            //////System.Threading.Thread.Sleep(1);
            ////// Increment Progress Meter...
            ////pm.MeterProgress();
            //pm.Star("当前进度1%");

            //System.Threading.Thread.Sleep(5);
            // Increment Progress Meter...
            pm.MeterProgress();
            // This allows AutoCAD to repaint
            System.Windows.Forms.Application.DoEvents();
        }

        public static void ProgressBarManaged(ProgressMeter pm)
        {
            for (int step = 0; step < 1; step++)
            {
                int n = 0;
                while (n < 100)//10
                {
                    // This allows AutoCAD to repaint
                    System.Windows.Forms.Application.DoEvents();
                    //System.Threading.Thread.Sleep(1);
                    // Increment Progress Meter...
                    pm.MeterProgress();
                    n++;

                }
            }

        }

        public static void Stop(this ProgressMeter pm)
        {
            pm.Stop();
        }
    }
}
