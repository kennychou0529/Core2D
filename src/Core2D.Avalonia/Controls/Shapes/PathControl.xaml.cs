// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Core2D.Avalonia.Controls.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="PathControl"/> xaml.
    /// </summary>
    public class PathControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathControl"/> class.
        /// </summary>
        public PathControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void Render(DrawingContext context)
        {
            System.Diagnostics.Debug.WriteLine("Render PathControl");
            base.Render(context);
        }
    }
}
