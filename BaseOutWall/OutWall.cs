using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;
using Autodesk.AutoCAD.GraphicsInterface;
using static System.Net.Mime.MediaTypeNames;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;
using System.Runtime.CompilerServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace BaseOutWall
{
    public class OutWall
    {
        #region 构造函数
        public OutWall() { }
        #endregion

        #region 类的属性

        #endregion

        #region 不同楼层的画法不同（已废除）
        public static List<Line> LessThan4Story(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            double totalHeight = storyList.Sum(story => double.Parse(story.StoryHeight));
            double banhou1 = double.Parse(storyList[0].SlabT);
            Line inwallline1 = new Line(
                point + new Vector3d(neiqiangkuan, dibanhou + dianceng, 0),
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - banhou1, 0));
            inwallline1.Layer = "0S-WALL-LINE";
            inwallline.Add(inwallline1);
            return inwallline;
        }
        public static List<Line> WhenFourStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            double totalHeight = storyList.Sum(story => double.Parse(story.StoryHeight));
            double banhou1 = double.Parse(storyList[0].SlabT);
            double xiaqianggao = double.Parse(storyList[3].StoryHeight);
            Line inwallline1 = new Line(
                point + new Vector3d(neiqiangkuan, xiaqianggao + dibanhou + dianceng, 0),
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - banhou1, 0));
            Line inwallline2 = new Line(
                point + new Vector3d(xianeiqiangkuan, dibanhou + dianceng, 0),
                point + new Vector3d(xianeiqiangkuan, xiaqianggao + dibanhou + dianceng, 0));
            Line inwallline3 = new Line(
                point + new Vector3d(neiqiangkuan, xiaqianggao + dibanhou + dianceng, 0),
                point + new Vector3d(xianeiqiangkuan, xiaqianggao + dibanhou + dianceng, 0));
            inwallline1.Layer = "0S-WALL-LINE";
            inwallline2.Layer = "0S-WALL-LINE";
            inwallline3.Layer = "0S-WALL-LINE";
            inwallline.Add(inwallline1);
            inwallline.Add(inwallline2);
            inwallline.Add(inwallline3);
            return inwallline;
        }
        public static List<Line> WhenFiveStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            return inwallline;
        }
        public static List<Line> WhenSixStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            return inwallline;
        }
        public static List<Line> WhenSevenStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            return inwallline;
        }
        public static List<Line> WhenEightStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            return inwallline;
        }
        public static List<Line> WhenNineStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            return inwallline;
        }
        public static List<Line> WhenTenStory(Point3d point, double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan, double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            List<Line> inwallline = new List<Line>();
            return inwallline;
        }
        #endregion

        #region 画折断线的函数
        public static Polyline ZheDuanXian(Point3d point1, Point3d point2)
        {
            Polyline breakLine = new Polyline();
            Vector2d vector2D = new Vector2d(point1.X, point1.Y);
            breakLine.AddVertexAt(0, new Point2d(0 - 100, 0) + vector2D, 0, 0, 0);
            breakLine.AddVertexAt(1, new Point2d(0 + 275, 0) + vector2D, 0, 0, 0);
            breakLine.AddVertexAt(2, new Point2d(0 + 300, -65) + vector2D, 0, 0, 0);
            breakLine.AddVertexAt(3, new Point2d(0 + 350, 65) + vector2D, 0, 0, 0);
            breakLine.AddVertexAt(4, new Point2d(0 + 375, 0) + vector2D, 0, 0, 0);
            breakLine.AddVertexAt(5, new Point2d(0 + 750, 0) + vector2D, 0, 0, 0);
            breakLine.Closed = false;
            double rotationAngle = new Line(point1, point2).Angle;
            double scaleFactor = new Line(point1, point2).Length / 650;
            // 获取多段线的插入点
            Point3d insert = new Point3d(point1.X + 100, point1.Y, 0);
            Point3d center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, 0);
            // 创建旋转变换矩阵
            Matrix3d rotationMatrix = Matrix3d.Rotation(rotationAngle, Vector3d.ZAxis, point1);
            // 应用旋转变换矩阵到多段线对象
            breakLine.TransformBy(rotationMatrix);
            // 创建缩放变换矩阵
            Matrix3d scaleMatrix = Matrix3d.Scaling(scaleFactor, point1);
            // 应用缩放变换矩阵到多段线对象
            breakLine.TransformBy(scaleMatrix);
            breakLine.Layer = "0S-DETL-SYMB";
            return breakLine;
        }
        #endregion

        #region 给文字加框的函数
        public static Polyline DrawTextKuang(MText mtext, double distance)
        {
            Point3d centerPoint;
            switch (mtext.Attachment)
            {
                case AttachmentPoint.TopLeft:
                    centerPoint = mtext.Location + new Vector3d(mtext.ActualWidth / 2, -mtext.ActualHeight / 2, 0);
                    break;
                case AttachmentPoint.TopCenter:
                    centerPoint = mtext.Location + new Vector3d(0, -mtext.ActualHeight / 2, 0);
                    break;
                case AttachmentPoint.TopRight:
                    centerPoint = mtext.Location + new Vector3d(-mtext.ActualWidth / 2, -mtext.ActualHeight / 2, 0);
                    break;
                case AttachmentPoint.MiddleLeft:
                    centerPoint = mtext.Location + new Vector3d(mtext.ActualWidth / 2, 0, 0);
                    break;
                case AttachmentPoint.MiddleCenter:
                    centerPoint = mtext.Location;
                    break;
                case AttachmentPoint.MiddleRight:
                    centerPoint = mtext.Location + new Vector3d(0, 0, 0);
                    break;
                case AttachmentPoint.BottomLeft:
                    centerPoint = mtext.Location + new Vector3d(mtext.ActualWidth / 2, mtext.ActualHeight / 2, 0);
                    break;
                case AttachmentPoint.BottomCenter:
                    centerPoint = mtext.Location + new Vector3d(0, mtext.ActualHeight / 2, 0);
                    break;
                case AttachmentPoint.BottomRight:
                    centerPoint = mtext.Location + new Vector3d(-mtext.ActualWidth / 2, mtext.ActualHeight / 2, 0);
                    break;
                default:
                    centerPoint = mtext.Location;
                    break;
            }

            // 获取 MText 对象的位置和大小
            Point3d position = centerPoint;
            double width = mtext.ActualWidth;
            double height = mtext.ActualHeight;

            // 根据距离调整边界框
            double adjustedWidth = width / 2 + distance;
            double adjustedHeight = height / 2 + distance;

            // 计算边界框的左下右上角点坐标
            Point3d minPoint = new Point3d(position.X - adjustedWidth, position.Y - adjustedHeight, position.Z);
            Point3d maxPoint = new Point3d(position.X + adjustedWidth, position.Y + adjustedHeight, position.Z);

            // 创建 Polyline 对象来表示框的边界
            Polyline polyline = new Polyline();
            polyline.Closed = true;
            polyline.AddVertexAt(0, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
            polyline.AddVertexAt(1, new Point2d(maxPoint.X, minPoint.Y), 0, 0, 0);
            polyline.AddVertexAt(2, new Point2d(maxPoint.X, maxPoint.Y), 0, 0, 0);
            polyline.AddVertexAt(3, new Point2d(minPoint.X, maxPoint.Y), 0, 0, 0);
            return polyline;
        }
        #endregion

        #region 画对齐标注
        public static void DrawBiaoZhu(Database db, Point3d startPoint, Point3d endPoint, double offsetdistance)
        {
            //标注

            Point3d midPoint = startPoint + ((endPoint - startPoint) / 2);
            Vector3d offset = (endPoint - startPoint).GetPerpendicularVector() * offsetdistance; // 调整标注位置的偏移量
            string distanceString = startPoint.DistanceTo(endPoint).ToString("0");
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //生成标注
                DimStyleTable dimStyleTable = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                ObjectId dimensionStyle = dimStyleTable["dims-100"];
                AlignedDimension diancengThickness = new AlignedDimension(startPoint, endPoint, midPoint + offset, distanceString, dimensionStyle);
                diancengThickness.Layer = "0S-WALL-DIMS";
                //画进去
                btr.AppendEntity(diancengThickness);
                tr.AddNewlyCreatedDBObject(diancengThickness, true);
                tr.Commit();
            }


        }
        #endregion

        #region 画两边是圆角的箍筋
        public static Polyline DrawHoopWithHalfCircle(Point3d point1, Point3d point2, double diameter, double hoopdiameter)
        {

            // 计算point1和point2之间的向量
            Vector3d vector2to1 = point2 - point1;
            // 计算第一个四等分点
            Point3d midpoint2 = point1 + vector2to1 * 0.25;
            // 计算第三个四等分点
            Point3d midpoint3 = point1 + vector2to1 * 0.75;
            double r = (diameter + hoopdiameter) / 2;
            double distance = point1.DistanceTo(point2) / 2;
            Circle circle1 = new Circle(point1, Vector3d.ZAxis, r);
            Circle circle2 = new Circle(midpoint2, Vector3d.ZAxis, distance / 2);
            Circle circle3 = new Circle(midpoint3, Vector3d.ZAxis, distance / 2);
            Circle circle4 = new Circle(point2, Vector3d.ZAxis, r);
            // 计算交点p1
            Point3dCollection intersectionPoints1 = new Point3dCollection();
            circle1.IntersectWith(circle2, Intersect.OnBothOperands, intersectionPoints1, IntPtr.Zero, IntPtr.Zero);
            // 按x坐标排序
            Point3d jiaodian1 = intersectionPoints1[0].X < intersectionPoints1[1].X ? intersectionPoints1[0] : intersectionPoints1[1];
            Point3d jiaodian2 = intersectionPoints1[0].X < intersectionPoints1[1].X ? intersectionPoints1[1] : intersectionPoints1[0];

            // 计算交点p2
            Point3dCollection intersectionPoints2 = new Point3dCollection();
            circle3.IntersectWith(circle4, Intersect.OnBothOperands, intersectionPoints2, IntPtr.Zero, IntPtr.Zero);
            // 按x坐标排序
            Point3d jiaodian3 = intersectionPoints2[0].X < intersectionPoints2[1].X ? intersectionPoints2[0] : intersectionPoints2[1];
            Point3d jiaodian4 = intersectionPoints2[0].X < intersectionPoints2[1].X ? intersectionPoints2[1] : intersectionPoints2[0];

            // 创建Polyline对象
            Vector3d vector = point1 - jiaodian2;
            Point3d symmetricPoint1 = point1 + vector;
            Point3d symmetricPoint2 = point2 - vector;
            Vector3d vector2 = jiaodian3 - jiaodian2;
            Point3d exsymmetric1 = symmetricPoint1 + vector2 / 8;
            Point3d exsymmetric2 = symmetricPoint2 - vector2 / 8;

            // 添加起点和终点到Polyline的顶点集合中
            //Point2d polylinepoint1 = new Point2d(jiaodian2.X, jiaodian2.Y);
            Polyline polyline = new Polyline();
            polyline.AddVertexAt(0, new Point2d(exsymmetric1.X, exsymmetric1.Y), 0, 0, 0);
            polyline.AddVertexAt(1, new Point2d(symmetricPoint1.X, symmetricPoint1.Y), -1, 0, 0);
            polyline.AddVertexAt(2, new Point2d(jiaodian2.X, jiaodian2.Y), 0, 0, 0);
            polyline.AddVertexAt(3, new Point2d(jiaodian3.X, jiaodian3.Y), 1, 0, 0);
            polyline.AddVertexAt(4, new Point2d(symmetricPoint2.X, symmetricPoint2.Y), 0, 0, 0);
            polyline.AddVertexAt(5, new Point2d(exsymmetric2.X, exsymmetric2.Y), 0, 0, 0);
            polyline.Closed = false;
            polyline.ConstantWidth = hoopdiameter;
            return polyline;
        }
        #endregion

        #region 找哪两层中间的墙宽变化了
        public static List<int> FindChangedWallThicknessIndices(List<Story> storylist)
        {
            List<int> changedIndices = new List<int>();

            for (int i = 0; i < storylist.Count - 1; i++)
            {
                Story currentStory = storylist[i];
                Story nextStory = storylist[i + 1];

                if (currentStory.WallThickness != nextStory.WallThickness)
                {
                    changedIndices.Add(i);
                }
            }

            return changedIndices;//i和i+1层
        }
        #endregion

        #region 画图函数，主函数
        public static void CreateOutWall
            (double dianceng, double dingbiaogao, double neiqiangkuan, double xianeiqiangkuan,
            double waiqiangkuan, double dibanhou, List<Story> storyList)
        {
            #region 预操作
            DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            List<int> changedIndices = OutWall.FindChangedWallThicknessIndices(storyList);//i和i+1层
            #endregion

            #region 用户选择点为基础点
            PromptPointResult pointResult = editor.GetPoint("选择一个点：");
            if (pointResult.Status != PromptStatus.OK)
                return;

            Point3d point = pointResult.Value;
            #endregion

            #region 画外墙
            // 使用 LINQ 求和 StoryHeight 属性
            double totalHeight = storyList.Sum(story => double.Parse(story.StoryHeight));
            //1
            Line outwallline1 = new Line(
                point + new Vector3d(0, -300, 0),
                point + new Vector3d(0, totalHeight + dibanhou + dianceng, 0));
            outwallline1.Layer = "0S-WALL-LINE";
            //2外
            Line outwallline2 = new Line(
                point + new Vector3d(-waiqiangkuan, -300, 0),
                point + new Vector3d(-waiqiangkuan, totalHeight + dibanhou + dianceng, 0));
            outwallline2.Layer = "0S-WALL-LINE";
            //zdx
            Polyline waiqiangzdx = ZheDuanXian(point + new Vector3d(-waiqiangkuan, -300, 0),
                point + new Vector3d(0, -300, 0));
            //3
            Polyline thickoutwallline = new Polyline();
            thickoutwallline.AddVertexAt(0, new Point2d(point.X, point.Y - 150), 0, 0, 0);
            thickoutwallline.AddVertexAt(1, new Point2d(point.X, point.Y + totalHeight + dibanhou + dianceng), 0, 0, 0);
            thickoutwallline.ConstantWidth = 15;
            thickoutwallline.Layer = "0S-WALL-LINE";
            //标注
            Point3d waiqiangbiaozhudian1 = point + new Vector3d(0, totalHeight + dibanhou + dianceng, 0);
            Point3d waiqiangbiaozhudian2 = point + new Vector3d(-waiqiangkuan, totalHeight + dibanhou + dianceng, 0);
            OutWall.DrawBiaoZhu(db, waiqiangbiaozhudian1, waiqiangbiaozhudian2, -200);

            #endregion

            #region 旁边的标注(totalHeight)
            double dicenggao = Double.Parse(storyList[storyList.Count - 1].StoryHeight);
            double dingcenggao = Double.Parse(storyList[0].StoryHeight);
            double distance = 50;
            //"室内"            
            MText mtextSN = new MText();//数字
            mtextSN.SetDatabaseDefaults();
            mtextSN.Contents = "室内";
            mtextSN.TextHeight = 250;
            mtextSN.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
            mtextSN.Location = point + new Vector3d(neiqiangkuan * 1.5 + 900 + 1000 + 300, totalHeight * 2 / 5, 0);
            mtextSN.Layer = "0S-DETL-DIMS";
            Polyline shineikuang = DrawTextKuang(mtextSN, distance);
            shineikuang.Layer = "0S-DETL-DIMS";
            //"室外"
            MText mtextSW = new MText();//数字
            mtextSW.SetDatabaseDefaults();
            mtextSW.Contents = "室外";
            mtextSW.TextHeight = 250;
            mtextSW.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
            mtextSW.Location = point + new Vector3d(-waiqiangkuan * 2 - 300, totalHeight * 2 / 5, 0);
            mtextSW.Layer = "0S-DETL-DIMS";
            Polyline shiwaikuang = DrawTextKuang(mtextSW, distance);//画个框
            shiwaikuang.Layer = "0S-DETL-DIMS";
            //"地下连续墙"
            MText mtextDXLXQ = new MText();//数字
            mtextDXLXQ.SetDatabaseDefaults();
            mtextDXLXQ.Contents = "地下连续墙";
            mtextDXLXQ.TextHeight = 250;
            mtextDXLXQ.Attachment = AttachmentPoint.BottomRight;// 设置文本对象的插入点为文本的顶端中点
            mtextDXLXQ.Location = point + new Vector3d(-waiqiangkuan * 1.05, totalHeight * 4 / 7, 0);
            mtextDXLXQ.Layer = "0S-BEAM-TEXT";
            Line lineDXLXQ = new Line(point + new Vector3d(-waiqiangkuan - 960, totalHeight * 4 / 7, 0)//下划线
                , point + new Vector3d(-waiqiangkuan * 7 / 8, totalHeight * 4 / 7, 0));
            lineDXLXQ.Layer = "0S-BEAM-TEXT";
            #endregion

            #region 画内墙
            double banhou1 = double.Parse(storyList[0].SlabT);
            //顶端虚线
            Line inwallline2 = new Line(
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - banhou1, 0),
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng, 0));
            inwallline2.Linetype = "HIDDEN";
            inwallline2.Layer = "0S-WALL-LINE";
            //顶层墙
            Line dingcengwall = new Line(
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - banhou1, 0),
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - dingcenggao, 0));
            dingcengwall.Layer = "0S-WALL-LINE";
            //水平配筋
            Point3d di_midshuipingpeijin1 = point + new Vector3d(40 + 5 + 20, totalHeight + dibanhou + dianceng - dingcenggao / 2, 0);
            Point3d di_midshuipingpeijin2 = point + new Vector3d(neiqiangkuan - (40 + 5 + 20), totalHeight + dibanhou + dianceng - dingcenggao / 2, 0);
            Point3d di_upshuipingpeijin1 = point + new Vector3d(40 + 5 + 20, totalHeight + dibanhou + dianceng - dingcenggao / 2 + 200, 0);
            Point3d di_upshuipingpeijin2 = point + new Vector3d(neiqiangkuan - (40 + 5 + 20), totalHeight + dibanhou + dianceng - dingcenggao / 2 + 200, 0);
            Point3d di_downshuipingpeijin1 = point + new Vector3d(40 + 5 + 20, totalHeight + dibanhou + dianceng - dingcenggao / 2 - 200, 0);
            Point3d di_downshuipingpeijin2 = point + new Vector3d(neiqiangkuan - (40 + 5 + 20), totalHeight + dibanhou + dianceng - dingcenggao / 2 - 200, 0);
            //
            Polyline di_midcircle1 = new Polyline();
            di_midcircle1.AddVertexAt(0, new Point2d(di_midshuipingpeijin1.X - 10, di_midshuipingpeijin1.Y), 1, 0, 0);
            di_midcircle1.AddVertexAt(1, new Point2d(di_midshuipingpeijin1.X + 10, di_midshuipingpeijin1.Y), 1, 0, 0);
            di_midcircle1.Closed = true;
            di_midcircle1.ConstantWidth = 20;
            di_midcircle1.Layer = "0S-DETL-RBAR";
            //
            Polyline di_midcircle2 = new Polyline();
            di_midcircle2.AddVertexAt(0, new Point2d(di_midshuipingpeijin2.X - 10, di_midshuipingpeijin2.Y), 1, 0, 0);
            di_midcircle2.AddVertexAt(1, new Point2d(di_midshuipingpeijin2.X + 10, di_midshuipingpeijin2.Y), 1, 0, 0);
            di_midcircle2.Closed = true;
            di_midcircle2.ConstantWidth = 20;
            di_midcircle2.Layer = "0S-DETL-RBAR";
            //
            Polyline di_upcircle1 = new Polyline();
            di_upcircle1.AddVertexAt(0, new Point2d(di_upshuipingpeijin1.X - 10, di_upshuipingpeijin1.Y), 1, 0, 0);
            di_upcircle1.AddVertexAt(1, new Point2d(di_upshuipingpeijin1.X + 10, di_upshuipingpeijin1.Y), 1, 0, 0);
            di_upcircle1.Closed = true;
            di_upcircle1.ConstantWidth = 20;
            di_upcircle1.Layer = "0S-DETL-RBAR";
            //
            Polyline di_upcircle2 = new Polyline();
            di_upcircle2.AddVertexAt(0, new Point2d(di_upshuipingpeijin2.X - 10, di_upshuipingpeijin2.Y), 1, 0, 0);
            di_upcircle2.AddVertexAt(1, new Point2d(di_upshuipingpeijin2.X + 10, di_upshuipingpeijin2.Y), 1, 0, 0);
            di_upcircle2.Closed = true;
            di_upcircle2.ConstantWidth = 20;
            di_upcircle2.Layer = "0S-DETL-RBAR";
            //
            Polyline di_downcircle1 = new Polyline();
            di_downcircle1.AddVertexAt(0, new Point2d(di_downshuipingpeijin1.X - 10, di_downshuipingpeijin1.Y), 1, 0, 0);
            di_downcircle1.AddVertexAt(1, new Point2d(di_downshuipingpeijin1.X + 10, di_downshuipingpeijin1.Y), 1, 0, 0);
            di_downcircle1.Closed = true;
            di_downcircle1.ConstantWidth = 20;
            di_downcircle1.Layer = "0S-DETL-RBAR";
            //
            Polyline di_downcircle2 = new Polyline();
            di_downcircle2.AddVertexAt(0, new Point2d(di_downshuipingpeijin2.X - 10, di_downshuipingpeijin2.Y), 1, 0, 0);
            di_downcircle2.AddVertexAt(1, new Point2d(di_downshuipingpeijin2.X + 10, di_downshuipingpeijin2.Y), 1, 0, 0);
            di_downcircle2.Closed = true;
            di_downcircle2.ConstantWidth = 20;
            di_downcircle2.Layer = "0S-DETL-RBAR";
            //
            Polyline di_xiegujin = OutWall.DrawHoopWithHalfCircle(di_midshuipingpeijin1, di_midshuipingpeijin2, 40, 20);
            di_xiegujin.Layer = "0S-DETL-RBAR";
            //标注
            Point3d dingcengneiqiangbiaozhu1 = point + new Vector3d(0, totalHeight + dibanhou + dianceng, 0);
            Point3d dingcengneiqiangbiaozhu2 = point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng, 0);
            OutWall.DrawBiaoZhu(db, dingcengneiqiangbiaozhu1, dingcengneiqiangbiaozhu2, 200);
            #endregion

            #region 画中间层内墙
            //每层的内墙线
            List<Line> insidewall = new List<Line>();
            List<Polyline> neiqiangpeijin = new List<Polyline>();
            for (int i = 1; i < storyList.Count; i++)
            {
                //参数
                double danyuanqiangkuan = Double.Parse(storyList[i].WallThickness);
                double qiangjiaogaodu = totalHeight + dianceng + dibanhou - (dingbiaogao * 1000 - Double.Parse(storyList[i].StoryElevation) * 1000);
                double danyuancenggao = Double.Parse(storyList[i].StoryHeight);
                //内墙线
                Line neiqiangxian = new Line(
                    point + new Vector3d(danyuanqiangkuan, qiangjiaogaodu + danyuancenggao, 0),
                    point + new Vector3d(danyuanqiangkuan, qiangjiaogaodu, 0));
                neiqiangxian.Layer = "0S-WALL-LINE";
                insidewall.Add(neiqiangxian);
                //转换连接线
                if (storyList[i - 1].WallThickness != storyList[i].WallThickness)
                {
                    double qianyiceng = Double.Parse(storyList[i - 1].WallThickness);
                    Line zhuanhuanxian = new Line(
                        point + new Vector3d(qianyiceng, qiangjiaogaodu + Double.Parse(storyList[i].StoryHeight), 0),
                        point + new Vector3d(danyuanqiangkuan, qiangjiaogaodu + Double.Parse(storyList[i].StoryHeight), 0));
                    zhuanhuanxian.Layer = "0S-WALL-LINE";
                    insidewall.Add(zhuanhuanxian);
                    //标注
                    Point3d neiqiangbiaozhu1 = point + new Vector3d(0, qiangjiaogaodu + Double.Parse(storyList[i].StoryHeight), 0);
                    Point3d neiqiangbiaozhu2 = point + new Vector3d(danyuanqiangkuan, qiangjiaogaodu + Double.Parse(storyList[i].StoryHeight), 0);
                    OutWall.DrawBiaoZhu(db, neiqiangbiaozhu1, neiqiangbiaozhu2, -300);
                }
                //水平配筋
                Point3d midshuipingpeijin1 = point + new Vector3d(40 + 5 + 20, qiangjiaogaodu + danyuancenggao / 2, 0);
                Point3d midshuipingpeijin2 = point + new Vector3d(danyuanqiangkuan - (40 + 5 + 20), qiangjiaogaodu + danyuancenggao / 2, 0);
                Point3d upshuipingpeijin1 = point + new Vector3d(40 + 5 + 20, qiangjiaogaodu + danyuancenggao / 2 + 200, 0);
                Point3d upshuipingpeijin2 = point + new Vector3d(danyuanqiangkuan - (40 + 5 + 20), qiangjiaogaodu + danyuancenggao / 2 + 200, 0);
                Point3d downshuipingpeijin1 = point + new Vector3d(40 + 5 + 20, qiangjiaogaodu + danyuancenggao / 2 - 200, 0);
                Point3d downshuipingpeijin2 = point + new Vector3d(danyuanqiangkuan - (40 + 5 + 20), qiangjiaogaodu + danyuancenggao / 2 - 200, 0);
                //
                Polyline midcircle1 = new Polyline();
                midcircle1.AddVertexAt(0, new Point2d(midshuipingpeijin1.X - 10, midshuipingpeijin1.Y), 1, 0, 0);
                midcircle1.AddVertexAt(1, new Point2d(midshuipingpeijin1.X + 10, midshuipingpeijin1.Y), 1, 0, 0);
                midcircle1.Closed = true;
                midcircle1.ConstantWidth = 20;
                neiqiangpeijin.Add(midcircle1);
                //
                Polyline midcircle2 = new Polyline();
                midcircle2.AddVertexAt(0, new Point2d(midshuipingpeijin2.X - 10, midshuipingpeijin2.Y), 1, 0, 0);
                midcircle2.AddVertexAt(1, new Point2d(midshuipingpeijin2.X + 10, midshuipingpeijin2.Y), 1, 0, 0);
                midcircle2.Closed = true;
                midcircle2.ConstantWidth = 20;
                neiqiangpeijin.Add(midcircle2);
                //
                Polyline upcircle1 = new Polyline();
                upcircle1.AddVertexAt(0, new Point2d(upshuipingpeijin1.X - 10, upshuipingpeijin1.Y), 1, 0, 0);
                upcircle1.AddVertexAt(1, new Point2d(upshuipingpeijin1.X + 10, upshuipingpeijin1.Y), 1, 0, 0);
                upcircle1.Closed = true;
                upcircle1.ConstantWidth = 20;
                neiqiangpeijin.Add(upcircle1);
                //
                Polyline upcircle2 = new Polyline();
                upcircle2.AddVertexAt(0, new Point2d(upshuipingpeijin2.X - 10, upshuipingpeijin2.Y), 1, 0, 0);
                upcircle2.AddVertexAt(1, new Point2d(upshuipingpeijin2.X + 10, upshuipingpeijin2.Y), 1, 0, 0);
                upcircle2.Closed = true;
                upcircle2.ConstantWidth = 20;
                neiqiangpeijin.Add(upcircle2);
                //
                Polyline downcircle1 = new Polyline();
                downcircle1.AddVertexAt(0, new Point2d(downshuipingpeijin1.X - 10, downshuipingpeijin1.Y), 1, 0, 0);
                downcircle1.AddVertexAt(1, new Point2d(downshuipingpeijin1.X + 10, downshuipingpeijin1.Y), 1, 0, 0);
                downcircle1.Closed = true;
                downcircle1.ConstantWidth = 20;
                neiqiangpeijin.Add(downcircle1);
                //
                Polyline downcircle2 = new Polyline();
                downcircle2.AddVertexAt(0, new Point2d(downshuipingpeijin2.X - 10, downshuipingpeijin2.Y), 1, 0, 0);
                downcircle2.AddVertexAt(1, new Point2d(downshuipingpeijin2.X + 10, downshuipingpeijin2.Y), 1, 0, 0);
                downcircle2.Closed = true;
                downcircle2.ConstantWidth = 20;
                neiqiangpeijin.Add(downcircle2);
                //
                Polyline midxiegujin = OutWall.DrawHoopWithHalfCircle(midshuipingpeijin1, midshuipingpeijin2, 40, 20);
                neiqiangpeijin.Add(midxiegujin);
            }

            #endregion

            #region 底板
            double neiqiangkuan0 = xianeiqiangkuan;
            Line diban = new Line(
                point + new Vector3d(neiqiangkuan0, dibanhou + dianceng, 0),
                point + new Vector3d(neiqiangkuan0 + 900, dibanhou + dianceng, 0));//标高标志插入点
            diban.Layer = "0S-WALL-LINE";
            //标注
            Point3d dibanbiaozhudian1 = point + new Vector3d(neiqiangkuan0 + 900, dibanhou + dianceng, 0);
            Point3d dibanbiaozhudian2 = point + new Vector3d(neiqiangkuan0 + 900, dianceng, 0);
            OutWall.DrawBiaoZhu(db, dibanbiaozhudian1, dibanbiaozhudian2, 500);
            //"底板"
            MText mtextDB = new MText();//数字
            mtextDB.SetDatabaseDefaults();
            mtextDB.Contents = "底板";
            mtextDB.TextHeight = 200;
            mtextDB.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
            mtextDB.Location = point + new Vector3d(neiqiangkuan0 + 600, (dibanhou + dianceng) * 0.67, 0);
            mtextDB.Layer = "0S-BEAM-TEXT";
            //"LaE"
            MText mtextLaE = new MText();//数字
            mtextLaE.SetDatabaseDefaults();
            mtextLaE.Contents = "LaE";
            mtextLaE.TextHeight = 150;
            mtextLaE.Attachment = AttachmentPoint.BottomCenter;// 设置文本对象的插入点为文本的顶端中点
            mtextLaE.Location = point + new Vector3d((neiqiangkuan + 900) / 2 - 100, dianceng + 100, 0);
            mtextLaE.Layer = "0S-BEAM-TEXT";
            //折断线
            Polyline dibanzdx = ZheDuanXian(point + new Vector3d(neiqiangkuan0 + 900, 0, 0),
                point + new Vector3d(neiqiangkuan0 + 900, dibanhou + dianceng, 0));
            #region 画标高          
            Point3d pointbioagao1 = point + new Vector3d(neiqiangkuan0 + 900 + 300, dibanhou + dianceng, 0);
            //线
            Line biaogaodixian1 = new Line(pointbioagao1 + new Vector3d(-200, 0, 0), pointbioagao1 + new Vector3d(200, 0, 0));
            Line biaogaoshangxian1 = new Line(pointbioagao1 + new Vector3d(-200, 200, 0), pointbioagao1 + new Vector3d(1000, 200, 0));
            Line biaogaozuoxiexian1 = new Line(pointbioagao1, pointbioagao1 + new Vector3d(200, 200, 0));
            Line biaogaoyouxiexian1 = new Line(pointbioagao1, pointbioagao1 + new Vector3d(-200, 200, 0));
            biaogaodixian1.Layer = "0S-BEAM-TEXT";
            biaogaoshangxian1.Layer = "0S-BEAM-TEXT";
            biaogaozuoxiexian1.Layer = "0S-BEAM-TEXT";
            biaogaoyouxiexian1.Layer = "0S-BEAM-TEXT";
            //文字
            MText mtext1 = new MText();//数字
            mtext1.SetDatabaseDefaults();
            mtext1.Contents = storyList[storyList.Count - 1].StoryElevation + "(相对标高)";
            mtext1.TextHeight = 250;
            mtext1.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
            mtext1.Location = pointbioagao1 + new Vector3d(600, 400, 0);
            mtext1.Layer = "0S-BEAM-TEXT";
            MText mtext11 = new MText();//层号
            mtext11.SetDatabaseDefaults();
            mtext11.Contents = "B" + storyList.Count.ToString();
            mtext11.TextHeight = 250;
            mtext11.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
            mtext11.Location = pointbioagao1 + new Vector3d(600, 700, 0);
            mtext11.Layer = "0S-BEAM-TEXT";
            #endregion

            #endregion

            #region 顶板
            Line dingbanup = new Line(
                point + new Vector3d(-waiqiangkuan, totalHeight + dibanhou + dianceng, 0),
                point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng, 0));//标高标志插入点
            dingbanup.Layer = "0S-WALL-LINE";
            //标注
            Point3d dingbanbiaozhu1 = point + new Vector3d(neiqiangkuan + 600, totalHeight + dibanhou + dianceng, 0);
            Point3d dingbanbiaozhu2 = point + new Vector3d(neiqiangkuan + 600, totalHeight + dibanhou + dianceng - banhou1, 0);
            OutWall.DrawBiaoZhu(db, dingbanbiaozhu1, dingbanbiaozhu2, 0);

            #region 画标高          
            Point3d pointbioagao2 = point + new Vector3d(neiqiangkuan + 900 + 300, totalHeight + dibanhou + dianceng, 0);
            //线
            Line biaogaodixian2 = new Line(pointbioagao2 + new Vector3d(-200, 0, 0), pointbioagao2 + new Vector3d(200, 0, 0));
            Line biaogaoshangxian2 = new Line(pointbioagao2 + new Vector3d(-200, 200, 0), pointbioagao2 + new Vector3d(1000, 200, 0));
            Line biaogaozuoxiexian2 = new Line(pointbioagao2, pointbioagao2 + new Vector3d(200, 200, 0));
            Line biaogaoyouxiexian2 = new Line(pointbioagao2, pointbioagao2 + new Vector3d(-200, 200, 0));
            biaogaodixian2.Layer = "0S-BEAM-TEXT";
            biaogaoshangxian2.Layer = "0S-BEAM-TEXT";
            biaogaozuoxiexian2.Layer = "0S-BEAM-TEXT";
            biaogaoyouxiexian2.Layer = "0S-BEAM-TEXT";
            //文字
            MText mtext2 = new MText();
            mtext2.SetDatabaseDefaults();
            mtext2.Contents = "L1";
            mtext2.TextHeight = 250;
            mtext2.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
            mtext2.Location = pointbioagao2 + new Vector3d(600, 400, 0);
            mtext2.Layer = "0S-BEAM-TEXT";

            #endregion

            Line dingbandown = new Line(
                point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - banhou1, 0),
                point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng - banhou1, 0));
            dingbandown.Layer = "0S-WALL-LINE";
            //折断线
            Polyline dingbanzdx = ZheDuanXian(point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng - banhou1, 0),
                point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng, 0));
            #endregion

            #region 画中间的楼板
            List<Line> uploubanlist = new List<Line>();
            List<Line> downloubanlist = new List<Line>();
            List<Line> biaogaolist = new List<Line>();
            List<MText> biaogaotextlist = new List<MText>();
            List<Polyline> zhongjianzdx = new List<Polyline>();
            List<Polyline> fujiajin = new List<Polyline>();
            for (int i = 1; i < storyList.Count; i++)
            {
                double dingduanjuli = Math.Abs(double.Parse(storyList[i - 1].StoryElevation) * 1000 - dingbiaogao);
                double banhou = double.Parse(storyList[i].SlabT);

                neiqiangkuan = Double.Parse(storyList[i].WallThickness);
                //楼板的上边那条线
                Line uplouban = new Line(
                    point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - dingduanjuli, 0),
                    point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng - dingduanjuli, 0));
                uplouban.Layer = "0S-WALL-LINE";
                uploubanlist.Add(uplouban);
                #region 画标高          
                Point3d pointbioagao = point + new Vector3d(neiqiangkuan + 900 + 300, totalHeight + dibanhou + dianceng - dingduanjuli, 0);
                //线
                Line biaogaodixian = new Line(pointbioagao + new Vector3d(-200, 0, 0), pointbioagao + new Vector3d(200, 0, 0));
                Line biaogaoshangxian = new Line(pointbioagao + new Vector3d(-200, 200, 0), pointbioagao + new Vector3d(1000, 200, 0));
                Line biaogaozuoxiexian = new Line(pointbioagao, pointbioagao + new Vector3d(200, 200, 0));
                Line biaogaoyouxiexian = new Line(pointbioagao, pointbioagao + new Vector3d(-200, 200, 0));
                biaogaodixian.Layer = "0S-BEAM-TEXT";
                biaogaoshangxian.Layer = "0S-BEAM-TEXT";
                biaogaozuoxiexian.Layer = "0S-BEAM-TEXT";
                biaogaoyouxiexian.Layer = "0S-BEAM-TEXT";
                biaogaolist.Add(biaogaodixian); biaogaolist.Add(biaogaoshangxian); biaogaolist.Add(biaogaozuoxiexian); biaogaolist.Add(biaogaoyouxiexian);
                //文字
                MText mtext = new MText();//数字
                mtext.SetDatabaseDefaults();
                mtext.Contents = "B" + i.ToString();
                mtext.TextHeight = 250;
                mtext.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
                mtext.Location = pointbioagao + new Vector3d(600, 700, 0);
                mtext.Layer = "0S-BEAM-TEXT";
                MText mtext0 = new MText();//文字
                mtext0.SetDatabaseDefaults();
                mtext0.Contents = storyList[i - 1].StoryElevation;
                mtext0.TextHeight = 250;
                mtext0.Attachment = AttachmentPoint.MiddleCenter;// 设置文本对象的插入点为文本的顶端中点
                mtext0.Location = pointbioagao + new Vector3d(600, 400, 0);
                mtext0.Layer = "0S-BEAM-TEXT";
                biaogaotextlist.Add(mtext);
                biaogaotextlist.Add(mtext0);
                #endregion
                //楼板的下边那条线
                Line downlouban = new Line(
                    point + new Vector3d(neiqiangkuan, totalHeight + dibanhou + dianceng - dingduanjuli - banhou, 0),
                    point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng - dingduanjuli - banhou, 0));
                downlouban.Layer = "0S-WALL-LINE";
                downloubanlist.Add(downlouban);
                //板厚标注
                Point3d zhongjianloubanbiaozhu1 = point + new Vector3d(neiqiangkuan + 450, totalHeight + dibanhou + dianceng - dingduanjuli, 0);
                Point3d zhongjianloubanbiaozhu2 = point + new Vector3d(neiqiangkuan + 450, totalHeight + dibanhou + dianceng - dingduanjuli - banhou, 0);
                OutWall.DrawBiaoZhu(db, zhongjianloubanbiaozhu1, zhongjianloubanbiaozhu2, 0);
                //折断线
                Polyline zdx = ZheDuanXian(point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng - dingduanjuli - banhou, 0),
                            point + new Vector3d(neiqiangkuan + 900, totalHeight + dibanhou + dianceng - dingduanjuli, 0));
                zhongjianzdx.Add(zdx);
                //楼板处的附加筋
                double shangmaogu = Double.Parse(storyList[i - 1].StoryHeight) / 3;
                double xiamaogu = Double.Parse(storyList[i].StoryHeight) / 3;
                Vector2d vector2Dtemp = new Vector2d(point.X, point.Y);
                Polyline fujia = new Polyline();
                fujia.AddVertexAt(0, new Point2d(40 + 30 + 60, totalHeight + dibanhou + dianceng - dingduanjuli - banhou - xiamaogu + 60) + vector2Dtemp, 0, 0, 0);
                fujia.AddVertexAt(1, new Point2d(40 + 30, totalHeight + dibanhou + dianceng - dingduanjuli - banhou - xiamaogu) + vector2Dtemp, 0, 0, 0);
                fujia.AddVertexAt(2, new Point2d(40 + 30, totalHeight + dibanhou + dianceng - dingduanjuli + shangmaogu) + vector2Dtemp, 0, 0, 0);
                fujia.AddVertexAt(3, new Point2d(40 + 30 + 60, totalHeight + dibanhou + dianceng - dingduanjuli + shangmaogu - 60) + vector2Dtemp, 0, 0, 0);
                fujia.Closed = false;
                fujia.Layer = "0S-DETL-RBAR";
                fujia.ConstantWidth = 10;
                fujiajin.Add(fujia);
                //附加筋标注
                Point3d fujiajinbiaozhu1 = point + new Vector3d(40 + 30, totalHeight + dibanhou + dianceng - dingduanjuli - banhou - xiamaogu, 0);
                Point3d fujiajinbiaozhu2 = point + new Vector3d(40 + 30, totalHeight + dibanhou + dianceng - dingduanjuli + shangmaogu, 0);
                OutWall.DrawBiaoZhu(db, fujiajinbiaozhu1, fujiajinbiaozhu2, 500);
            }
            #endregion

            #region 画素砼垫层
            Line diancengup = new Line(
                point + new Vector3d(0, dianceng, 0),
                point + new Vector3d(neiqiangkuan + 900, dianceng, 0));
            diancengup.Layer = "0S-WALL-LINE";
            Line diancengdown = new Line(
                point + new Vector3d(0, 0, 0),
                point + new Vector3d(neiqiangkuan + 900, 0, 0));
            diancengdown.Layer = "0S-WALL-LINE";
            //“素砼垫层”
            MText mtextSTDC = new MText();//数字
            mtextSTDC.SetDatabaseDefaults();
            mtextSTDC.Contents = "素砼垫层";
            mtextSTDC.TextHeight = 200;
            mtextSTDC.Attachment = AttachmentPoint.BottomCenter;// 设置文本对象的插入点为文本的顶端中点
            mtextSTDC.Location = point + new Vector3d((neiqiangkuan + 900) * 0.75, -400, 0);
            mtextSTDC.Layer = "0S-BEAM-TEXT";
            //引线
            Polyline stdc = new Polyline();
            stdc.AddVertexAt(0, new Point2d(point.X + (neiqiangkuan + 900) / 2 - 100, point.Y), 0, 0, 0);
            stdc.AddVertexAt(1, new Point2d(point.X + (neiqiangkuan + 900) / 2, point.Y - 400), 0, 0, 0);
            stdc.AddVertexAt(2, new Point2d(point.X + neiqiangkuan + 900, point.Y - 400), 0, 0, 0);
            stdc.Closed = false;
            stdc.Layer = "0S-WALL-DIMS";
            //标注
            Point3d diancengbiaozhudian1 = point + new Vector3d(neiqiangkuan + 900, dianceng, 0);
            Point3d diancengbiaozhudian2 = point + new Vector3d(neiqiangkuan + 900, 0, 0);
            OutWall.DrawBiaoZhu(db, diancengbiaozhudian1, diancengbiaozhudian2, 500);

            #region 填充的边界
            // 添加边界路径
            Polyline boundary = new Polyline();
            boundary.AddVertexAt(0, new Point2d(point.X, point.Y + dianceng), 0, 0, 0);
            boundary.AddVertexAt(1, new Point2d(point.X + neiqiangkuan + 900, point.Y + dianceng), 0, 0, 0);
            boundary.AddVertexAt(2, new Point2d(point.X + neiqiangkuan + 900, point.Y), 0, 0, 0);
            boundary.AddVertexAt(3, new Point2d(point.X, point.Y), 0, 0, 0);
            boundary.AddVertexAt(4, new Point2d(point.X, point.Y + dianceng), 0, 0, 0);
            boundary.Closed = true;
            ObjectIdCollection boundaryIds = new ObjectIdCollection();

            #endregion

            #endregion

            #region 画纵向钢筋
            Vector2d vector2D = new Vector2d(point.X, point.Y);
            //外层钢筋
            Polyline waitongchang = new Polyline();
            waitongchang.AddVertexAt(0, new Point2d(neiqiangkuan0 + 900 - 70, dianceng + 20) + vector2D, 0, 0, 0);
            waitongchang.AddVertexAt(1, new Point2d(40, dianceng + 20) + vector2D, 0, 0, 0);
            waitongchang.AddVertexAt(2, new Point2d(40, totalHeight + dibanhou + dianceng - 20) + vector2D, 0, 0, 0);
            waitongchang.AddVertexAt(3, new Point2d(40 + 60, totalHeight + dibanhou + dianceng - 20) + vector2D, 0, 0, 0);
            waitongchang.Closed = false;
            waitongchang.Layer = "0S-DETL-RBAR";
            waitongchang.ConstantWidth = 10;
            //内侧钢筋
            Polyline neitongchang = new Polyline();
            neitongchang.AddVertexAt(0, new Point2d(neiqiangkuan0 + 900 - 70, dianceng + 80) + vector2D, 0, 0, 0);
            neitongchang.AddVertexAt(1, new Point2d(neiqiangkuan0 - 40, dianceng + 80) + vector2D, 0, 0, 0);
            neitongchang.AddVertexAt(2, new Point2d(neiqiangkuan0 - 40, totalHeight + dibanhou + dianceng - 20) + vector2D, 0, 0, 0);
            neitongchang.AddVertexAt(3, new Point2d(neiqiangkuan0 - 40 + 60, totalHeight + dibanhou + dianceng - 20) + vector2D, 0, 0, 0);
            neitongchang.Closed = false;
            neitongchang.Layer = "0S-DETL-RBAR";
            neitongchang.ConstantWidth = 10;
            //底板处的附加筋
            double maogu = Double.Parse(storyList[storyList.Count - 1].StoryHeight) / 3;
            Polyline dibufujia = new Polyline();
            dibufujia.AddVertexAt(0, new Point2d(neiqiangkuan0 + 900 - 70, dianceng + 50) + vector2D, 0, 0, 0);
            dibufujia.AddVertexAt(1, new Point2d(40 + 30, dianceng + 50) + vector2D, 0, 0, 0);
            dibufujia.AddVertexAt(2, new Point2d(40 + 30, maogu + dibanhou + dianceng) + vector2D, 0, 0, 0);
            dibufujia.AddVertexAt(3, new Point2d(40 + 30 + 60, maogu + dibanhou + dianceng - 60) + vector2D, 0, 0, 0);
            dibufujia.Closed = false;
            dibufujia.Layer = "0S-DETL-RBAR";
            dibufujia.ConstantWidth = 10;
            //附加筋标注
            Point3d dibanfujiajinbiaozhu1 = point + new Vector3d(40 + 30, dibanhou + dianceng, 0);
            Point3d dibanfujiajinbiaozhu2 = point + new Vector3d(40 + 30, maogu + dibanhou + dianceng, 0);
            OutWall.DrawBiaoZhu(db, dibanfujiajinbiaozhu1, dibanfujiajinbiaozhu2, 500);
            #endregion

            #region 图名区
            Vector3d vector3D = new Vector3d(point.X, point.Y, 0);
            //图名
            MText mtextTM = new MText();//数字
            mtextTM.SetDatabaseDefaults();
            mtextTM.Contents = "WA";
            mtextTM.TextHeight = 400;
            mtextTM.Attachment = AttachmentPoint.BottomCenter;// 设置文本对象的插入点为文本的顶端中点
            mtextTM.Location = new Point3d(450, -900, 0) + vector3D;
            mtextTM.Layer = "0S-BEAM-TEXT";
            //下划线
            Polyline xhx = new Polyline();
            xhx.AddVertexAt(0, new Point2d(point.X + 300, point.Y - 900), 0, 0, 0);
            xhx.AddVertexAt(1, new Point2d(point.X + 600, point.Y - 900), 0, 0, 0);
            xhx.Closed = false;
            xhx.Layer = "0S-DETL-SYMB";
            xhx.ConstantWidth = 10;
            Line lineXHX = new Line(new Point3d(300, -900 - 20, 0) + vector3D, new Point3d(600, -900 - 20, 0) + vector3D);
            lineXHX.Layer = "0S-DETL-SYMB";
            #endregion

            #region 把“线们”添加到模型空间，并画出来
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {

                //打开块表（若前面已经打开，这里可以注释掉）
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开块表记录
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                #region 设置填充
                btr.AppendEntity(boundary);
                tr.AddNewlyCreatedDBObject(boundary, true);
                boundary.Visible = false;
                boundaryIds.Add(boundary.ObjectId); // 添加多段线的 ObjectId 到边界集合
                // 创建填充对象
                Hatch hatch = new Hatch();
                // 设置填充类型和样式
                hatch.SetHatchPattern(HatchPatternType.PreDefined, "AR-CONC");
                hatch.Layer = "0S-WALL-LINE";
                btr.AppendEntity(hatch);
                tr.AddNewlyCreatedDBObject(hatch, true);
                //设置关联
                hatch.Associative = true;
                // 添加边界路径
                hatch.AppendLoop(HatchLoopTypes.Outermost, boundaryIds);
                //计算填充并显示
                hatch.EvaluateHatch(true);
                #endregion

                #region 多段线的尝试
                Polyline polyline = new Polyline();
                polyline.SetDatabaseDefaults();
                polyline.Closed = false;

                // 添加点到Polyline
                polyline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                polyline.AddVertexAt(1, new Point2d(10, 0), 0, 0, 0);

                // 计算半圆弧所需的参数
                double radius = 5.0;
                double startAngle = Math.PI;
                double endAngle = 2 * Math.PI;
                int numSegments = 36; // 半圆弧的线段数

                // 计算半圆弧上的点
                for (int i = 0; i <= numSegments; i++)
                {
                    double angle = startAngle + (endAngle - startAngle) * i / numSegments;
                    double x = 15 + radius * Math.Cos(angle);
                    double y = radius * Math.Sin(angle);
                    polyline.AddVertexAt(i + 2, new Point2d(x, y), 0, 0, 0);
                }

                polyline.AddVertexAt(2 + numSegments + 1, new Point2d(20, 0), 0, 0, 0);
                polyline.AddVertexAt(3 + numSegments + 1, new Point2d(25, 0), 0, 0, 0);
                polyline.AddVertexAt(4 + numSegments + 1, new Point2d(35, 0), 0, 0, 0);
                polyline.Layer = "0S-DETL-RBAR";
                //btr.AppendEntity(polyline);
                //tr.AddNewlyCreatedDBObject(polyline, true); 
                #endregion

                btr.AppendEntity(outwallline1);
                tr.AddNewlyCreatedDBObject(outwallline1, true);
                btr.AppendEntity(outwallline2);
                tr.AddNewlyCreatedDBObject(outwallline2, true);
                btr.AppendEntity(waiqiangzdx);
                tr.AddNewlyCreatedDBObject(waiqiangzdx, true);
                btr.AppendEntity(inwallline2);
                tr.AddNewlyCreatedDBObject(inwallline2, true);
                btr.AppendEntity(diban);
                tr.AddNewlyCreatedDBObject(diban, true);
                btr.AppendEntity(dibanzdx);
                tr.AddNewlyCreatedDBObject(dibanzdx, true);
                btr.AppendEntity(biaogaodixian1);
                tr.AddNewlyCreatedDBObject(biaogaodixian1, true);
                btr.AppendEntity(biaogaoshangxian1);
                tr.AddNewlyCreatedDBObject(biaogaoshangxian1, true);
                btr.AppendEntity(biaogaozuoxiexian1);
                tr.AddNewlyCreatedDBObject(biaogaozuoxiexian1, true);
                btr.AppendEntity(biaogaoyouxiexian1);
                tr.AddNewlyCreatedDBObject(biaogaoyouxiexian1, true);

                #region 文本
                //获取文本样式表
                TextStyleTable textStyleTable = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForWrite);
                //获取文件中的“XD2008”styles
                TextStyleTableRecord textStyleObj = (TextStyleTableRecord)tr.GetObject(textStyleTable["盈建科标注"], OpenMode.ForRead);
                mtext1.TextStyleId = textStyleObj.ObjectId;
                mtext11.TextStyleId = textStyleObj.ObjectId;
                mtext2.TextStyleId = textStyleObj.ObjectId;
                mtextSTDC.TextStyleId = textStyleObj.ObjectId;
                mtextDB.TextStyleId = textStyleObj.ObjectId;
                mtextTM.TextStyleId = textStyleObj.ObjectId;
                mtextLaE.TextStyleId = textStyleObj.ObjectId;
                mtextSN.TextStyleId = textStyleObj.ObjectId;
                mtextSW.TextStyleId = textStyleObj.ObjectId;
                mtextDXLXQ.TextStyleId = textStyleObj.ObjectId;
                btr.AppendEntity(mtext1);
                tr.AddNewlyCreatedDBObject(mtext1, true);
                btr.AppendEntity(mtext11);
                tr.AddNewlyCreatedDBObject(mtext11, true);
                btr.AppendEntity(mtextSTDC);
                tr.AddNewlyCreatedDBObject(mtextSTDC, true);
                btr.AppendEntity(mtextDB);
                tr.AddNewlyCreatedDBObject(mtextDB, true);
                btr.AppendEntity(mtextTM);
                tr.AddNewlyCreatedDBObject(mtextTM, true);
                btr.AppendEntity(mtextLaE);
                tr.AddNewlyCreatedDBObject(mtextLaE, true);
                btr.AppendEntity(mtextSN);
                tr.AddNewlyCreatedDBObject(mtextSN, true);
                btr.AppendEntity(mtextSW);
                tr.AddNewlyCreatedDBObject(mtextSW, true);
                btr.AppendEntity(mtextDXLXQ);
                tr.AddNewlyCreatedDBObject(mtextDXLXQ, true);
                #endregion

                btr.AppendEntity(dingbanup);
                tr.AddNewlyCreatedDBObject(dingbanup, true);
                btr.AppendEntity(biaogaodixian2);
                tr.AddNewlyCreatedDBObject(biaogaodixian2, true);
                btr.AppendEntity(biaogaoshangxian2);
                tr.AddNewlyCreatedDBObject(biaogaoshangxian2, true);
                btr.AppendEntity(biaogaozuoxiexian2);
                tr.AddNewlyCreatedDBObject(biaogaozuoxiexian2, true);
                btr.AppendEntity(biaogaoyouxiexian2);
                tr.AddNewlyCreatedDBObject(biaogaoyouxiexian2, true);
                btr.AppendEntity(mtext2);
                tr.AddNewlyCreatedDBObject(mtext2, true);
                btr.AppendEntity(dingbandown);
                tr.AddNewlyCreatedDBObject(dingbandown, true);
                btr.AppendEntity(dingbanzdx);
                tr.AddNewlyCreatedDBObject(dingbanzdx, true);
                btr.AppendEntity(diancengup);
                tr.AddNewlyCreatedDBObject(diancengup, true);
                btr.AppendEntity(diancengdown);
                tr.AddNewlyCreatedDBObject(diancengdown, true);
                btr.AppendEntity(thickoutwallline);
                tr.AddNewlyCreatedDBObject(thickoutwallline, true);
                btr.AppendEntity(waitongchang);
                tr.AddNewlyCreatedDBObject(waitongchang, true);
                btr.AppendEntity(neitongchang);
                tr.AddNewlyCreatedDBObject(neitongchang, true);
                btr.AppendEntity(dibufujia);
                tr.AddNewlyCreatedDBObject(dibufujia, true);
                btr.AppendEntity(stdc);
                tr.AddNewlyCreatedDBObject(stdc, true);
                btr.AppendEntity(xhx);
                tr.AddNewlyCreatedDBObject(xhx, true);
                btr.AppendEntity(lineXHX);
                tr.AddNewlyCreatedDBObject(lineXHX, true);
                btr.AppendEntity(lineDXLXQ);
                tr.AddNewlyCreatedDBObject(lineDXLXQ, true);
                btr.AppendEntity(shineikuang);
                tr.AddNewlyCreatedDBObject(shineikuang, true);
                btr.AppendEntity(shiwaikuang);
                tr.AddNewlyCreatedDBObject(shiwaikuang, true);

                #region 画内墙
                btr.AppendEntity(dingcengwall);
                tr.AddNewlyCreatedDBObject(dingcengwall, true);

                foreach (Line line in insidewall)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }
                #endregion

                #region 内墙的水平钢筋和半圆箍筋
                btr.AppendEntity(di_midcircle1);
                tr.AddNewlyCreatedDBObject(di_midcircle1, true);
                btr.AppendEntity(di_midcircle2);
                tr.AddNewlyCreatedDBObject(di_midcircle2, true);
                btr.AppendEntity(di_upcircle1);
                tr.AddNewlyCreatedDBObject(di_upcircle1, true);
                btr.AppendEntity(di_upcircle2);
                tr.AddNewlyCreatedDBObject(di_upcircle2, true);
                btr.AppendEntity(di_downcircle1);
                tr.AddNewlyCreatedDBObject(di_downcircle1, true);
                btr.AppendEntity(di_downcircle2);
                tr.AddNewlyCreatedDBObject(di_downcircle2, true);
                btr.AppendEntity(di_xiegujin);
                tr.AddNewlyCreatedDBObject(di_xiegujin, true);
                foreach (Polyline line in neiqiangpeijin)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }
                #endregion

                foreach (Line line in uploubanlist)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }

                foreach (Line line in downloubanlist)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }
                ;
                foreach (Polyline line in fujiajin)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }


                foreach (Polyline line in zhongjianzdx)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }

                foreach (Line line in biaogaolist)
                {
                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                }

                foreach (MText mtext in biaogaotextlist)
                {
                    mtext.TextStyleId = textStyleObj.ObjectId;
                    btr.AppendEntity(mtext);
                    tr.AddNewlyCreatedDBObject(mtext, true);
                }
                tr.Commit();

                // Dispose the MText object
                mtextDXLXQ.Dispose();
            }

            #endregion

            m_DocumentLock.Dispose();//一定要记得解锁
        }
        #endregion
    }
}
