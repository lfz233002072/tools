/*======================================================================
 *
 *        Copyright (C)  1996-2012  lfz    
 *        All rights reserved
 *
 *        Filename :LineEquation.cs
 *        DESCRIPTION :
 *
 *        Created By 林芳崽 at 2013-07-05 10:32
 *        https://git.oschina.net/lfz/tools
 *
 *======================================================================*/

using System;
using System.Drawing;
using Lfz.Utitlies;

namespace Lfz.Draw
{

    /// <summary>
    /// Ax + By = C 关联的直线(其中B取1)
    /// </summary>
    public class LineEquation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public LineEquation(Point start, Point end)
        {
            Start = start;
            End = end;

            IsVertical = Math.Abs(End.X - start.X) < 0.00001f;
            if (IsVertical)
            {
                A = 1;
                B = 0;
                C = end.X;
            }
            else
            {
                A = -((double)End.Y - Start.Y) / ((double)End.X - Start.X);
                B = 1;
                C = Start.Y + A * Start.X;
            }
        }

        /// <summary>
        /// 是否为垂直线条
        /// </summary>
        public bool IsVertical { get; private set; }


        /// <summary>
        /// 开始节点
        /// </summary>
        public Point Start { get; private set; }

        /// <summary>
        /// 结束节点
        /// </summary>
        public Point End { get; private set; }

        /// <summary>
        /// 直线表达式Ax + By = C 中的A
        /// </summary>
        public double A { get; private set; }
        /// <summary>
        /// 直线表达式Ax + By = C 中的B
        /// </summary>
        public double B { get; private set; }
        /// <summary>
        /// 直线表达式Ax + By = C 中的C
        /// </summary>
        public double C { get; private set; }

        /// <summary>
        /// 两直线是否相交
        /// </summary>
        /// <param name="otherLine"></param>
        /// <param name="intersectionPoint"></param>
        /// <returns></returns>
        public bool IntersectsWithLine(LineEquation otherLine, out Point intersectionPoint)
        {
            intersectionPoint = new Point(0, 0);
            if (IsVertical && otherLine.IsVertical)
                return false;
            if (IsVertical || otherLine.IsVertical)
            {
                intersectionPoint = GetIntersectionPointIfOneIsVertical(otherLine, this);
                return true;
            }
            double delta = A * otherLine.B - otherLine.A * B;
            bool hasIntersection = Math.Abs(delta - 0) > 0.0001f;
            if (hasIntersection)
            {
                double x = (otherLine.B * C - B * otherLine.C) / delta;
                double y = (A * otherLine.C - otherLine.A * C) / delta;
                intersectionPoint = new Point(TypeParse.StrToInt(x), TypeParse.StrToInt(y)); ;
            }


            /* float delta = A1*B2 - A2*B1; 
             * float x = (B2 * C1 - B1 * C2) / delta;
                float y = (A1 * C2 - A2 * C1) / delta;*/
            return hasIntersection;
        }

        private static Point GetIntersectionPointIfOneIsVertical(LineEquation line1, LineEquation line2)
        {
            LineEquation verticalLine = line2.IsVertical ? line2 : line1;
            LineEquation nonVerticalLine = line2.IsVertical ? line1 : line2;

            double y = (verticalLine.Start.X - nonVerticalLine.Start.X) *
                       (nonVerticalLine.End.Y - nonVerticalLine.Start.Y) /
                       ((nonVerticalLine.End.X - nonVerticalLine.Start.X)) +
                       nonVerticalLine.Start.Y;
            double x = verticalLine.Start.X;
            return new Point(TypeParse.StrToInt(x), TypeParse.StrToInt(y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + Start + "], [" + End + "]";
        }
         
    }
}