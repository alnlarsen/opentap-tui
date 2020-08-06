﻿using OpenTap;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTap.TUI;
using Terminal.Gui;

namespace OpenTAP.TUI.PropEditProviders
{
    public class BooleanEditProvider : IPropEditProvider
    {
        public int Order => 100;
        public View Edit(AnnotationCollection annotation)
        {
            var booledit = annotation.Get<IObjectValueAnnotation>();
            if (booledit == null || annotation.Get<IMemberAnnotation>()?.ReflectionInfo != TypeData.FromType(typeof(bool))) return null;

            var check = new CheckBox(annotation.Get<DisplayAttribute>()?.Name ?? "...", (bool)booledit.Value);
            check.Toggled += b => 
            {
                try
                {
                    booledit.Value = check.Checked;
                }
                catch (Exception exception)
                {
                    TUI.Log.Error(exception.Message);
                    TUI.Log.Debug(exception);
                }
            };

            return check;
        }
    }
}
