﻿using OpenTap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terminal.Gui;

namespace OpenTap.Tui.PropEditProviders
{
    public interface IPropEditProvider
    {
        /// <summary>
        /// Ranks the provider.
        /// </summary>
        int Order { get; }
        /// <summary>
        /// Create the view.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        View Edit(AnnotationCollection annotation);
    }
}
