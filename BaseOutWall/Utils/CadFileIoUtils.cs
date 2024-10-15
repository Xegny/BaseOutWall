using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Text;
using MgdDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using mgdApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Autodesk.AutoCAD.Geometry;
using System.Windows;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Forms;
using System.IO;

namespace BaseOutWall.Utils
{
    /// <summary>
    /// CAD 命令调用静态方法
    /// </summary>
    internal static class CadFileIoUtils
    {
        #region 图层Layer

        /// <summary>
        /// 默认图层 ，标注， 字体样式，线型
        /// </summary>
        public static void AddDefaultSetting()
        {
            var doc = mgdApplication.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;

            CadDrawing.Current.Start();
            if (CadDrawing.Current.IsActiveDrawing())
            {
                using (DocumentLock doclock = doc.LockDocument())
                {

                    using (var tr = CadDrawing.Current.StartTrans())
                    {

                        #region 默认图层

                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)1, "0S-WALL-LINE", false);
                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "0S-WALL-DIMS", false);
                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "0S-BEAM-TEXT", false);
                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)4, "0S-DETL-SYMB", false);
                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)5, "0S-DETL-RBAR", false);
                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "0S-DETL-SYMB", false);
                        CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)7, "0S-DETL-DIMS", false);
                        //CadFileIoUtils.CadDrawingUtils_AddLayer(CadDrawing.Current, (short)5, "", false); 
                        #endregion

                        #region 默认标注

                        CadFileIoUtils.CadDrawingUtils_AddDimStyle(CadDrawing.Current, (short)1, "dims-100", 300);

                        #endregion

                        #region 默认字体

                        #endregion

                        #region 默认线型

                        var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        var linetypeFile = Path.Combine(dir, "acad.lin"); // Hidden线型在acad.lin文件中定义
                        var linetypeName = "HIDDEN";
                        CadFileIoUtils.CadDrawingUtils_AddLineType(CadDrawing.Current, "acad.lin", linetypeName);


                        #endregion

                        tr.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// 目前发生在Update之前
        /// </summary>
        public static void AddDefaultLayer()
        {
            using (var tr = CadDrawing.Current.StartTrans())
            {
                //三维图层
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_楼层图框");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "数据中台_楼层编号");
                //三维图层
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)1, "数据中台_框柱三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "数据中台_主梁三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_次梁三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)4, "数据中台_楼板三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)5, "数据中台_楼梯三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "数据中台_内墙三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "数据中台_外墙三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)4, "数据中台_门洞三维");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_窗洞三维");
                //CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "数据中台_边缘构件");
                //二维图层
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_框柱平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "数据中台_主梁平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_次梁平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)4, "数据中台_楼板平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)5, "数据中台_楼梯平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "数据中台_内墙平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)6, "数据中台_外墙平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)4, "数据中台_门洞平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_窗洞平面");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)11, "数据中台_门洞标注");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_窗洞标注");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "数据中台_边缘构件");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)1, "数据中台_轴网");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_轴网编号");
                CadDrawingUtils_AddLayer(CadDrawing.Current, (short)3, "数据中台_轴网标注");

                tr.Commit();
            }
        }
        /// <summary>
        /// 添加定位点临时图层
        /// </summary>
        public static void AddIDropTempLayer()
        {
            //三维图层
            CadDrawingUtils_AddLayer(CadDrawing.Current, (short)2, "iDrop_temp", false);
        }
        public static void AddIDropTempLayer(Document doc)
        {
            try
            {
                //var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                using (DocumentLock doclock = doc.LockDocument())
                {
                    CadDrawing.Current.Start();
                    if (CadDrawing.Current.IsActiveDrawing())
                    {
                        using (var tr = CadDrawing.Current.StartTrans())
                        {
                            CadFileIoUtils.AddIDropTempLayer();
                            CadDrawing.Current.CommitTrans();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 获取当前图层
        /// </summary>
        /// <returns></returns>
        public static ObjectId GetCurrentLayer(Document doc)
        {
            var cLayerId = ObjectId.Null;
            try
            {
                //var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                using (DocumentLock doclock = doc.LockDocument())
                {
                    CadDrawing.Current.Start();
                    if (CadDrawing.Current.IsActiveDrawing())
                    {
                        using (var tr = CadDrawing.Current.StartTrans())
                        {
                            //CadFileIoUtils.AddIDropTempLayer();
                            cLayerId = GetCurrentLayerTableRecords(CadDrawing.Current);
                            CadDrawing.Current.CommitTrans();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.Message + ex.StackTrace);
            }
            return cLayerId;
        }
        /// <summary>
        /// 切换当前图层
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="clayer"></param>
        public static void SetCurrentLayer(Document doc, ObjectId clayer)
        {
            try
            {
                //var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                using (DocumentLock doclock = doc.LockDocument())
                {
                    CadDrawing.Current.Start();
                    if (CadDrawing.Current.IsActiveDrawing())
                    {
                        using (var tr = CadDrawing.Current.StartTrans())
                        {
                            //CadFileIoUtils.AddIDropTempLayer();
                            SetCurrentLayer(CadDrawing.Current, clayer);
                            CadDrawing.Current.CommitTrans();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        public static ObjectId AddpendEntity(Document doc, Entity ent)
        {
            var newid = ObjectId.Null;

            try
            {
                //var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                using (DocumentLock doclock = doc.LockDocument())
                {
                    CadDrawing.Current.Start();
                    if (CadDrawing.Current.IsActiveDrawing())
                    {
                        using (var tr = CadDrawing.Current.StartTrans())
                        {
                            newid = ((BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false)).AppendEntity(ent);
                            tr.AddNewlyCreatedDBObject(ent, true);
                            CadDrawing.Current.CommitTrans();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.Message + ex.StackTrace);
            }
            return newid;
        }
        /// <summary>
        /// 添加一个实体点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pt"></param>
        public static ObjectId AddIDropPointEntity(Document doc, Point3d pt)
        {
            var ptE = new DBPoint(pt);
            if (ptE != null)
            {
                var id = AddpendEntity(doc, ptE);
                return id;
            }
            return ObjectId.Null;
        }
        /// <summary>
        /// 选中Rhino 对象
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="guid"></param>
        public static void SelectEntity(Document doc, ObjectId id)
        {
            try
            {
                //var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                using (DocumentLock doclock = doc.LockDocument())
                {
                    CadDrawing.Current.Start();
                    if (CadDrawing.Current.IsActiveDrawing())
                    {
                        using (var tr = CadDrawing.Current.StartTrans())
                        {
                            //CadFileIoUtils.AddIDropTempLayer();
                            var ent = (Entity)CadDrawing.Current.Cad_Trans.GetObject(id, OpenMode.ForWrite);
                            //ed.SelectLast();
                            ent.Highlight();
                            ed.SetImpliedSelection(new ObjectId[] { ent.Id });
                            CadDrawing.Current.CommitTrans();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 添加默认图层
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="colorIndex"></param>
        /// <param name="layerName"></param>
        public static void CadDrawingUtils_AddLayer(CadDrawing drawing, short colorIndex, string layerName, bool isPrint = true)
        {

            short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256 using 

            var layerTable = GetLayerTables(drawing); //层表记录
            if (!layerTable.Has(layerName))
            {
                LayerTableRecord ltr = new LayerTableRecord();
                ltr.Name = layerName;
                ltr.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                if (!isPrint) ltr.IsPlottable = false;
                var layerId = layerTable.Add(ltr);
                drawing.Cad_Trans.AddNewlyCreatedDBObject(ltr, true);
                SetCurrentLayer(drawing, ltr);
            }
            else
            {
                LayerTableRecord ltr = drawing.Cad_Trans.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                if (ltr != null)
                    SetCurrentLayer(drawing, ltr);
            }


        }
        /// <summary>
        /// 设置当前层
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="layer"></param>
        public static void SetCurrentLayer(CadDrawing drawing, LayerTableRecord layer)
        {
            if (layer.ObjectId != ObjectId.Null) drawing.Cad_Database.Clayer = layer.ObjectId;
        }
        public static void SetCurrentLayer(CadDrawing drawing, ObjectId layerObjectId)
        {
            if (layerObjectId != ObjectId.Null) drawing.Cad_Database.Clayer = layerObjectId;
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
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 获取层表记录（读写）
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
        /// 获取当前默认图层
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static ObjectId GetCurrentLayerTableRecords(CadDrawing drawing)
        {
            if (drawing.Cad_Database == null) return ObjectId.Null;
            if (drawing.Cad_Trans == null) return ObjectId.Null;
            //ObjectId LayerTableId = drawing.Cad_Database.LayerTableId;
            //var Cad_LayerTable = (LayerTable)drawing.Cad_Trans.GetObject(LayerTableId, OpenMode.ForWrite);
            var cLayerId = drawing.Cad_Database.Clayer;
            return cLayerId;
        }

        /// <summary>
        ///  获取线表记录（读写）
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static LinetypeTable GetLineTypeTable(CadDrawing drawing)
        {
            if (drawing.Cad_Database == null) return null;
            if (drawing.Cad_Trans == null) return null;
            ObjectId LineTypeTableId = drawing.Cad_Database.LinetypeTableId;
            var Cad_LineTypeTable = (LinetypeTable)drawing.Cad_Trans.GetObject(LineTypeTableId, OpenMode.ForWrite);
            return Cad_LineTypeTable;
        }

        /// <summary>
        /// 获取当前默认线型
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static ObjectId GetCurrentLineTypeTableRecords(CadDrawing drawing)
        {
            if (drawing.Cad_Database == null) return ObjectId.Null;
            if (drawing.Cad_Trans == null) return ObjectId.Null;
            //ObjectId LineTypeTableId = drawing.Cad_Database.LinetypeTableId;
            //var Cad_LineTypeTable = (LinetypeTable)drawing.Cad_Trans.GetObject(LineTypeTableId, OpenMode.ForWrite);
            var cLayerId = drawing.Cad_Database.ContinuousLinetype;
            return cLayerId;
        }


        #endregion

        #region 线型LineType

        public static void CadDrawingUtils_AddLineType(CadDrawing drawing, string linetypeFile, string linetypeName)
        {
            var db = drawing.Cad_Database;
            var tr = drawing.Cad_Trans;
            var lt = GetLineTypeTables(drawing);

            if (!lt.Has("HIDDEN"))
            {
                if (System.IO.File.Exists(linetypeFile))
                {
                    db.LoadLineTypeFile(linetypeName, linetypeFile);
                }
                else
                {
                    //Application.ShowAlertDialog("Linetype file not found.");
                }
            }
        }

        public static LinetypeTable GetLineTypeTables(CadDrawing drawing)
        {
            var db = drawing.Cad_Database;
            var tr = drawing.Cad_Trans;
            LinetypeTable lt = tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite) as LinetypeTable;
            return lt;
        }

        #endregion

        #region 字体Style

        #endregion

        #region 标注DimStyle

        public static void CadDrawingUtils_AddDimStyle(CadDrawing drawing, short colorIndex, string layerName, double textHeight)
        {
            short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256 using 

            var dimStyleTable = GetDimStyleTable(drawing); //层表记录
            if (!dimStyleTable.Has(layerName))
            {
                var dstr = new DimStyleTableRecord();
                //dstr.Color = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                dstr.Name = layerName;
                // 设置尺寸样式的属性
                dstr.Dimtxt = textHeight;// 2.5; // 文本高度
                dstr.Dimasz = 2.5; // 箭头大小
                dstr.Dimtsz = 1.0; // 标准字体的大小
                dstr.Dimscale = 1.0;
                var newDimStyleId = dimStyleTable.Add(dstr);
                drawing.Cad_Trans.AddNewlyCreatedDBObject(dstr, true);
                //SetCurrentDimStyle(drawing, dstr);
            }
            else
            {
                //DimStyleTableRecord dstr = drawing.Cad_Trans.GetObject(dimStyleTable[layerName], OpenMode.ForWrite) as DimStyleTableRecord;
                //if (dstr != null)
                //    SetCurrentDimStyle(drawing, dstr);
            }
        }

        /// <summary>
        /// 获取标注样式表记录（读写）
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static DimStyleTable GetDimStyleTable(CadDrawing drawing)
        {
            if (drawing.Cad_Database == null) return null;
            if (drawing.Cad_Trans == null) return null;
            ObjectId DimStyleTableId = drawing.Cad_Database.DimStyleTableId;
            var Cad_DimStyleTable = (DimStyleTable)drawing.Cad_Trans.GetObject(DimStyleTableId, OpenMode.ForWrite);
            return Cad_DimStyleTable;
        }

        /// <summary>
        /// 设置当前层
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="layer"></param>
        public static void SetCurrentDimStyle(CadDrawing drawing, DimStyleTableRecord dimstyle)
        {
            //if (dimstyle.ObjectId != ObjectId.Null) drawing.Cad_Database.Dimstyle Dimstyle = dimstyle.ObjectId;
        }
        public static void SetCurrentDimStyle(CadDrawing drawing, ObjectId layerObjectId)
        {
            if (layerObjectId != ObjectId.Null) drawing.Cad_Database.Dimstyle = layerObjectId;
        }
        public static void SetCurrentDimStyle(CadDrawing drawing, short colorIndex, string layerName)
        {
            try
            {
                short colorIndex1 = (short)(colorIndex % 256);//防止输入的颜色超出256 using 
                //层表记录
                var dimstyleTable = GetDimStyleTable(drawing);
                if (!dimstyleTable.Has(layerName))
                {

                }
                else
                {
                    //DimStyleTableRecord ltr = drawing.Cad_Trans.GetObject(dimstyleTable[layerName], OpenMode.ForWrite) as DimStyleTableRecord;
                    //SetCurrentDimStyle(drawing, ltr);
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        #region 词典Dic

        /// <summary>
        /// 将当前词典序列化到DWG文件
        /// </summary>
        /// <param name="dict"></param>
        internal static void WriteDicToDwg(Dictionary<string, string> dict)
        {
            var doc = mgdApplication.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // 命名对象字典
                DBDictionary nod = tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;

                if (nod == null)
                {
                    tr.Dispose();
                    return;
                }

                foreach (var key in dict.Keys)
                {
                    try
                    {
                        var val = dict[key];
                        // 自定义数据
                        Xrecord myXrecord = new Xrecord();
                        myXrecord.Data = new ResultBuffer(new TypedValue((int)DxfCode.Text, val));
                        // 往命名对象字典中存储自定义数据
                        var has = nod.GetAt(key);
                        //if (has == null)
                        //{
                        //    ed.WriteMessage(key + "添加值" + val + "\r\n");
                        //}
                        //else
                        //{
                        //    ed.WriteMessage(key + "更新值" + val + "\r\n");
                        //}
                        nod.SetAt(key, myXrecord);
                        tr.AddNewlyCreatedDBObject(myXrecord, true);
                    }
                    catch
                    { }


                }



                tr.Commit();
            }
        }

        /// <summary>
        /// 将当前DWG文件反序列化到词典
        /// </summary>
        internal static void ReadDicFromDwg()
        {
            var doc = mgdApplication.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // 命名对象字典
                DBDictionary nod = tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;

                if (nod == null)
                {
                    tr.Dispose();
                    return;
                }
                foreach (var item in nod)
                {
                    try
                    {
                        #region 读取词典
                        var key = item.Key;
                        //查找自定义数据
                        if (nod.Contains(key))
                        {
                            ObjectId myDataId = nod.GetAt(key);
                            Xrecord myXrecord = tr.GetObject(myDataId, OpenMode.ForRead) as Xrecord;
                            if (myXrecord == null) continue;
                            //foreach (TypedValue tv in myXrecord.Data)
                            //{
                            //    doc.Editor.WriteMessage("type: {0},key: {1}, value: {2}\n", tv.TypeCode, key, tv.Value + "\r\n");
                            //}
                        }
                        // doc.Editor.WriteMessage(item.Key + "-" + item.Value + "\r\n");
                        #endregion
                    }
                    catch { }
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// 获取字典索引
        /// </summary>
        /// <param name="ddic"></param>
        /// <returns></returns>
        internal static string[] AllKeys(this DBDictionary ddic)
        {
            var list = new List<string>();
            foreach (var item in ddic)
            {
                list.Add(item.Key);
            }
            return list.ToArray();
        }

        #endregion
    }

    public static class EntityConvert
    {
        public static Curve Geometry(this Entity ent)
        {
            if (ent.GetType() == typeof(BlockReference))
            {
                #region 构件外框线
                var br = ent as BlockReference;
                if (br == null) return null;
                var geo = br.GeometryExtentsBestFit();
                if (geo == null) return null;
                var po = br.Position;
                var min = geo.MinPoint;
                var max = geo.MaxPoint;
                var crv = GeometryUtils.GetRectanceLine(min, max);
                return crv;
                #endregion
            }
            else
            {
                #region 底图外框线
                try
                {
                    var crv = ent as Curve;
                    if (crv != null) return crv;
                    else return crv;
                }
                catch
                {
                    return null;
                }
                #endregion
            }
        }
        public static Entity GeometryE(this Entity ent)
        {
            try
            {
                var crv = ent as Curve;
                if (crv != null) return crv;
                else return crv;
            }
            catch
            {
                return ent;
            }
        }

        public static Dictionary<string, string[]> GetAllUserStrings(this Entity ent)
        {
            var dic = new Dictionary<string, string[]>();
            if (ent.XData == null) return dic;

            foreach (var rb in ent.XData)
            {
                var key = rb.Value.ToString();
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, new string[] { });
                }
            }
            return dic;
        }
        public static void SetUserString(this Entity ent, string key, string value)
        {
            try
            {
                var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                var tr = CadDrawing.Current.Cad_Trans;
                var tv = new TypedValue[] { new TypedValue(1001, key), new TypedValue(1000, value) };
                SetXData(tr, db, ent, key, tv);
            }
            catch { }
        }
        public static string GetUserString(this Entity ent, string key)
        {
            try
            {
                var doc = mgdApplication.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;
                var db = doc.Database;
                var tr = CadDrawing.Current.Cad_Trans;

                TypedValue[] tv;
                var has = TryGetXData(tr, db, ent.Id, key, out tv);
                return tv[1].Value.ToString();
            }
            catch
            {
                return string.Empty;
            }

        }
        /// <summary>
        /// 给指定的实体增加扩展数据 根据 DBObject
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="obj">ent等</param>
        /// <param name="regAppName">扩展数据名</param>
        /// <param name="tv">扩展数据</param>
        public static void SetXData(Transaction tr, Database db, DBObject obj, string regAppName, TypedValue[] tv)
        {
            //using (Transaction tr = db.TransactionManager.StartTransaction())
            //{
            RegAppTable rat = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForWrite);

            using (ResultBuffer rb = new ResultBuffer(tv))
            {
                obj.UpgradeOpen();
                CheckAddAppName(tr, rat, regAppName); //注册并添加扩展数据名
                obj.XData = rb;
                rb.Dispose();
                //tr.Commit();
            }
            //}
        }
        /// <summary>
        /// 注册并添加扩展数据名
        /// </summary>
        /// <param name="regAppName"></param>
        public static void CheckAddAppName(Transaction tr, RegAppTable rat, string regAppName)
        {
            if (!rat.Has(regAppName))
            {
                RegAppTableRecord ratr = new RegAppTableRecord();
                ratr.Name = regAppName;
                rat.Add(ratr);
                tr.AddNewlyCreatedDBObject(ratr, true);
            }

        }
        /// <summary>
        /// 得到扩展数据
        /// </summary>
        /// <param name="db">db</param>
        /// <param name="entId">实体Id</param>
        /// <param name="regAppName">扩展数据名</param>
        /// <param name="tv">扩展数据</param>
        /// <returns>true,false</returns>
        public static bool TryGetXData(Transaction tr, Database db, ObjectId entId, string regAppName, out TypedValue[] tv)
        {
            //Transaction tr = db.TransactionManager.StartTransaction();
            //using (tr)
            //{
            DBObject obj = tr.GetObject(entId, OpenMode.ForRead);
            using (ResultBuffer rb = obj.GetXDataForApplication(regAppName))
            {
                if (rb != null)
                {
                    tv = rb.AsArray();
                    return true;
                }
                else
                {
                    tv = null;
                    return false;
                }
            }
            //}
        }
        /// <summary>
        /// 可写入块属性
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool CanWriteBlock(this PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(string)) return true;
            if (pi.PropertyType == typeof(double)) return true;
            if (pi.PropertyType == typeof(int)) return true;
            return false;
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Entity Find(this Document doc, ObjectId id)
        {
            try
            {
                var tr = CadDrawing.Current.Cad_Trans;
                var ent = (Entity)tr.GetObject(id, OpenMode.ForRead, true);
                return ent;
            }
            catch { return null; }
        }
        public static void Delete(this Document doc, ObjectId id, bool isWrite)
        {
            try
            {
                var tr = CadDrawing.Current.Cad_Trans;
                var ent = (Entity)tr.GetObject(id, OpenMode.ForWrite, true);
                ent.Erase();
            }
            catch { }
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
        ///// <summary>
        ///// 最小距离
        ///// </summary>
        ///// <param name="crv"></param>
        ///// <param name="mid"></param>
        ///// <returns></returns>
        //public static double MinimumDistanceTo(this Curve crv, Point3d mid)
        //{
        //    var ps = crv.StartPoint; var ls = (ps - mid).Length;
        //    var pe = crv.EndPoint; var le = (pe - mid).Length;
        //    if (ls > le)
        //    {
        //        return le;
        //    }
        //    else
        //    {
        //        return ls;
        //    }
        //}
        //public static double ClosePtDIstanceTo(this Curve crv, Point3d mid)
        //{
        //    //return 300;
        //    try
        //    {
        //        var pt = crv.GetClosestPointTo(mid, true);
        //        var dist = (pt - mid).Length;
        //        //MessageBox.Show("1:" + pt.X.ToString("0") + "," + pt.Y.ToString("0"));
        //        //MessageBox.Show("2:" + mid.X.ToString("0") + "," + mid.Y.ToString("0"));
        //        //MessageBox.Show("宽:" + dist.ToString("0"));
        //        return dist;
        //    }
        //    catch
        //    {
        //        var ps = crv.StartPoint;
        //        var pe = crv.EndPoint;
        //        var pm = GeometryUtils.GetMid(ps, pe);
        //        var lm = (pm - mid).Length;
        //        return lm;

        //    }
        //}
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
        /// 添加线
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="crv"></param>
        /// <returns></returns>
        public static ObjectId Objects_AddCurve(this Document doc, Curve crv)
        {
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = CadDrawing.Current.Cad_Trans;

            try
            {
                var bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                var btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                if (crv == null) return ObjectId.Null;
                btr.AppendEntity(crv);
                tr.AddNewlyCreatedDBObject(crv, true);

                return crv.Id;
            }
            catch
            {
                return ObjectId.Null;
            }
        }

        //public static double VectorAngle(Vector3d axia, Vector3d v3)
        //{
        //    return a.GetAngleTo(b);
        //}
    }
}
