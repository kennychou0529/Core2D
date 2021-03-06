﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using System.Collections.Generic;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPathGeometryTests
    {
        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Figures_Not_Null()
        {
            var target = new XPathGeometry();
            Assert.NotNull(target.Figures);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void FillRule_Set_To_Nonzero_By_Default()
        {
            var target = new XPathGeometry();
            Assert.Equal(XFillRule.Nonzero, target.FillRule);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Empty()
        {
            var geometry = new XPathGeometry();

            var target = new List<XPathFigure>();
            var actual = geometry.ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_Nonzero()
        {
            var target = new XPathGeometry();

            var actual = target.ToString();

            Assert.Equal("F1", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_EvenOdd()
        {
            var target = new XPathGeometry() { FillRule = XFillRule.EvenOdd };

            var actual = target.ToString();

            Assert.Equal("", actual);
        }
    }
}
