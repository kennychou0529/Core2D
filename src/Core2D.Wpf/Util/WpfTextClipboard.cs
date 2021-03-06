﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System.Threading.Tasks;
using System.Windows;

namespace Core2D.Wpf
{
    /// <summary>
    /// Wrapper class for <see cref="System.Windows.Clipboard"/> clipboard class.
    /// </summary>
    public sealed class WpfTextClipboard : ITextClipboard
    {
        /// <summary>
        /// Set clipboard text.
        /// </summary>
        /// <param name="text">The text string.</param>
        Task ITextClipboard.SetText(string text)
        {
            return Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Clipboard.SetText(text, TextDataFormat.UnicodeText);
                });
            });
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        Task<string> ITextClipboard.GetText()
        {
            return Task.Run(() =>
            {
                return App.Current.Dispatcher.Invoke(() =>
                {
                    return Clipboard.GetText(TextDataFormat.UnicodeText);
                });
            });
        }

        /// <summary>
        /// Return true if clipboard contains text string.
        /// </summary>
        /// <returns>True if clipboard contains text string.</returns>
        Task<bool> ITextClipboard.ContainsText()
        {
            return Task.Run(() =>
            {
                return App.Current.Dispatcher.Invoke(() =>
                {
                    return Clipboard.ContainsText(TextDataFormat.UnicodeText);
                });
            });
        }
    }
}
