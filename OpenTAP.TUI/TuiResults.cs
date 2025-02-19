using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using OpenTap.Cli;
using OpenTap.Package;
using OpenTap.Tui.Views;
using OpenTap.Tui.Windows;
using Terminal.Gui;

namespace OpenTap.Tui
{
    [Display("tui-results")]
    public class TuiResults : TuiAction
    {
        public override int TuiExecute(CancellationToken cancellationToken)
        {
            var win = new ResultsViewerWindow()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            
            // Run application
            Application.Run(win);

            return 0;
        }
    }
}