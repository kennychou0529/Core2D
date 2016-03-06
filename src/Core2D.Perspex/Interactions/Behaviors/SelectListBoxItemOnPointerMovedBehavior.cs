﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex.Controls;
using Perspex.Input;
using Perspex.Xaml.Interactivity;

namespace Core2D.Perspex.Interactions.Behaviors
{
    public class SelectListBoxItemOnPointerMovedBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerMoved += PointerMoved;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerMoved -= PointerMoved;
        }

        private void PointerMoved(object sender, PointerEventArgs args)
        {
            var listBoxItem = AssociatedObject.Parent as ListBoxItem;
            if (listBoxItem != null)
            {
                listBoxItem.IsSelected = true;
                listBoxItem.Focus();
            }
        }
    }
}