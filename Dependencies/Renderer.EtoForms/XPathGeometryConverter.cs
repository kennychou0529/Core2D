﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Eto.Drawing;
using Core2D;

namespace TestEtoForms
{
    /// <summary>
    /// 
    /// </summary>
    public static class XPathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static GraphicsPath ToGraphicsPath(this XPathGeometry pg, double dx, double dy, Func<double, float> scale)
        {
            var gp = new GraphicsPath();
            gp.FillMode = pg.FillRule == Core2D.XFillRule.EvenOdd ? FillMode.Alternate : FillMode.Winding;

            foreach (var pf in pg.Figures)
            {
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is Core2D.XArcSegment)
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        //var arcSegment = segment as Core2D.XArcSegment;
                        // TODO: Convert WPF/SVG elliptical arc segment format to GDI+ bezier curves.
                        //startPoint = arcSegment.Point;
                    }
                    else if (segment is Core2D.XBezierSegment)
                    {
                        var bezierSegment = segment as Core2D.XBezierSegment;
                        gp.AddBezier(
                            new PointF(
                                scale(startPoint.X),
                                scale(startPoint.Y)),
                            new PointF(
                                scale(bezierSegment.Point1.X),
                                scale(bezierSegment.Point1.Y)),
                            new PointF(
                                scale(bezierSegment.Point2.X),
                                scale(bezierSegment.Point2.Y)),
                            new PointF(
                                scale(bezierSegment.Point3.X),
                                scale(bezierSegment.Point3.Y)));
                        startPoint = bezierSegment.Point3;
                    }
                    else if (segment is Core2D.XLineSegment)
                    {
                        var lineSegment = segment as Core2D.XLineSegment;
                        gp.AddLine(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(lineSegment.Point.X),
                            scale(lineSegment.Point.Y));
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is Core2D.XPolyBezierSegment)
                    {
                        var polyBezierSegment = segment as Core2D.XPolyBezierSegment;
                        if (polyBezierSegment.Points.Count >= 3)
                        {
                            gp.AddBezier(
                                new PointF(
                                    scale(startPoint.X),
                                    scale(startPoint.Y)),
                                new PointF(
                                    scale(polyBezierSegment.Points[0].X),
                                    scale(polyBezierSegment.Points[0].Y)),
                                new PointF(
                                    scale(polyBezierSegment.Points[1].X),
                                    scale(polyBezierSegment.Points[1].Y)),
                                new PointF(
                                    scale(polyBezierSegment.Points[2].X),
                                    scale(polyBezierSegment.Points[2].Y)));
                        }

                        if (polyBezierSegment.Points.Count > 3
                            && polyBezierSegment.Points.Count % 3 == 0)
                        {
                            for (int i = 3; i < polyBezierSegment.Points.Count; i += 3)
                            {
                                gp.AddBezier(
                                    new PointF(
                                        scale(polyBezierSegment.Points[i - 1].X),
                                        scale(polyBezierSegment.Points[i - 1].Y)),
                                    new PointF(
                                        scale(polyBezierSegment.Points[i].X),
                                        scale(polyBezierSegment.Points[i].Y)),
                                    new PointF(
                                        scale(polyBezierSegment.Points[i + 1].X),
                                        scale(polyBezierSegment.Points[i + 1].Y)),
                                    new PointF(
                                        scale(polyBezierSegment.Points[i + 2].X),
                                        scale(polyBezierSegment.Points[i + 2].Y)));
                            }
                        }

                        startPoint = polyBezierSegment.Points.Last();
                    }
                    else if (segment is Core2D.XPolyLineSegment)
                    {
                        var polyLineSegment = segment as Core2D.XPolyLineSegment;
                        if (polyLineSegment.Points.Count >= 1)
                        {
                            gp.AddLine(
                                scale(startPoint.X),
                                scale(startPoint.Y),
                                scale(polyLineSegment.Points[0].X),
                                scale(polyLineSegment.Points[0].Y));
                        }

                        if (polyLineSegment.Points.Count > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Count; i++)
                            {
                                gp.AddLine(
                                    scale(polyLineSegment.Points[i - 1].X),
                                    scale(polyLineSegment.Points[i - 1].Y),
                                    scale(polyLineSegment.Points[i].X),
                                    scale(polyLineSegment.Points[i].Y));
                            }
                        }

                        startPoint = polyLineSegment.Points.Last();
                    }
                    else if (segment is Core2D.XPolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as Core2D.XPolyQuadraticBezierSegment;
                        if (polyQuadraticSegment.Points.Count >= 2)
                        {
                            var p1 = startPoint;
                            var p2 = polyQuadraticSegment.Points[0];
                            var p3 = polyQuadraticSegment.Points[1];
                            double x1 = p1.X;
                            double y1 = p1.Y;
                            double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                            double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                            double x3 = x2 + (p3.X - p1.X) / 3.0;
                            double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                            double x4 = p3.X;
                            double y4 = p3.Y;
                            gp.AddBezier(
                                new PointF(
                                    scale(x1 + dx),
                                    scale(y1 + dy)),
                                new PointF(
                                    scale(x2 + dx),
                                    scale(y2 + dy)),
                                new PointF(
                                    scale(x3 + dx),
                                    scale(y3 + dy)),
                                new PointF(
                                    scale(x4 + dx),
                                    scale(y4 + dy)));
                        }

                        if (polyQuadraticSegment.Points.Count > 2
                            && polyQuadraticSegment.Points.Count % 2 == 0)
                        {
                            for (int i = 3; i < polyQuadraticSegment.Points.Count; i += 3)
                            {
                                var p1 = polyQuadraticSegment.Points[i - 1];
                                var p2 = polyQuadraticSegment.Points[i];
                                var p3 = polyQuadraticSegment.Points[i + 1];
                                double x1 = p1.X;
                                double y1 = p1.Y;
                                double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                                double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                                double x3 = x2 + (p3.X - p1.X) / 3.0;
                                double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                                double x4 = p3.X;
                                double y4 = p3.Y;
                                gp.AddBezier(
                                    new PointF(
                                        scale(x1 + dx),
                                        scale(y1 + dy)),
                                    new PointF(
                                        scale(x2 + dx),
                                        scale(y2 + dy)),
                                    new PointF(
                                        scale(x3 + dx),
                                        scale(y3 + dy)),
                                    new PointF(
                                        scale(x4 + dx),
                                        scale(y4 + dy)));
                            }
                        }

                        startPoint = polyQuadraticSegment.Points.Last();
                    }
                    else if (segment is Core2D.XQuadraticBezierSegment)
                    {
                        var qbezierSegment = segment as Core2D.XQuadraticBezierSegment;
                        var p1 = startPoint;
                        var p2 = qbezierSegment.Point1;
                        var p3 = qbezierSegment.Point2;
                        double x1 = p1.X;
                        double y1 = p1.Y;
                        double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                        double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                        double x3 = x2 + (p3.X - p1.X) / 3.0;
                        double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                        double x4 = p3.X;
                        double y4 = p3.Y;
                        gp.AddBezier(
                            new PointF(
                                scale(x1 + dx),
                                scale(y1 + dy)),
                            new PointF(
                                scale(x2 + dx),
                                scale(y2 + dy)),
                            new PointF(
                                scale(x3 + dx),
                                scale(y3 + dy)),
                            new PointF(
                                scale(x4 + dx),
                                scale(y4 + dy)));
                        startPoint = qbezierSegment.Point2;
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                if (pf.IsClosed)
                {
                    gp.CloseFigure();
                }
                else
                {
                    gp.StartFigure();
                }
            }

            return gp;
        }
    }
}