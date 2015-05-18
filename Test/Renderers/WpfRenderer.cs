﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Test2d;

namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    public class WpfRenderer : ObservableObject, IRenderer
    {
        private double _zoom;
        private double _panX;
        private double _panY;
        private ShapeState _drawShapeState;
        private BaseShape _selectedShape;
        private ICollection<BaseShape> _selectedShapes;

        /// <summary>
        /// 
        /// </summary>
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value != _zoom)
                {
                    _zoom = value;
                    Notify("Zoom");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double PanX
        {
            get { return _panX; }
            set
            {
                if (value != _panX)
                {
                    _panX = value;
                    Notify("PanX");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double PanY
        {
            get { return _panY; }
            set
            {
                if (value != _panY)
                {
                    _panY = value;
                    Notify("PanY");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState DrawShapeState
        {
            get { return _drawShapeState; }
            set
            {
                if (value != _drawShapeState)
                {
                    _drawShapeState = value;
                    Notify("DrawShapeState");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseShape SelectedShape
        {
            get { return _selectedShape; }
            set
            {
                if (value != _selectedShape)
                {
                    _selectedShape = value;
                    Notify("SelectedShape");
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ICollection<BaseShape> SelectedShapes
        {
            get { return _selectedShapes; }
            set
            {
                if (value != _selectedShapes)
                {
                    _selectedShapes = value;
                    Notify("SelectedShapes");
                }
            }
        }

        private const bool _enableGuidelines = true;
        private const bool _enableStyleCache = true;
        private const bool _enableArcCache = true;
        private const bool _enableBezierCache = true;
        private const bool _enableQBezierCache = true;
        private const bool _enableTextCache = true;
        private const bool _enableImageCache = true;

        private IDictionary<ShapeStyle, Tuple<Brush, Pen>> _styleCache;
        private IDictionary<XArc, PathGeometry> _arcCache;
        private IDictionary<XBezier, PathGeometry> _bezierCache;
        private IDictionary<XQBezier, PathGeometry> _qbezierCache;
        private IDictionary<XText, Tuple<string, FormattedText>> _textCache;
        private IDictionary<Uri, BitmapImage> _biCache;

        /// <summary>
        /// 
        /// </summary>
        public WpfRenderer()
        {
            _zoom = 1.0;
            _drawShapeState = ShapeState.Visible | ShapeState.Printable;
            _selectedShape = default(BaseShape);
            _selectedShapes = default(ICollection<BaseShape>);

            ClearCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IRenderer Create()
        {
            return new WpfRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="rect"></param>
        /// <param name="ft"></param>
        /// <returns></returns>
        private static Point GetTextOrigin(ShapeStyle style, ref Rect rect, FormattedText ft)
        {
            double ox, oy;

            switch (style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    ox = rect.TopLeft.X;
                    break;
                case TextHAlignment.Right:
                    ox = rect.Right - ft.Width;
                    break;
                case TextHAlignment.Center:
                default:
                    ox = (rect.Left + rect.Width / 2.0) - (ft.Width / 2.0);
                    break;
            }

            switch (style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    oy = rect.TopLeft.Y;
                    break;
                case TextVAlignment.Bottom:
                    oy = rect.Bottom - ft.Height;
                    break;
                case TextVAlignment.Center:
                default:
                    oy = (rect.Bottom - rect.Height / 2.0) - (ft.Height / 2.0);
                    break;
            }

            return new Point(ox, oy);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Brush CreateBrush(ArgbColor color)
        {
            var brush = new SolidColorBrush(
                Color.FromArgb(
                    color.A,
                    color.R,
                    color.G,
                    color.B));
            brush.Freeze();
            return brush;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        private static Pen CreatePen(LineStyle ls, ArgbColor color, double thickness)
        {
            var brush = CreateBrush(color);
            var pen = new Pen(brush, thickness);
            switch (ls.LineCap)
            {
                case LineCap.Flat:
                    pen.StartLineCap = PenLineCap.Flat;
                    pen.EndLineCap = PenLineCap.Flat;
                    pen.DashCap = PenLineCap.Flat;
                    break;
                case LineCap.Square:
                    pen.StartLineCap = PenLineCap.Square;
                    pen.EndLineCap = PenLineCap.Square;
                    pen.DashCap = PenLineCap.Square;
                    break;
                case LineCap.Round:
                    pen.StartLineCap = PenLineCap.Round;
                    pen.EndLineCap = PenLineCap.Round;
                    pen.DashCap = PenLineCap.Round;
                    break;
            }
            pen.DashStyle = new DashStyle(ls.Dashes, ls.DashOffset);
            pen.DashStyle.Offset = ls.DashOffset;
            pen.Freeze();
            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="br"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private static Rect CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new Rect(
                new Point(tlx + dx, tly + dy),
                new Point(brx + dx, bry + dy));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="pen"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private static void DrawLineInternal(
            DrawingContext dc, 
            double half, 
            Pen pen, 
            ref Point p0, 
            ref Point p1)
        {
            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] { p0.X + half, p1.X + half },
                    new double[] { p0.Y + half, p1.Y + half });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawLine(pen, p0, p1);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawRectangleInternal(
            DrawingContext dc, 
            double half, 
            Brush brush, 
            Pen pen, 
            bool isFilled, 
            ref Rect rect)
        {
            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] 
                        { 
                            rect.TopLeft.X + half, 
                            rect.BottomRight.X + half 
                        },
                    new double[] 
                        { 
                            rect.TopLeft.Y + half,
                            rect.BottomRight.Y + half
                        });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawRectangle(isFilled ? brush : null, pen, rect);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isFilled"></param>
        /// <param name="center"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        private static void DrawEllipseInternal(
            DrawingContext dc, 
            double half, 
            Brush brush, 
            Pen pen, 
            bool isFilled, 
            ref Point center,
            double rx, double ry)
        {
            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] 
                        { 
                            center.X - rx + half, 
                            center.X + rx + half 
                        },
                    new double[] 
                        { 
                            center.Y - ry + half,
                            center.Y + ry + half
                        });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawEllipse(
                isFilled ? brush : null,
                pen,
                center,
                rx, ry);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isFilled"></param>
        /// <param name="pg"></param>
        private static void DrawPathGeometryInternal(
            DrawingContext dc,
            double half,
            Brush brush,
            Pen pen,
            bool isFilled,
            PathGeometry pg)
        {
            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] 
                        { 
                            pg.Bounds.TopLeft.X + half, 
                            pg.Bounds.BottomRight.X + half 
                        },
                    new double[] 
                        { 
                            pg.Bounds.TopLeft.Y + half,
                            pg.Bounds.BottomRight.Y + half
                        });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawGeometry(isFilled ? brush : null, pen, pg);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearCache()
        {
            _styleCache = new Dictionary<ShapeStyle, Tuple<Brush, Pen>>();
            _arcCache = new Dictionary<XArc, PathGeometry>();
            _bezierCache = new Dictionary<XBezier, PathGeometry>();
            _qbezierCache = new Dictionary<XQBezier, PathGeometry>();
            _textCache = new Dictionary<XText, Tuple<string, FormattedText>>();

            if (_biCache != null)
            {
                foreach (var kvp in _biCache)
                {
                    kvp.Value.StreamSource.Dispose();
                }
                _biCache.Clear();
            }
            _biCache = new Dictionary<Uri, BitmapImage>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        public void Draw(object dc, Container container, IList<ShapeProperty> db)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(dc, layer, db);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        public void Draw(object dc, Layer layer, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(DrawShapeState))
                {
                    shape.Draw(_dc, this, 0, 0, db);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XLine line, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = line.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(line.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(line.Style.Fill);
                stroke = CreatePen(
                    line.Style.LineStyle,
                    line.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(line.Style, Tuple.Create(fill, stroke));
            }

            double x1 = line.Start.X + dx;
            double y1 = line.Start.Y + dy;
            double x2 = line.End.X + dx;
            double y2 = line.End.Y + dy;

            var sas = line.Style.LineStyle.StartArrowStyle;
            var eas = line.Style.LineStyle.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;
            bool doRectTransform1 = a1 % 90.0 != 0.0;
            bool doRectTransform2 = a2 % 90.0 != 0.0;

            var t1 = new RotateTransform(a1, x1, y1);
            var t2 = new RotateTransform(a2, x2, y2);

            Point pt1;
            Point pt2;

            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;
    
            switch (sas.ArrowType) 
            {
                default:
                case ArrowType.None:
                    {
                        pt1 = new Point(x1, y1);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt1 = t1.Transform(new Point(x1 - sizeX1, y1));
                        var rect = new Rect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        if (doRectTransform1)
                        {
                            _dc.PushTransform(t1);
                            DrawRectangleInternal(_dc, half, fill, stroke, sas.IsFilled, ref rect);
                            _dc.Pop();
                        }
                        else
                        {
                            var bounds = t1.TransformBounds(rect);
                            DrawRectangleInternal(_dc, half, fill, stroke, sas.IsFilled, ref bounds);
                        }
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt1 = t1.Transform(new Point(x1 - sizeX1, y1));
                        _dc.PushTransform(t1);
                        var c = new Point(x1 - radiusX1, y1);
                        DrawEllipseInternal(_dc, half, fill, stroke, sas.IsFilled, ref c, radiusX1, radiusY1);
                        _dc.Pop();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt1 = t1.Transform(new Point(x1, y1));
                        var p11 = t1.Transform(new Point(x1 - sizeX1, y1 + sizeY1));
                        var p21 = t1.Transform(new Point(x1, y1));
                        var p12 = t1.Transform(new Point(x1 - sizeX1, y1 - sizeY1));
                        var p22 = t1.Transform(new Point(x1, y1));
                        DrawLineInternal(_dc, half, stroke, ref p11, ref p21);
                        DrawLineInternal(_dc, half, stroke, ref p12, ref p22);
                    }
                    break;
            }
            
            double radiusX2 = eas.RadiusX;
            double radiusY2 = eas.RadiusY;
            double sizeX2 = 2.0 * radiusX2;
            double sizeY2 = 2.0 * radiusY2;
            
            switch (eas.ArrowType) 
            {
                default:
                case ArrowType.None:
                    {
                        pt2 = new Point(x2, y2);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt2 = t2.Transform(new Point(x2 - sizeX2, y2));
                        var rect = new Rect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        if (doRectTransform2)
                        {
                            _dc.PushTransform(t2);
                            DrawRectangleInternal(_dc, half, fill, stroke, eas.IsFilled, ref rect);
                            _dc.Pop();
                        }
                        else
                        {
                            var bounds = t2.TransformBounds(rect);
                            DrawRectangleInternal(_dc, half, fill, stroke, eas.IsFilled, ref bounds);
                        }
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt2 = t2.Transform(new Point(x2 - sizeX2, y2));
                        _dc.PushTransform(t2);
                        var c = new Point(x2 - radiusX2, y2);
                        DrawEllipseInternal(_dc, half, fill, stroke, eas.IsFilled, ref c, radiusX2, radiusY2);
                        _dc.Pop();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                         pt2 = t2.Transform(new Point(x2, y2));
                         var p11 = t2.Transform(new Point(x2 - sizeX2, y2 + sizeY2));
                         var p21 = t2.Transform(new Point(x2, y2));
                         var p12 = t2.Transform(new Point(x2 - sizeX2, y2 - sizeY2));
                         var p22 = t2.Transform(new Point(x2, y2));
                         DrawLineInternal(_dc, half, stroke, ref p11, ref p21);
                         DrawLineInternal(_dc, half, stroke, ref p12, ref p22);
                    }
                    break;
            }

            DrawLineInternal(_dc, half, stroke, ref pt1, ref pt2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XRectangle rectangle, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = rectangle.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(rectangle.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(rectangle.Style.Fill);
                stroke = CreatePen(
                    rectangle.Style.LineStyle,
                    rectangle.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(rectangle.Style, Tuple.Create(fill, stroke));
            }
            
            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

            DrawRectangleInternal(_dc, half, fill, stroke, rectangle.IsFilled, ref rect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XEllipse ellipse, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = ellipse.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(ellipse.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(ellipse.Style.Fill);
                stroke = CreatePen(
                    ellipse.Style.LineStyle,
                    ellipse.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(ellipse.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;
            var center = new Point(rect.X + rx, rect.Y + ry);

            DrawEllipseInternal(
                _dc, 
                half, 
                fill, stroke, 
                ellipse.IsFilled, 
                ref center,
                rx, ry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XArc arc, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = arc.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache
                && _styleCache.TryGetValue(arc.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(arc.Style.Fill);
                stroke = CreatePen(
                    arc.Style.LineStyle,
                    arc.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(arc.Style, Tuple.Create(fill, stroke));
            }

            var a = WpfArc.FromXArc(arc, dx, dy);

            PathGeometry pg;
            if (_enableArcCache
                && _arcCache.TryGetValue(arc, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(a.Start.X, a.Start.Y);
                pf.IsFilled = arc.IsFilled;
                var segment = pf.Segments[0] as ArcSegment;
                segment.Point = new Point(a.End.X, a.End.Y);
                segment.Size = new Size(a.Radius.Width, a.Radius.Height);
                segment.IsLargeArc = a.IsLargeArc;
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(a.Start.X, a.Start.Y),
                    IsFilled = arc.IsFilled
                };

                var segment = new ArcSegment(
                    new Point(a.End.X, a.End.Y),
                    new Size(a.Radius.Width, a.Radius.Height), 
                    0.0, 
                    a.IsLargeArc, SweepDirection.Clockwise, 
                    true);

                //segment.Freeze();
                pf.Segments.Add(segment);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableArcCache)
                    _arcCache.Add(arc, pg);
            }

            DrawPathGeometryInternal(_dc, half, fill, stroke, arc.IsFilled, pg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XBezier bezier, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = bezier.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(bezier.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(bezier.Style.Fill);
                stroke = CreatePen(
                    bezier.Style.LineStyle,
                    bezier.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(bezier.Style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg;
            if (_enableBezierCache 
                && _bezierCache.TryGetValue(bezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(bezier.Point1.X + dx, bezier.Point1.Y + dy);
                pf.IsFilled = bezier.IsFilled;
                var bs = pf.Segments[0] as BezierSegment;
                bs.Point1 = new Point(bezier.Point2.X + dx, bezier.Point2.Y + dy);
                bs.Point2 = new Point(bezier.Point3.X + dx, bezier.Point3.Y + dy);
                bs.Point3 = new Point(bezier.Point4.X + dx, bezier.Point4.Y + dy);
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(bezier.Point1.X + dx, bezier.Point1.Y + dy),
                    IsFilled = bezier.IsFilled
                };
                var bs = new BezierSegment(
                        new Point(bezier.Point2.X + dx, bezier.Point2.Y + dy),
                        new Point(bezier.Point3.X + dx, bezier.Point3.Y + dy),
                        new Point(bezier.Point4.X + dx, bezier.Point4.Y + dy),
                        true);
                //bs.Freeze();
                pf.Segments.Add(bs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableBezierCache)
                    _bezierCache.Add(bezier, pg);
            }

            DrawPathGeometryInternal(_dc, half, fill, stroke, bezier.IsFilled, pg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XQBezier qbezier, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = qbezier.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(qbezier.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(qbezier.Style.Fill);
                stroke = CreatePen(
                    qbezier.Style.LineStyle,
                    qbezier.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(qbezier.Style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg;

            if (_enableQBezierCache 
                && _qbezierCache.TryGetValue(qbezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(qbezier.Point1.X + dx, qbezier.Point1.Y + dy);
                pf.IsFilled = qbezier.IsFilled;
                var qbs = pf.Segments[0] as QuadraticBezierSegment;
                qbs.Point1 = new Point(qbezier.Point2.X + dx, qbezier.Point2.Y + dy);
                qbs.Point2 = new Point(qbezier.Point3.X + dx, qbezier.Point3.Y + dy);
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(qbezier.Point1.X + dx, qbezier.Point1.Y + dy),
                    IsFilled = qbezier.IsFilled
                };

                var qbs = new QuadraticBezierSegment(
                        new Point(qbezier.Point2.X + dx, qbezier.Point2.Y + dy),
                        new Point(qbezier.Point3.X + dx, qbezier.Point3.Y + dy),
                        true);
                //bs.Freeze();
                pf.Segments.Add(qbs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();
                if (_enableQBezierCache)
                    _qbezierCache.Add(qbezier, pg);
            }

            DrawPathGeometryInternal(_dc, half, fill, stroke, qbezier.IsFilled, pg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XText text, double dx, double dy, IList<ShapeProperty> db)
        {
            var _dc = dc as DrawingContext;

            double thickness = text.Style.Thickness / _zoom;
            double half = thickness / 2.0;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(text.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(text.Style.Fill);
                stroke = CreatePen(
                    text.Style.LineStyle,
                    text.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(text.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            DrawRectangleInternal(_dc, half, fill, null, text.IsFilled, ref rect);

            Tuple<string, FormattedText> tcache;
            FormattedText ft;
            string ct;
            if (_enableTextCache
                && _textCache.TryGetValue(text, out tcache) 
                && string.Compare(tcache.Item1, text.Text) == 0)
            {
                ct = tcache.Item1;
                ft = tcache.Item2;

                _dc.DrawText(
                    ft, 
                    GetTextOrigin(text.Style, ref rect, ft));
            }
            else
            {
                var ci = CultureInfo.InvariantCulture;

                var fontStyle = System.Windows.FontStyles.Normal;
                if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Italic))
                {
                    fontStyle = System.Windows.FontStyles.Italic;
                }

                var fontWeight = FontWeights.Regular;
                if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Bold))
                {
                    fontWeight = FontWeights.Bold;
                }

                ft = new FormattedText(
                    text.Bind(db),
                    ci,
                    ci.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                    new Typeface(
                        new FontFamily(text.Style.TextStyle.FontName),
                        fontStyle,
                        fontWeight,
                        FontStretches.Normal),
                    text.Style.TextStyle.FontSize,
                    stroke.Brush, null, TextFormattingMode.Ideal);

                if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline)
                    || text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
                {
                    var decorations = new TextDecorationCollection();

                    if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline))
                    {
                        decorations = new TextDecorationCollection(
                            decorations.Union(TextDecorations.Underline));
                    }

                    if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
                    {
                        decorations = new TextDecorationCollection(
                            decorations.Union(TextDecorations.Strikethrough));
                    }

                    ft.SetTextDecorations(decorations);
                }

                if (_enableTextCache)
                {
                    if (_textCache.ContainsKey(text))
                    {
                        _textCache[text] = Tuple.Create((string)text.Text.Clone(), ft);
                    }
                    else
                    {
                        _textCache.Add(text, Tuple.Create((string)text.Text.Clone(), ft));
                    }
                }

                _dc.DrawText(
                    ft, 
                    GetTextOrigin(text.Style, ref rect, ft));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object dc, XImage image, double dx, double dy, IList<ShapeProperty> db)
        {
            if (image.Path == null)
                return;

            var _dc = dc as DrawingContext;

            double thickness = image.Style.Thickness / _zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache
                && _styleCache.TryGetValue(image.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(image.Style.Fill);
                stroke = CreatePen(
                    image.Style.LineStyle,
                    image.Style.Stroke,
                    thickness);

                if (_enableStyleCache)
                    _styleCache.Add(image.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            DrawRectangleInternal(_dc, half, fill, null, image.IsFilled, ref rect);

            if (_enableImageCache
                && _biCache.ContainsKey(image.Path))
            {
                _dc.DrawImage(_biCache[image.Path], rect);
            }
            else
            {
                if (!image.Path.IsAbsoluteUri || !System.IO.File.Exists(image.Path.LocalPath))
                    return;

                byte[] buffer = System.IO.File.ReadAllBytes(image.Path.LocalPath);
                var ms = new System.IO.MemoryStream(buffer);
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
                _biCache[image.Path] = bi;

                _dc.DrawImage(_biCache[image.Path], rect);
            }
        }
    }
}
