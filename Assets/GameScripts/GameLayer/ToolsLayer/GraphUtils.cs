using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 图形工具类
    /// </summary>
    public class GraphUtils
    {
        /// <summary>
        /// 检测圆形是否与多个矩形相交
        /// </summary>
        /// <param name="circlePivot">圆心坐标</param>
        /// <param name="circleRadius">圆形半径</param>
        /// <param name="rects">矩形列表</param>
        /// <returns>如果与任意一个矩形相交则返回true，否则返回false</returns>
        public static bool CircleHitRects(Vector2 circlePivot, float circleRadius, List<Rect> rects)
        {
            // 如果矩形列表为空,直接返回false
            if (rects.Count <= 0)
                return false;

            // 遍历所有矩形,检查是否有相交
            for (var i = 0; i < rects.Count; i++)
            {
                if (GraphUtils.CircleHitRect(circlePivot, circleRadius, rects[i]) == true)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 检测圆形是否与多个矩形相交(数组版本)
        /// </summary>
        /// <param name="circlePivot">圆心坐标</param>
        /// <param name="circleRadius">圆形半径</param>
        /// <param name="rects">矩形数组</param>
        /// <returns>如果与任意一个矩形相交则返回true，否则返回false</returns>
        public static bool CircleHitRects(Vector2 circlePivot, float circleRadius, Rect[] rects)
        {
            List<Rect> rl = new List<Rect>();
            for (var i = 0; i < rects.Length; i++)
            {
                rl.Add(rects[i]);
            }
            return CircleHitRects(circlePivot, circleRadius, rl);
        }

        /// <summary>
        /// 检测圆形是否与矩形相交
        /// </summary>
        /// <param name="circlePivot">圆心坐标</param>
        /// <param name="circleRadius">圆形半径</param>
        /// <param name="rect">矩形</param>
        /// <returns>如果相交返回true，否则返回false</returns>
        public static bool CircleHitRect(Vector2 circlePivot, float circleRadius, Rect rect)
        {
            // 判断圆心在矩形的哪个区域
            // xp: 0-左侧 1-中间 2-右侧
            int xp = circlePivot.x < rect.x ? 0 : (circlePivot.x > rect.x + rect.width ? 2 : 1);
            // yp: 0-下方 1-中间 2-上方
            int yp = circlePivot.y < rect.y ? 0 : (circlePivot.y > rect.y + rect.height ? 2 : 1);

            // 圆心在矩形内部,必定相交
            if (yp == 1 && xp == 1)
                return true;  //在中间，则一定命中

            // 圆心在矩形上下方
            if (yp != 1 && xp == 1)
            {
                float halfRect = rect.height / 2;
                // 计算圆心到矩形中心的垂直距离
                float toHeart = Mathf.Abs(circlePivot.y - (rect.y + halfRect));
                return (toHeart <= circleRadius + halfRect);
            }
            // 圆心在矩形左右方
            else if (yp == 1 && xp != 1)
            {
                float halfRect = rect.width / 2;
                // 计算圆心到矩形中心的水平距离
                float toHeart = Mathf.Abs(circlePivot.x - (rect.x + halfRect));
                return (toHeart <= circleRadius + halfRect);
            }
            // 圆心在矩形四个角落区域
            else
            {
                // 判断圆心到最近顶点的距离是否小于半径
                return InRange(
                    circlePivot.x, circlePivot.y,
                    yp == 0 ? rect.x : (rect.x + rect.width),
                    xp == 0 ? rect.y : (rect.y + rect.height),
                    circleRadius
                );
            }
        }

        ///<summary>
        ///AABB的矩形之间是否有碰撞
        ///<param name="a">一个rect</param>
        ///<param name="b">另一个rect</param>
        ///<return>是否碰撞到了，true代表碰到了</return>
        ///</summary>
        public static bool RectCollide(Rect a, Rect b)
        {
            float ar = a.x + a.width;
            float br = b.x + b.width;
            float ab = a.y + a.height;
            float bb = b.y + b.height;
            return (
                (a.x >= b.x && a.x <= br) ||
                (b.x >= a.x && b.x <= ar)
            ) && (
                (a.y >= b.y && a.y <= bb) ||
                (b.y >= a.y && b.y <= ab)
            );
        }


        public static bool InRange(float x1, float y1, float x2, float y2, float range)
        {
            return Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) <= Mathf.Pow(range, 2);
        }


        ///<summary>
        ///根据面向和移动方向得到一个资源名预订了规则的后缀名
        ///<param name="faceDegree">面向角度</param>
        ///<param name="moveDegree">移动角度</param>
        ///<return>约定好的关键字，比如"Forward","Back","Left","Right"，对应到角色动画的key</return>
        ///</summary>
        public static string GetTailStringByDegree(float faceDegree, float moveDegree)
        {
            float fd = faceDegree;
            float md = moveDegree;
            while (fd < 180)
                fd += 360;
            while (md < 180)
                md += 360;
            fd = fd % 360;
            md = md % 360;
            float dd = md - fd;
            if (dd > 180)
            {
                dd -= 360;
            }
            else if (dd < -180)
            {
                dd += 360;
            }
            //Debug.Log("degree:"+fd + " / " + md + " / " + dd);
            if (dd >= -45 && dd <= 45)
            {
                return "Forward";
            }
            else
            if (dd < -45 && dd >= -135)
            {
                return "Left";
            }
            else
            if (dd > 45 && dd <= 135)
            {
                return "Right";
            }
            else
            {
                return "Back";
            }
        }

        public static bool CheckCover(SpriteRenderer source, SpriteRenderer target)
        {
            return source.bounds.Intersects(target.bounds);
        }

        /// <summary>
        /// 判断点是否在多边形内
        /// 相交次数为奇数则点在多边形内，否则在多边形外
        /// </summary>
        /// <param name="checkPoint">监测坐标点</param>
        /// <param name="polygonVerticeList">多边形顶点坐标。需要数组是有序的，顶点是相邻的</param>
        /// <returns></returns>
        public static bool CheckPointInPolygonShape(Vector2 checkPoint, Vector3 shaperCenterWorldPos, Vector3[] shapeVerterLocalPosArray)
        {
            bool isInShape = false;
            int verticeCount = shapeVerterLocalPosArray.Length;
            for (int i = 0, j = verticeCount - 1; i < verticeCount; i++)
            {
                Vector2[] lineVertices = { shaperCenterWorldPos + shapeVerterLocalPosArray[j], shaperCenterWorldPos + shapeVerterLocalPosArray[i] };
                if (CheckPositiveHorizontalRayIntersectsLineSegment(checkPoint, lineVertices))
                {
                    isInShape = !isInShape;
                }
                j = i;
            }
            return isInShape;
        }

        /// <summary>
        /// 判断点是否在多边形内
        /// 相交次数为奇数则点在多边形内，否则在多边形外
        /// </summary>
        /// <param name="checkPoint">监测坐标点</param>
        /// <param name="polygonVerticeList">多边形顶点坐标。需要数组是有序的，顶点是相邻的</param>
        public static bool CheckPointInPolygonShape2(Vector2 checkPoint, Vector3[] shapeVerterWorldPosArray)
        {
            bool isInShape = false;
            int verticeCount = shapeVerterWorldPosArray.Length;
            for (int i = 0, j = verticeCount - 1; i < verticeCount; i++)
            {
                Vector2[] lineVertices = { shapeVerterWorldPosArray[j], shapeVerterWorldPosArray[i] };
                if (CheckPositiveHorizontalRayIntersectsLineSegment(checkPoint, lineVertices))
                {
                    isInShape = !isInShape;
                }
                j = i;
            }
            return isInShape;
        }


        /// <summary>
        /// 判断正x轴方向的水平射线与线段是否有交点
        /// 使用两点式公式判定 (X-X1)/(X1-X2)=(Y-Y1)/(Y1-Y2)
        /// </summary>
        /// <param name="checkPoint">水平射线的起点</param>
        /// <param name="lineVertices">线段的两端坐标</param>
        /// <returns></returns>
        static bool CheckPositiveHorizontalRayIntersectsLineSegment(Vector2 checkPoint, Vector2[] lineVertices)
        {
            float pointY = checkPoint.y;
            if (lineVertices[0].y == lineVertices[1].y)
            {
                //这是水平线段
                if (pointY == lineVertices[0].y)
                    return CheckPointInLineSegment(checkPoint, lineVertices);
                else
                    return false;
            }
            float calX = (pointY - lineVertices[0].y) / (lineVertices[1].y - lineVertices[0].y) * (lineVertices[1].x - lineVertices[0].x) + lineVertices[0].x;
            if (calX >= checkPoint.x)
                return CheckPointInLineSegment(new Vector2(calX, pointY), lineVertices);
            else
                return false;
        }

        /// <summary>
        /// 判断点是否在线段上
        /// </summary>
        /// <param name="checkPoint">检测点</param>
        /// <param name="lineVertices">线段的两端坐标</param>
        /// <returns></returns>
        static bool CheckPointInLineSegment(Vector2 checkPoint, Vector2[] lineVertices)
        {
            float lineMaxX = Mathf.Max(lineVertices[0].x, lineVertices[1].x);
            float lineMinX = Mathf.Min(lineVertices[0].x, lineVertices[1].x);
            if (checkPoint.x > lineMaxX || checkPoint.x < lineMinX)
                return false;

            float lineMaxY = Mathf.Max(lineVertices[0].y, lineVertices[1].y);
            float lineMinY = Mathf.Min(lineVertices[0].y, lineVertices[1].y);
            if (checkPoint.y > lineMaxY || checkPoint.y < lineMinY)
                return false;

            if (checkPoint.Equals(lineVertices[0]) || checkPoint.Equals(lineVertices[1]))
                return true;

            float slope1 = (checkPoint.x - lineVertices[0].x) / (checkPoint.y - lineVertices[0].y);
            float slope2 = (checkPoint.x - lineVertices[1].x) / (checkPoint.y - lineVertices[1].y);
            return slope1.Equals(slope2);
        }

        /// <summary>
        /// 判断多边形A是否与多边形B有重叠区域
        /// </summary>
        /// <param name="shapeACenterWorldPos">A的中心点</param>
        /// <param name="shapeAVerterLocalPosArray">A的顶点局部坐标</param>
        /// <param name="shapeBCenterWorldPos">B的中心点</param>
        /// <param name="shapeBVerterLocalPosArray">B的顶点局部坐标</param>
        /// <returns></returns>
        public static bool CheckPolygonOverlap(Vector3 shapeACenterWorldPos, Vector3[] shapeAVerterLocalPosArray, Vector3 shapeBCenterWorldPos, Vector3[] shapeBVerterLocalPosArray)
        {
            //遍历A的每个顶点，判断是否在B内
            foreach (var vertex in shapeAVerterLocalPosArray)
            {
                if (CheckPointInPolygonShape(vertex + shapeACenterWorldPos, shapeBCenterWorldPos, shapeBVerterLocalPosArray))
                {
                    return true;
                }
            }
            return false;
        }
    }
}