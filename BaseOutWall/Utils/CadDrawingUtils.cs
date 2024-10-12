using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MgdDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using mgdApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using Line = Autodesk.AutoCAD.DatabaseServices.Line;
using Autodesk.AutoCAD.Runtime;
using Exception = System.Exception;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.ApplicationServices;
using System.Linq;
using System.Numerics;
using Plane = Autodesk.AutoCAD.Geometry.Plane;
//using System.Windows.Documents;
//using Windows.Devices.Pwm;
//using System.Numerics;

namespace BaseOutWall.Utils
{
    /// <summary>
    /// CAD 对象调用静态方法
    /// </summary>
    internal static class CadDrawingUtils
    {
        public static void AddLayer(CadDrawing drawing, short colorIndex, string layerName)
        {
            try
            {
                short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256 using 
                //层表记录
                var layerTable = GetLayerTables(drawing);
                if (!layerTable.Has(layerName))
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = layerName;
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                    var layerId = layerTable.Add(ltr);
                    drawing.Cad_Trans.AddNewlyCreatedDBObject(ltr, true);
                    SetCurrentLayer(drawing, ltr);
                }
                else
                {
                    LayerTableRecord ltr = drawing.Cad_Trans.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                    SetCurrentLayer(drawing, ltr);
                }
            }
            catch (Exception)
            { }
        }
        /// <summary>
        /// 获取层表记录（只读）
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static LayerTable GetLayerTables(CadDrawing drawing)
        {
            if (drawing.Cad_Database == null) return null;
            if (drawing.Cad_Trans == null) return null;
            ObjectId LayerTableId = drawing.Cad_Database.LayerTableId;
            var Cad_LayerTable = (LayerTable)drawing.Cad_Trans.GetObject(LayerTableId, OpenMode.ForWrite);
            return Cad_LayerTable;
        }
        /// <summary>
        /// 获取层表记录并且可用（读写）
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static LayerTable GetLayerTableRecords(CadDrawing drawing)
        {
            if (drawing.Cad_Database == null) return null;
            if (drawing.Cad_Trans == null) return null;
            ObjectId LayerTableId = drawing.Cad_Database.LayerTableId;
            var Cad_LayerTable = (LayerTable)drawing.Cad_Trans.GetObject(LayerTableId, OpenMode.ForWrite);
            foreach (var layerId in Cad_LayerTable)
            {
                try
                {
                    LayerTableRecord ltr = (LayerTableRecord)drawing.Cad_Trans.GetObject(layerId, OpenMode.ForWrite);
                    if (ltr.IsFrozen)
                    { ltr.IsFrozen = false; }
                    if (ltr.IsLocked)
                    { ltr.IsLocked = false; }
                }
                catch { }
            }
            return Cad_LayerTable;
        }
        /// <summary>
        /// 获取层名
        /// </summary>
        /// <param name="layeRecords"></param>
        /// <returns></returns>
        public static string[] GetLayerNames(IList<LayerTableRecord> layeRecords)
        {
            var layerNames = layeRecords.Select(c => c.Name).ToArray();
            return layerNames.Length != 0 ? layerNames : new string[] { };
        }
        public static LayerTableRecord[] GetLayerTableRecords()
        {

            List<LayerTableRecord> list = new List<LayerTableRecord>();
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            using (DocumentLock doclock = doc.LockDocument())
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    ObjectId LayerTableId = db.LayerTableId;
                    var Cad_LayerTable = (LayerTable)tr.GetObject(LayerTableId, OpenMode.ForWrite);

                    foreach (var id in Cad_LayerTable)
                    {
                        try
                        {
                            var ltr = (LayerTableRecord)tr.GetObject(id, OpenMode.ForWrite, true);
                            if (ltr != null)
                            {
                                list.Add(ltr);
                            }
                        }
                        catch (Exception)
                        { }
                    }
                    tr.Commit();
                }
            };

            return list.ToArray();
        }

        public static void SetCurrentLayer(CadDrawing drawing, LayerTableRecord layer)
        {
            if (layer.ObjectId != ObjectId.Null) drawing.Cad_Database.Clayer = layer.ObjectId;
        }
        public static void SetCurrentLayer(CadDrawing drawing, short colorIndex, string layerName)
        {
            try
            {
                short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256 using 
                //层表记录
                var layerTable = GetLayerTables(drawing);
                if (!layerTable.Has(layerName))
                {

                }
                else
                {
                    LayerTableRecord ltr = drawing.Cad_Trans.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                    SetCurrentLayer(drawing, ltr);
                }
            }
            catch (Exception)
            { }
        }

        public static ObjectId[] GetObjectIds(CadDrawing drawing, Type[] tps, string message = "正在遍历所有图元...\r\n")
        {
            drawing.Cad_Editor.WriteMessage(message);
            var db = drawing.Cad_Database;
            var list = new List<ObjectId>();
            using (var tr = drawing.StartTrans())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (var id in btr)
                {
                    var entity = (Entity)tr.GetObject(id, OpenMode.ForWrite, true);
                    if (entity.GetType() != typeof(DBText) &&
                        entity.GetType() != typeof(BlockReference)) continue;
                    list.Add(entity.Id);
                }
                tr.Commit();
            }
            return list.ToArray();
        }

        public static ObjectId[] GetSelectionSet(CadDrawing drawing, Type[] tps, Point3d[] pts, string message)
        {
            drawing.Cad_Editor.WriteMessage(message);
            PromptSelectionOptions selops = new PromptSelectionOptions();
            // 建立过滤器
            SelectionFilter filter = GetSelectionFilter(drawing, tps);
            // 按照过滤器进行选择
            PromptSelectionResult ents = null;
            if (pts.Length != 2)
                ents = drawing.Cad_Editor.GetSelection(selops, filter);
            else
                ents = drawing.Cad_Editor.SelectCrossingWindow(pts[1], pts[0], filter);
            if (ents.Status == PromptStatus.OK)
            {
                SelectionSet SS = ents.Value;
                return SS.GetObjectIds();
            }
            return new ObjectId[] { };
        }
        private static SelectionFilter GetSelectionFilter(CadDrawing drawing, Type[] tps)
        {
            var types = GetDxfNames(tps);
            Database db = drawing.Cad_Database;
            Editor ed = drawing.Cad_Editor;
            PromptSelectionOptions selops = new PromptSelectionOptions();
            // 建立选择的过滤器内容
            TypedValue[] filList = new TypedValue[tps.Length + 2];
            filList[0] = new TypedValue((int)DxfCode.Operator, "<or");
            filList[tps.Length + 1] = new TypedValue((int)DxfCode.Operator, "or>");
            for (int i = 0; i < tps.Length; i++)
            {
                filList[i + 1] = new TypedValue((int)DxfCode.Start, types[i]);
            }
            // 建立过滤器
            SelectionFilter filter = new SelectionFilter(filList);
            return filter;
        }
        private static string[] GetDxfNames(IList<Type> types)
        {
            List<string> dxfNames = new List<string>();
            foreach (var type in types)
            {
                dxfNames.Add(GetDxfName(type));
                //if(GetDxfName(type)=="LWPOLYLINE")  dxfNames.Add(typeof(Polyline).Name);
                //if (GetDxfName(type) == "3DFACE") dxfNames.Add(typeof(Face).Name);
                if (GetDxfName(type) == "3DFACE") dxfNames.Add("FACE");
            }
            return dxfNames.ToArray();
        }
        private static string GetDxfName(Type type)
        {
            //if (type.Equals(typeof (Face))) return "Face";
            return RXObject.GetClass(type).DxfName;
            //switch (type.Name.ToString())
            //{
            //    case "Curve":
            //        return RXObject.GetClass(typeof(Curve)).DxfName;
            //    case "Dimension":
            //        return RXObject.GetClass(typeof(Dimension)).DxfName;
            //    case "Polyline":
            //        var p = RXObject.GetClass(typeof(Polyline)).DxfName;
            //        return p;//
            //    case "Polyline2d":
            //        var p2 = RXObject.GetClass(typeof(Polyline2d)).DxfName;
            //        return p2;//
            //    case "Polyline3d":
            //        var p3 = RXObject.GetClass(typeof(Polyline3d)).DxfName;
            //        return p3;//
            //    case "BlockReference":
            //        return RXObject.GetClass(typeof(BlockReference)).DxfName;
            //    case "Circle":
            //        return RXObject.GetClass(typeof(Circle)).DxfName;
            //    case "Line":
            //        return RXObject.GetClass(typeof(Line)).DxfName;
            //    case "Arc":
            //        return RXObject.GetClass(typeof(Arc)).DxfName;
            //    case "DBText":
            //        return RXObject.GetClass(typeof(DBText)).DxfName;
            //    case "MText":
            //        return RXObject.GetClass(typeof(MText)).DxfName;
            //    case "Face":
            //        return RXObject.GetClass(typeof(Face)).DxfName;
            //    default:
            //        return RXObject.GetClass(typeof(Entity)).DxfName;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="message"></param>
        public static void SendMessage(CadDrawing drawing, string message)
        {
            try
            {
                drawing.Cad_Editor.WriteMessage(message + "\n\t");
            }
            catch //()Exception
            { }
        }

        /// <summary>
        /// ZOOMALL
        /// </summary>
        /// <param name="ed"></param>
        internal static void ZoomExtents(Editor ed)
        {
            var db = ed.Document.Database;
            db.UpdateExt(false);
            Extents3d ext = (short)mgdApplication.GetSystemVariable("cvport") == 1 ?
                new Extents3d(db.Pextmax, db.Pextmax) :
                new Extents3d(db.Extmin, db.Extmax);
            Zoom(ed, ext);
        }
        internal static void Zoom(Editor ed, Extents3d ext)
        {
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                ext.TransformBy(WorldToEye(view));
                view.Width = ext.MaxPoint.X - ext.MinPoint.X;
                view.Height = ext.MaxPoint.Y - ext.MinPoint.Y;
                view.CenterPoint = new Point2d((ext.MaxPoint.X + ext.MinPoint.X) / 2,
                                               (ext.MaxPoint.Y + ext.MinPoint.Y) / 2);
                ed.SetCurrentView(view);
            }
        }
        internal static void Zoom(Editor ed, Point3d pt, int scale = 100)
        {
            var origin = new Point3d(pt.X, pt.Y, pt.Z);
            var min = origin + new Vector3d(-300, -300, 0);
            var max = origin + new Vector3d(+300 + 200, 300 + 100, 0);
            var box = new Extents3d(min, max);
            CadDrawingUtils.Zoom(ed, box);
        }
        internal static void Zoom(Editor ed, Point3d pt1, Point3d pt2)
        {
            //var origin = new Point3d(pt.X, pt.Y, pt.Z);
            var min = pt1; // origin + new Vector3d(-scale, -scale, 0);
            var max = pt2 + new Vector3d(300 + 300, 300 + 100, 0);// origin + new Vector3d(scale, scale, 0);
            var box = new Extents3d(min, max);
            CadDrawingUtils.Zoom(ed, box);
        }
        internal static Matrix3d WorldToEye(ViewTableRecord view)
        {
            return EyeToWorld(view).Inverse();
        }
        internal static Matrix3d EyeToWorld(ViewTableRecord view)
        {
            return Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) *
                   Matrix3d.Displacement(view.Target - Point3d.Origin) *
                   Matrix3d.PlaneToWorld(view.ViewDirection);
        }

        /// <summary>
        /// 提示用户输入非整数 
        /// </summary> 
        /// <param name="message">提示</param>
        /// <returns>用户输入的整数</returns>
        public static double GetIntputDoulbe(string message)
        {
            var ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            var i = ed.GetDouble(message);
            if (i.Status == PromptStatus.OK) { return (int)i.Value; } else { return 0; }
        }

        /// <summary>
        /// 提示用户输入整数 
        /// </summary> 
        /// <param name="message">提示</param>
        /// <returns>用户输入的整数</returns>
        public static int GetIntputInt(string message)
        {
            var ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            var i = ed.GetInteger(message);
            if (i.Status == PromptStatus.OK) { return (int)i.Value; } else { return 0; }
        }

        /// <summary>
        /// 提示用户输入字符 
        /// </summary> 
        /// <param name="message">提示</param>
        /// <returns>用户输入的整数</returns>
        public static string GetIntputKey(string message)
        {
            var doc = mgdApplication.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            PromptResult i = ed.GetString(message);
            if (i.Status == PromptStatus.OK)
            { return (string)i.StringResult; }
            else { return string.Empty; }
        }
        public static string GetIntputKey(string message, Dictionary<string, string[]> keyValues)
        {
            var doc = mgdApplication.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            PromptKeywordOptions pkeyOpts = new PromptKeywordOptions("select");
            pkeyOpts.Message = message;
            foreach (var key in keyValues.Keys)
            {
                var val = keyValues[key];
                if (val.Length != 2) continue;
                pkeyOpts.Keywords.Add(val[1], val[1], val[0], true, true);
            }
            pkeyOpts.AllowNone = true;
            var i = ed.GetKeywords(pkeyOpts);
            //mgdApplication.ShowAlertDialog(message + i.StringResult);

            if (i.Status == PromptStatus.OK)
            {
                return (string)i.StringResult;
            }
            else
            {
                return (string)i.StringResult;
            }

        }

        /// <summary>
        /// 获取二级图块名判断是否图框
        /// </summary>
        /// <param name="blockRef"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public static List<string> GetBlockSubBlockNames(BlockReference blockRef, Transaction tr)
        {
            var subBlocks = new List<BlockReference>();
            if (blockRef != null)
            {
                BlockTableRecord block = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                if (block != null)
                {
                    foreach (ObjectId objId in block)
                    {
                        Entity entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                        var dt = entity as BlockReference;
                        if (dt == null) continue;
                        if (dt != null)
                        {
                            subBlocks.Add(dt);
                        }
                    }
                }
            }
            return subBlocks.Select(c => c.Name).ToList();
        }
        public static bool 获取二级图块名判断是否图框(BlockReference blockRef, Transaction tr)
        {
            var brNames = CadDrawingUtils.GetBlockSubBlockNames(blockRef, tr);
            if (brNames.Where(c => c.Contains("华东院")).FirstOrDefault() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool 获取二级图块名判断是否图框(BlockReference blockRef)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            using (DocumentLock doclock = doc.LockDocument())
            {
                //操作过程
                var drawing = CadDrawing.Current;
                using (var tr = drawing.StartTrans())
                {
                    var brNames = CadDrawingUtils.GetBlockSubBlockNames(blockRef, tr);
                    if (brNames.Where(c => c.Contains("华东院")).FirstOrDefault() != null)
                    {
                        tr.Commit();
                        return true;
                    }
                    else
                    {
                        tr.Dispose();
                        return false;
                    }
                }
                //doclock.Dispose();
            };
        }

        // 用于不需要返回值的TaskCompletionSource提供的辅助结构
        private struct EmptyType { }
        /// <summary>
        /// 将一个任务放入CAD命令上下文中进行调用
        /// </summary>
        /// <param name="action">需要被CAD调用的任务</param>
        /// <param name="taskName"></param>
        /// <returns>Task封装的任务</returns>
        /// <exception cref="Exception">任务抛出的异常</exception>
        internal static Task ExecuteInCommandContextAsync(Action action)
        {
            var tcs = new TaskCompletionSource<EmptyType>();

            // 为了防止有可能从非CAD的线程中进行命令调用，使用以下方法将命令调用放入CAD的命令上下文中
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.ExecuteInCommandContextAsync(async (obj) =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    return;
                }
                tcs.SetResult(default);
            }, null);
            return tcs.Task;
        }
        /// <summary>
        /// 包装一个任务，在CAD命令上下文中进行异步调用
        /// </summary>
        /// <typeparam name="T">任务的返回类型</typeparam>
        /// <param name="function">需要被CAD调用的任务</param>
        /// <returns>Task封装的任务返回值</returns>
        /// <exception cref="Exception">任务抛出的异常</exception>
        internal static Task<T> ExecuteInCommandContextAsync<T>(Func<T> function)
        {
            var tcs = new TaskCompletionSource<T>();

            // 为了防止有可能从非CAD的线程中进行命令调用，使用以下方法将命令调用放入CAD的命令上下文中
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.ExecuteInCommandContextAsync(async (obj) =>
            {
                T result;
                try
                {
                    result = function();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    return;
                }
                tcs.SetResult(result);
            }, null);
            return tcs.Task;
        }
    }

    internal static class GeometryUtils
    {
        /// <summary>
        /// 获取图案
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<Entity> GetBlockGeometries(string name)
        {
            var list = new List<Entity>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction transaction = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = transaction.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                var btr = transaction.GetObject(bt[name], OpenMode.ForWrite) as BlockTableRecord;
                if (btr != null)
                {
                    foreach (var id in btr)
                    {
                        Entity ent = (Entity)transaction.GetObject(id, OpenMode.ForWrite);
                        if (ent != null) list.Add(ent);
                    }
                }
                transaction.Commit();
            }
            return list;
        }
        public static List<Entity> GetBlockGeometries(string name, Transaction tr)
        {
            var list = new List<Entity>();
            Database db = HostApplicationServices.WorkingDatabase;
            //using (Transaction transaction = db.TransactionManager.StartTransaction())
            //{
            BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
            var btr = tr.GetObject(bt[name], OpenMode.ForWrite) as BlockTableRecord;
            if (btr != null)
            {
                foreach (var id in btr)
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    if (ent != null) list.Add(ent);
                }
            }
            //transaction.Commit();
            //}
            return list;
        }

        public static void AddEntity(Transaction tr, Editor ed, Entity ent)
        {
            BlockTable bt = tr.GetObject(ed.Document.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            if (ent == null) return;
            btr.AppendEntity(ent);
            tr.AddNewlyCreatedDBObject(ent, true);

        }
        /// <summary>
        /// 中点
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="pe"></param>
        /// <returns></returns>
        public static List<Curve> GetEdg(Hatch hatch)
        {
            var curvs = new List<Curve>();
            try
            {
                var loop = hatch.GetLoopAt(0);
                if (loop != null)
                {
                    for (int i = 0; i < loop.Polyline.Count - 1; i++)
                    {
                        #region 填充边界
                        var obj1 = loop.Polyline[i];
                        var obj2 = loop.Polyline[i + 1];
                        var bv1 = obj1 as BulgeVertex;
                        var bv2 = obj2 as BulgeVertex;
                        var p1 = bv1.Vertex;
                        var p2 = bv2.Vertex;
                        var ps = new Point3d(p1.X, p1.Y, 0);
                        var pe = new Point3d(p1.X, p1.Y, 0);
                        curvs.Add(new Line(ps, pe));
                        //MessageBox.Show(obj.GetType().ToString());
                        #endregion
                    }
                    return curvs;
                }
                else
                    return curvs;
            }
            catch
            {
                return curvs;
            }
        }
        public static Point3d GetMid(Hatch hatch)
        {
            try
            {
                var curvs = GetEdg(hatch);
                if (curvs.Count == 0) return Point3d.Origin;
                var min = Point3d.Origin;
                var max = Point3d.Origin;
                var vec = GeometryUtils.GetCurveCorner(curvs.ToArray(), ref min, ref max, 10, 10);
                var mid = GetMid(min, max);
                return mid;
            }
            catch
            {
                //MessageBox.Show("没有填充边缘");
                return Point3d.Origin;

            }

            //var lines = hatch.GetHatchLinesData();
            ////MessageBox.Show(lines.Count.ToString());
            //var curvs = new List<Curve>();
            //foreach (var obj in lines)
            //{
            //    //MessageBox.Show(obj.GetType().ToString());
            //    var line = obj as Line2d;
            //    if (line != null)
            //    {
            //        var ps = new Point3d(line.StartPoint.X, line.StartPoint.Y, 0);
            //        var pe = new Point3d(line.EndPoint.X  , line.EndPoint.Y, 0);
            //        curvs.Add(new Line(ps, pe));
            //    }
            //}
            //var min = Point3d.Origin;
            //var max = Point3d.Origin;
            //var vec = GeometryUtils.GetCurveCorner(curvs.ToArray(),ref min, ref max,10,10);
            //var mid = GetMid(min,max);
            //var norml = hatch.Normal;
            //var mid = new Point3d(norml.X, norml.Y, 0);
            //return mid;

        }
        public static Point3d GetMid(Curve crv)
        {
            var min = Point3d.Origin;
            var max = Point3d.Origin;
            var vec = GetCurveCorner(crv, ref min, ref max);
            return GetMid(min, max);
        }
        public static Point3d GetMid(Point3d ps, Point3d pe)
        {
            var v3 = pe - ps;
            var v4 = v3 * 0.5;//Multiply
            var mid = ps + v4;
            return mid;
        }
        public static Point3d GetMid(Point3d ps, double length, double width)
        {
            var pe = ps + new Vector3d(length, width, 0);
            return GetMid(ps, pe);
        }
        public static Point3d GetMid(IList<Entity> objs)
        {
            var crvs = new List<Curve>();
            #region 线
            foreach (var obj in objs)
            {
                var crv = obj as Curve;
                if (crv == null) continue;
                crvs.Add(crv);
            }
            #endregion
            var min = Point3d.Origin;
            var max = Point3d.Origin;
            var vec = GetCurveCorner(crvs.ToArray(), ref min, ref max, 10, 10);
            var mid = GetMid(min, max);
            return mid;
        }
        public static Point3d GetMid(Entity obj)
        {
            var crv = obj as Curve;
            if (crv == null) return Point3d.Origin;
            return GetMid(crv);
        }

        /// <summary>
        /// 偏移线段
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="pe"></param>
        /// <param name="off"></param>
        /// <returns></returns>
        public static Point2d[] OffsetPoint(Point2d ps, Point2d pe, double off)
        {
            var p1Off = new Point2d(ps.X + off, ps.Y + off);
            var p2Off = new Point2d(pe.X + off, pe.Y + off);

            var p1 = new System.Numerics.Vector2((float)ps.X, (float)ps.Y);
            var p2 = new System.Numerics.Vector2((float)pe.X, (float)pe.Y);
            var vec = p2 - p1;
            if (vec.Length() < 1.0) return new Point2d[] { p1Off, p2Off };


            //var vec2 = RotateVector(vec, Math.PI / 2);
            var offset = System.Numerics.Vector2.Normalize(vec) * (float)off;
            var p1OffNew = p1 + offset;
            var p2OffNew = p2 + offset;
            p1Off = new Point2d(p1OffNew.X, p1OffNew.Y);
            p2Off = new Point2d(p2OffNew.X, p2OffNew.Y);
            return new Point2d[] { p1Off, p2Off };
        }
        private static System.Numerics.Vector2 RotateVector(Vector2d vector, double angle)
        {
            float x = (float)(vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle));
            float y = (float)(vector.X * Math.Sin(angle) + vector.Y * Math.Cos(angle));

            return new System.Numerics.Vector2(x, y);
        }

        /// <summary>
        /// 合并矩形框
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<Entity> JoinCurves(List<Entity> array)
        {
            var list = new List<Entity>();
            var pl = new Polyline();
            if (array.Count == 0) return list;
            for (int i = 0; i < array.Count; i++)
            {
                var item = array[i];
                var crv = item as Curve;
                if (crv == null) continue;
                var ps = crv.StartPoint;
                pl.AddVertexAt(i, new Point2d(ps.X, ps.Y), 0, 0, 0);
            }
            var first = array[0];
            var crv1 = first as Curve;
            if (crv1 == null) return list;
            var ps1 = crv1.StartPoint;
            pl.AddVertexAt(array.Count, new Point2d(ps1.X, ps1.Y), 0, 0, 0);
            pl.Closed = true;
            list.Add(pl);
            return list;
        }
        /// <summary>
        /// 矩形框
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Curve GetRectanceLine(Point3d min, Point3d max)
        {
            var list = GetRectance(min, max);
            var join = JoinCurves(list);
            if (join.Count == 0) return null;
            return join[0] as Curve;
        }
        public static List<Entity> GetRectance(Point3d min, Point3d max, double r)
        {
            var line1 = new Line(min + new Vector3d(0, -r, 0), max + new Vector3d(0, -r, 0));
            var line2 = new Line(min + new Vector3d(0, +r, 0), max + new Vector3d(0, +r, 0));
            return new List<Entity> { line1, line2 };
        }
        public static List<Entity> GetRectance(Point3d min, Point3d max)
        {
            var p0 = min; var length = Math.Abs(max.X - min.X); var width = Math.Abs(max.Y - min.Y);
            return GetRectance(p0, length, width);
        }
        public static List<Entity> GetRectance(Point3d p0, double length, double width)
        {
            var list = new List<Entity>();

            var l1 = new Line(p0 + new Vector3d(0, 0, 0), p0 + new Vector3d(length, 0, 0));
            var l2 = new Line(p0 + new Vector3d(length, 0, 0), p0 + new Vector3d(length, width, 0));
            var l3 = new Line(p0 + new Vector3d(length, width, 0), p0 + new Vector3d(0, width, 0));
            var l4 = new Line(p0 + new Vector3d(0, width, 0), p0 + new Vector3d(0, 0, 0));

            list.Add(l1);
            list.Add(l2);
            list.Add(l3);
            list.Add(l4);
            return list;
        }
        public static List<Entity> GetRectance(Point3d p0, double length, double width, string type = "门")
        {
            var list = new List<Entity>();

            var l1 = new Line(p0 + new Vector3d(0, 0, 0), p0 + new Vector3d(length, 0, 0));
            var l2 = new Line(p0 + new Vector3d(length, 0, 0), p0 + new Vector3d(length, width, 0));
            var l3 = new Line(p0 + new Vector3d(length, width, 0), p0 + new Vector3d(0, width, 0));
            var l4 = new Line(p0 + new Vector3d(0, width, 0), p0 + new Vector3d(0, 0, 0));

            if (type != "门")
                list.Add(l1);
            list.Add(l2);
            list.Add(l3);
            list.Add(l4);
            return list;
        }
        public static List<Entity> GetRectTrus(Point3d p0, double width, double heigth, double 顶筋, double 底筋, double 腹筋)
        {
            var list = new List<Entity>();
            var p1 = p0 + new Vector3d(0, heigth, 0);
            var p2 = p0 + new Vector3d(-width / 2, 0, 0);
            var p3 = p0 + new Vector3d(+width / 2, 0, 0);

            var c1 = GetCircle(p1, 顶筋 / 2); list.AddRange(c1);
            var c2 = GetCircle(p2, 底筋 / 2); list.AddRange(c2);
            var c3 = GetCircle(p3, 底筋 / 2); list.AddRange(c3);

            var l1 = GetRectance(p2 + new Vector3d(-底筋 / 2, 0, 0), 腹筋, (p1 - p2).Length);
            foreach (var item in l1) { Rota(item, p2, -Math.PI / 2 + GeometryUtils.GetAngle(new Line(p2, p1)), Vector3d.ZAxis); }
            list.AddRange(l1);
            var r1 = GetRectance(p3 + new Vector3d(-底筋 / 2, 0, 0), 腹筋, (p1 - p3).Length);
            foreach (var item in r1) { Rota(item, p3, -Math.PI / 2 + GeometryUtils.GetAngle(new Line(p3, p1)), Vector3d.ZAxis); }
            list.AddRange(r1);

            return list;
        }
        public static void Rota(Entity ent, Point3d rotatePt1, double angle, Vector3d axia)
        {
            var mt = Matrix3d.Rotation(angle, axia, rotatePt1);
            ent.TransformBy(mt);
        }
        public static void Rota(Point3d origin, Point3d rotatePt1, double angle, Vector3d axia)
        {
            var mt = Matrix3d.Rotation(angle, axia, rotatePt1);
            origin.TransformBy(mt);
        }
        public static double GetAngle(Curve line)
        {
            var dir = line.StartPoint.GetVectorTo(line.EndPoint);
            var length = dir.Length;
            dir = dir.DivideBy(length);
            var dir2d = dir.Convert2d(new Plane()).GetNormal(); //梁方向向量
            var angle = dir2d.Angle; //梁的角度
            return angle;
        }
        public static double GetAngle(LineSegment2d line)
        {
            var ps = new Point3d(line.StartPoint.X, line.StartPoint.Y, 0);
            var pe = new Point3d(line.EndPoint.X, line.EndPoint.Y, 0);
            return GetAngle(new Line(ps, pe));
        }
        public static double GetAngle(double angle)
        {
            return angle;
        }
        /// <summary>
        /// 圆形框
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static List<Entity> GetCircle(Point3d p0, double r)
        {
            var list = new List<Entity>();

            var c1 = new Circle(p0, Vector3d.ZAxis, r);
            list.Add(c1);

            return list;
        }


        /// <summary>
        /// 获取左下右上点
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string GetCurveCorner(List<Point3d> pts, ref Point3d min, ref Point3d max)
        {
            string v = "H";

            #region 多段线

            #region PolyCurve
            //var pts = GetPoints(pl);
            var maxX = 0.0; var maxY = 0.0;
            var minX = 0.0; var minY = 0.0;
            if (pts.Count == 0) return v;

            maxX = pts[0].X; maxY = pts[0].Y;
            minX = pts[0].X; minY = pts[0].Y;
            for (int i = 0; i < pts.Count; i++)
            {
                if (pts[i].X < minX) minX = pts[i].X;
                if (pts[i].Y < minY) minY = pts[i].Y;
                if (pts[i].X > maxX) maxX = pts[i].X;
                if (pts[i].Y > maxY) maxY = pts[i].Y;
            }
            min = new Point3d(minX, minY, 0);
            max = new Point3d(maxX, maxY, 0);

            return Math.Abs(max.X - min.X) > Math.Abs(max.Y - min.Y) ? "H" : "V";
            #endregion

            #endregion

        }
        public static string GetCurveCorner(Entity[] objs, ref Point3d min, ref Point3d max)
        {
            var retangs = new List<Curve>();
            foreach (var o in objs)
            {
                var crv = o as Curve;
                if (crv == null) continue;
                retangs.Add(crv);
            }
            return GetCurveCorner(retangs.ToArray(), ref min, ref max, 10, 10);
        }
        public static string GetCurveCorner(Curve retang, ref Point3d min, ref Point3d max)
        {
            string v = "H";

            var pl = retang as Polyline;

            #region 直线

            if (pl == null && retang != null)
            {
                //return "H";
                //v = Math.Abs(retang.PointAtStart.X - retang.PointAtEnd.X) > Math.Abs(retang.PointAtStart.Y - retang.PointAtEnd.Y) ? "H" : "V";
                //if (v == "H") min = retang.PointAtStart.X < retang.PointAtEnd.X ? retang.PointAtStart : retang.PointAtEnd;
                //if (v == "H") max = retang.PointAtStart.X > retang.PointAtEnd.X ? retang.PointAtStart : retang.PointAtEnd;
                //if (v == "V") min = retang.PointAtStart.Y < retang.PointAtEnd.Y ? retang.PointAtStart : retang.PointAtEnd;
                //if (v == "V") max = retang.PointAtStart.Y > retang.PointAtEnd.Y ? retang.PointAtStart : retang.PointAtEnd;
                var pts = new List<Point3d> { retang.StartPoint, retang.EndPoint };
                var maxX = 0.0; var maxY = 0.0;
                var minX = 0.0; var minY = 0.0;

                maxX = pts[0].X; maxY = pts[0].Y;
                minX = pts[0].X; minY = pts[0].Y;
                for (int i = 0; i < pts.Count; i++)
                {
                    if (pts[i].X < minX) minX = pts[i].X;
                    if (pts[i].Y < minY) minY = pts[i].Y;
                    if (pts[i].X > maxX) maxX = pts[i].X;
                    if (pts[i].Y > maxY) maxY = pts[i].Y;
                }
                min = new Point3d(minX, minY, 0);
                max = new Point3d(maxX, maxY, 0);

                return Math.Abs(max.X - min.X) > Math.Abs(max.Y - min.Y) ? "H" : "V";
            }
            #endregion

            #region 多段线

            if (pl != null)
            {
                #region PolyCurve
                var pts = GetPoints(pl);
                var maxX = 0.0; var maxY = 0.0;
                var minX = 0.0; var minY = 0.0;
                if (pts.Count == 0) return v;

                maxX = pts[0].X; maxY = pts[0].Y;
                minX = pts[0].X; minY = pts[0].Y;
                for (int i = 0; i < pts.Count; i++)
                {
                    if (pts[i].X < minX) minX = pts[i].X;
                    if (pts[i].Y < minY) minY = pts[i].Y;
                    if (pts[i].X > maxX) maxX = pts[i].X;
                    if (pts[i].Y > maxY) maxY = pts[i].Y;
                }
                min = new Point3d(minX, minY, 0);
                max = new Point3d(maxX, maxY, 0);

                var l = Math.Abs(max.X - min.X); //MessageBox.Show("l" + l.ToString("0"));
                var w = Math.Abs(max.Y - min.Y); //MessageBox.Show("w" + w.ToString("0"));
                return l > w ? "H" : "V";
                #endregion        
            }

            #endregion

            return v;
        }
        public static string GetCurveCorner(Curve retang1, Curve retang2, Curve retang3, Curve retang4, ref Point3d min, ref Point3d max)
        {
            var min1 = Point3d.Origin;
            var max1 = Point3d.Origin;
            var vec1 = GeometryUtils.GetCurveCorner(retang1, ref min1, ref max1);
            var min2 = Point3d.Origin;
            var max2 = Point3d.Origin;
            var vec2 = GeometryUtils.GetCurveCorner(retang2, ref min2, ref max2);
            var min3 = Point3d.Origin;
            var max3 = Point3d.Origin;
            var vec3 = GeometryUtils.GetCurveCorner(retang3, ref min3, ref max3);
            var min4 = Point3d.Origin;
            var max4 = Point3d.Origin;
            var vec4 = GeometryUtils.GetCurveCorner(retang4, ref min4, ref max4);
            min = new Point3d(max1.X, max3.Y, 0);
            max = new Point3d(min2.X, min4.Y, 0);
            return Math.Abs(max.X - min.X) > Math.Abs(max.Y - min.Y) ? "H" : "V";
        }
        public static string GetCurveCorner(Curve[] retangs, ref Point3d min, ref Point3d max, double tolMin, double tolMax)
        {
            var minX = 0.0;
            var maxX = 0.0;
            var minY = 0.0;
            var maxY = 0.0;

            var v = "H";
            var min1 = Point3d.Origin;
            var max1 = Point3d.Origin;
            foreach (var retange in retangs)
            {
                v = GetCurveCorner(retange, ref min1, ref max1);
                if (minX == Point3d.Origin.X &&
                    maxX == Point3d.Origin.X &&
                    minY == Point3d.Origin.Y &&
                    maxY == Point3d.Origin.Y)
                {
                    minX = min1.X; maxX = max1.X;
                    minY = min1.Y; maxY = max1.Y;
                }
                if (min1.X <= minX) minX = min1.X;
                if (max1.X >= maxX) maxX = max1.X;
                if (min1.Y <= minY) minY = min1.Y;
                if (max1.Y >= maxY) maxY = max1.Y;
            }
            min = new Point3d(minX, minY, 0);
            max = new Point3d(maxX, maxY, 0);
            v = Math.Abs(max.X - min.X) > Math.Abs(max.Y - min.Y) ? "H" : "V";
            return v;
        }
        public static string GetCurveCorner(Curve[] retangs, ref Point3d min, ref Point3d max, double tolMin, double tolMax, string v)
        {
            var minX = retangs[0].StartPoint.X;
            var maxX = retangs[0].StartPoint.X;
            var minY = retangs[0].StartPoint.Y;
            var maxY = retangs[0].StartPoint.Y;

            //var v = "H";
            //var min1 = Point3d.Origin;
            //var max1 = Point3d.Origin;
            foreach (var retange in retangs)
            {
                if (v == "H")
                {
                    var min1 = retange.StartPoint.X > retange.EndPoint.X ? retange.EndPoint : retange.StartPoint;
                    var max1 = retange.StartPoint.X < retange.EndPoint.X ? retange.EndPoint : retange.StartPoint;
                    if (min1.X <= minX) { minX = min1.X; minY = min1.Y; }
                    if (max1.X >= maxX) { maxX = max1.X; maxY = max1.Y; }
                }
                else
                {
                    var min1 = retange.StartPoint.Y > retange.EndPoint.Y ? retange.EndPoint : retange.StartPoint;
                    var max1 = retange.StartPoint.Y < retange.EndPoint.Y ? retange.EndPoint : retange.StartPoint;
                    if (min1.Y <= minY) { minX = min1.X; minY = min1.Y; }
                    if (max1.Y >= maxY) { maxX = max1.X; maxY = max1.Y; }
                }
            }
            min = new Point3d(minX, minY, 0);
            max = new Point3d(maxX, maxY, 0);
            //v = Math.Abs(max.X - min.X) > Math.Abs(max.Y - min.Y) ? "H" : "V";
            return v;
        }
        public static string GetCurveCorner(Point3d ps1, Point3d pe1, Point3d ps2, Point3d pe2, Point3d min1, Point3d max1, Point3d min2, Point3d max2,
                                                              ref Point3d min, ref Point3d max, ref double angle)
        {
            angle = 0;
            var tol = 1;

            var hLength1 = Math.Abs(max1.X - min1.X);
            var hLength2 = Math.Abs(max2.X - min2.X);

            var vLength1 = Math.Abs(max1.Y - min1.Y);
            var vLength2 = Math.Abs(max2.Y - min2.Y);

            if ((pe1 - ps1).Length - Math.Abs(max1.X - min1.X) < tol &&
                (pe2 - ps2).Length - Math.Abs(max2.X - min2.X) < tol)
            {
                #region 水平 0度
                //var v1 = GeometryUtils.GetCurveCorner("H", "H", min1, max1, min2, max2, ref min, ref max);
                var s1 = ps1.X < pe1.X ? ps1 : pe1;
                var e1 = ps1.X < pe1.X ? pe1 : ps1;
                var s2 = ps2.X < pe2.X ? ps2 : pe2;
                var e2 = ps2.X < pe2.X ? pe2 : ps2;
                var len1 = (e1 - s1).Length;
                var len2 = (e2 - s2).Length;
                var len = len1 > len2 ? len1 : len2;
                var crv1 = new Line(s1, e1);
                var mid = GeometryUtils.GetMid(s2, e2);
                var wid = crv1.ClosePtDIstanceTo(mid);
                //var wid = (s2 - s1).Length;
                min = s1.Y > s2.Y ? s2 : s1;
                max = min + new Vector3d(len, wid, 0);
                angle = 0;
                return "H";
                #endregion
            }
            else if ((pe1 - ps1).Length - Math.Abs(max1.Y - min1.Y) < tol &&
                     (pe2 - ps2).Length - Math.Abs(max2.Y - min2.Y) < tol)
            {
                #region 垂直 0度
                //var v1 = GeometryUtils.GetCurveCorner("V", "V", min1, max1, min2, max2, ref min, ref max);
                var s1 = ps1.Y < pe1.Y ? ps1 : pe1;
                var e1 = ps1.Y < pe1.Y ? pe1 : ps1;
                var s2 = ps2.Y < pe2.Y ? ps2 : pe2;
                var e2 = ps2.Y < pe2.Y ? pe2 : ps2;
                var len1 = (e1 - s1).Length;
                var len2 = (e2 - s2).Length;
                var len = len1 > len2 ? len1 : len2;
                var crv1 = new Line(s1, e1);
                var mid = GeometryUtils.GetMid(s2, e2);
                var wid = crv1.ClosePtDIstanceTo(mid);
                //var wid = (s2 - s1).Length;
                min = s1.X > s2.X ? s2 : s1;
                max = min + new Vector3d(wid, len, 0);
                angle = Math.PI / 2;
                return "V";
                #endregion
            }
            else
            {
                if (Math.Abs(pe1.X - ps1.X) > Math.Abs(pe1.Y - ps1.Y))/*&&   Math.Abs(pe2.X - ps2.X) > Math.Abs(pe2.Y - ps2.Y)*/
                {
                    #region 水平 倾斜
                    var s1 = ps1.X < pe1.X ? ps1 : pe1;
                    var e1 = ps1.X < pe1.X ? pe1 : ps1;
                    var s2 = ps2.X < pe2.X ? ps2 : pe2;
                    var e2 = ps2.X < pe2.X ? pe2 : ps2;
                    var len1 = (e1 - s1).Length;
                    var len2 = (e2 - s2).Length;
                    var len = len1 > len2 ? len1 : len2;
                    var v3 = e1 - s1;
                    var v0 = new Vector3d(1, 0, 0);
                    var crv1 = new Line(s1, e1);
                    var mid = GeometryUtils.GetMid(s2, e2);
                    var wid = crv1.ClosePtDIstanceTo(mid);
                    //var wid = (s2 - s1).Length;
                    min = s1.Y > s2.Y ? s2 : s1;
                    max = min + new Vector3d(len, wid, 0);
                    angle = GeometryUtils.VectorAngle(Vector3d.XAxis, v3); if (e1.Y < s1.Y) angle = 0 - angle;
                    var rot = GeometryUtils.ToAngle(angle);   //MessageBox.Show(rot.ToString());
                    return "H";
                    #endregion
                }
                else
                {
                    #region 垂直倾斜
                    var s1 = ps1.Y < pe1.Y ? ps1 : pe1;
                    var e1 = ps1.Y < pe1.Y ? pe1 : ps1;
                    var s2 = ps2.Y < pe2.Y ? ps2 : pe2;
                    var e2 = ps2.Y < pe2.Y ? pe2 : ps2;
                    var len1 = (e1 - s1).Length;
                    var len2 = (e2 - s2).Length;
                    var len = len1 > len2 ? len1 : len2;
                    var v3 = e1 - s1;
                    var v0 = new Vector3d(0, 1, 0);
                    var crv1 = new Line(s1, e1);
                    var mid = GeometryUtils.GetMid(s2, e2);
                    var wid = crv1.ClosePtDIstanceTo(mid);
                    //var wid = (s2 - s1).Length;
                    min = s1.X > s2.X ? s1 : s2;
                    max = min + new Vector3d(len, wid, 0);
                    angle = GeometryUtils.VectorAngle(Vector3d.XAxis, v3);/* if (e1.X < s1.X) angle = Math.PI - angle;*/
                    var rot = GeometryUtils.ToAngle(angle);   /*MessageBox.Show(rot.ToString());*/
                    return "V";
                    #region OLD
                    //var v1 = GeometryUtils.GetCurveCorner("V", "V", min1, max1, min2, max2, ref min, ref max);
                    //var v3 = s1 - e1;
                    //var v0 = new Vector3d(1, 0, 0);
                    //var crv1 = new Line(s1, e1);
                    //var mid = GeometryUtils.GetMid(s2, e2);
                    //var len = v3.Length;
                    //var wid = crv1.MinimumDistanceTo(mid);
                    //var minX = e1.X < e2.X ? e1.X : e2.X;
                    //var minY = e1.X < e2.X ? e1.Y : e2.Y;
                    //angle = EntityConvert.VectorAngle(Vector3d.XAxis, v3);
                    //min = new Point3d(minX, minY, 0);
                    //max = min + new Vector3d(len, wid, 0);
                    //if (e1.X < s1.X) angle = 0 - angle;
                    //if (e1.X > s1.X) angle = 0 - angle;
                    //return v1;
                    #endregion

                    #endregion
                }
            }
        }
        public static string GetCurveCorner(string v1, string v2, Point3d min1, Point3d max1, Point3d min2, Point3d max2,
                                                              ref Point3d min, ref Point3d max)
        {
            if (v1 != v2) return v1;
            if (v1 == "H" && v2 == "H")
            {
                min = min1.Y < min2.Y ? min1 : min2;
                max = max1.Y > max2.Y ? max1 : max2;
            }
            else if (v1 == "V" && v2 == "V")
            {
                min = min1.X < min2.X ? min1 : min2;
                max = max1.X > max2.X ? max1 : max2;
            }
            return v1;
        }
        public static string GetCurveCorner(string v1, string v2, Point3d min1, Point3d max1, Point3d min2, Point3d max2,
                                                              ref Point3d min, ref Point3d max, ref double tolMin, ref double tolMax)
        {
            if (v1 != v2) return v1;
            if (v1 == "V" && v2 == "V")
            {
                var y1 = min1.Y;
                var y2 = max1.Y;
                if (min2.X > min1.X)
                {
                    #region 左400右300
                    //=max1========max2
                    //=min1========min2
                    var x1 = min1.X;
                    var x2 = min2.X;
                    min = new Point3d(x1, y1, 0);
                    max = new Point3d(x2, y2, 0);
                    #endregion
                }
                else
                {
                    #region 左300右400
                    //=max2========max1
                    //=min2========min1
                    var x1 = min2.X;
                    var x2 = min1.X;
                    min = new Point3d(x1, y1, 0);
                    max = new Point3d(x2, y2, 0);
                    var temp = tolMax;
                    tolMax = tolMin;
                    tolMin = temp;
                    #endregion
                }
                //min = min1.Y < min2.Y ? min1 : min2;
                //max = max1.Y > max2.Y ? max1 : max2;
            }
            else if (v1 == "H" && v2 == "H")
            {
                var x1 = min1.X;
                var x2 = max1.X;
                if (min2.Y > min1.Y)
                {
                    #region 下400上300
                    var y1 = min1.Y;
                    var y2 = min2.Y;
                    min = new Point3d(x1, y1, 0);
                    max = new Point3d(x2, y2, 0);
                    #endregion
                }
                else
                {
                    #region 下300上400
                    var y1 = min2.Y;
                    var y2 = min1.Y;
                    min = new Point3d(x1, y1, 0);
                    max = new Point3d(x2, y2, 0);
                    var temp = tolMax;
                    tolMax = tolMin;
                    tolMin = temp;
                    #endregion
                }
                //min = min1.X < min2.X ? min1 : min2;
                //max = max1.X > max2.X ? max1 : max2;
            }
            return v1;
        }
        public static List<Point3d> GetPoints(Polyline pline)
        {
            List<Point3d> list = new List<Point3d>();
            for (int i = 0; i < pline.NumberOfVertices; i++)
            {
                try
                {
                    var line = pline.GetLineSegmentAt(i);
                    list.Add(line.StartPoint);
                    if ((line.EndPoint - pline.StartPoint).Length < 10)
                        list.Add(pline.StartPoint);
                }
                catch
                {
                    var line = pline.GetLineSegmentAt(i - 1);
                    list.Add(line.EndPoint);
                    if ((line.EndPoint - pline.StartPoint).Length < 10)
                        list.Add(pline.StartPoint);
                    if ((line.EndPoint - pline.EndPoint).Length < 10)
                        list.Add(pline.EndPoint);
                }
            }
            //list.Add(pline.StartPoint);
            return list;
        }


        /// <summary>
        /// 线长
        /// </summary>
        /// <param name="crv"></param>
        /// <returns></returns>
        public static double GetLength(this Curve crv)
        {
            var pl = crv as Polyline;
            if (pl != null) return pl.Length;
            var l = crv.StartPoint - crv.EndPoint;
            if (l.Length < 1) return 0;
            return l.Length;

        }
        /// <summary>
        /// 最小距离
        /// </summary>
        /// <param name="crv"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        public static double MinimumDistanceTo(this Curve crv, Point3d mid)
        {
            var ps = crv.StartPoint; var ls = (ps - mid).Length;
            var pe = crv.EndPoint; var le = (pe - mid).Length;
            if (ls > le)
            {
                return le;
            }
            else
            {
                return ls;
            }
        }
        public static double ClosePtDIstanceTo(this Curve crv, Point3d mid)
        {
            //return 300;
            try
            {
                var pt = crv.GetClosestPointTo(mid, true);
                var dist = (pt - mid).Length;
                //MessageBox.Show("1:" + pt.X.ToString("0") + "," + pt.Y.ToString("0"));
                //MessageBox.Show("2:" + mid.X.ToString("0") + "," + mid.Y.ToString("0"));
                //MessageBox.Show("宽:" + dist.ToString("0"));
                return dist;
            }
            catch
            {
                var ps = crv.StartPoint;
                var pe = crv.EndPoint;
                var pm = GeometryUtils.GetMid(ps, pe);
                var lm = (pm - mid).Length;
                return lm;

            }
        }
        /// <summary>
        /// 夹角
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double VectorAngle(Vector3d a, Vector3d b)
        {
            return a.GetAngleTo(b);
        }

        /// <summary>
        /// 转角度
        /// </summary>
        /// <param name="ra"></param>
        /// <returns></returns>
        public static double ToAngle(double ra)
        {
            return ra * 180 / Math.PI;
        }
        /// <summary>
        /// 转弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double FromAngle(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}
