﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Terminal.Gui;

namespace OpenTap.Tui
{
    public class PlotView : View
    {
        private List<Plot> Plots { get; set; } = new List<Plot>();
        private char[] Symbols = { '#', '+', '*', '\u25c6' };

        public void Plot(Plot plot)
        {
            Plots.Add(plot);
        }
        
        public override void Redraw(Rect bounds)
        {
            base.Redraw(bounds);

            if (Plots.Any() == false)
                return;
            
            Driver.SetAttribute(ColorScheme.Normal);
            
            // Find max values for scaling
            var allXValues = Plots.SelectMany(p => p.Points.Keys).ToList();
            var allYValues = Plots.SelectMany(p => p.Points.Values).ToList();
            var minX = allXValues.Min();
            var maxX = allXValues.Max();
            var minY = allYValues.Min();
            var maxY = allYValues.Max();
            
            // Set X and Y axis values
            var YWidth = Math.Max(((int)maxY).ToString().Length, ((int)minY).ToString().Length);
            // Size of box
            var size = new Rect(1 + YWidth, 0, Frame.Width - YWidth, Frame.Height - 2);
            
            DrawYAxis(minY, maxY, YWidth, size);
            DrawXAxis(minX, maxX, size);
            
            
            for (int i = 0; i < Plots.Count; i++)
            {
                var scaledPlot = new ScaledPlot(Plots[i], size, minX, maxX, minY, maxY);
                DrawPlot(scaledPlot, Symbols[i % Symbols.Length], size);
            }
        }

        private const int YLINECOUNT = 5;
        void DrawYAxis(double min, double max, int axisWidth, Rect size)
        {
            var difference = max - min;
            var interval = difference / size.Height;

            for (int i = 0; i < size.Height; i++)
            {
                if (i % YLINECOUNT == 0) // Every 5 lines print y value
                {
                    var num = string.Format("{0," + axisWidth + "}", Math.Round(max - i * interval));
                    
                    for (int j = 0; j < num.Length; j++)
                    {
                        Move(j, i);
                        Driver.AddRune(num[j]);
                    }
                    Move(num.Length, i);
                    Driver.AddRune(Driver.RightTee);
                }
                else
                {
                    Move(axisWidth, i);
                    Driver.AddRune(Driver.VLine);
                }
            }
        }

        private const int XLINECOUNT = 15;
        void DrawXAxis(double min, double max, Rect size)
        {
            var difference = max - min;
            var interval = difference / size.Width;

            for (var i = size.Width; i > size.X; i--)
            {
                if (i % XLINECOUNT == 0) // Every 15 lines print x value
                {
                    var num = Math.Round(min + i * interval).ToString();
                    
                    for (var j = 0; j < num.Length; j++)
                    {
                        Move(i + j - num.Length/2, size.Height + 1);
                        Driver.AddRune(num[j]);
                    }
                    Move(i, size.Height);
                    Driver.AddRune(Driver.TopTee);
                }
                else
                {
                    Move(i, size.Height);
                    Driver.AddRune(Driver.HLine);
                }
            }
        }

        void DrawPlot(ScaledPlot plot, char symbol, Rect size)
        {
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++) 
                {
                    var material = plot.IsPoint(x, y);
                    if (material)
                    {
                        Move(x, y);

                        var rune = new Rune(symbol);
                        Driver.AddRune(rune);
                    }
                }
            }
        }
    }

    public class Plot
    {
        public Dictionary<double, double> Points { get; set; }
        
        public Plot()
        {
            Points = new Dictionary<double, double>();
        }
    }

    public class ScaledPlot
    {
        private Dictionary<int, int> Points { get; set; }

        public bool IsPoint(int x, int y)
        {
            return Points.Contains(new KeyValuePair<int, int>(x, y));
        }
        
        public ScaledPlot(Plot plot, Rect size, double minX, double maxX, double minY, double maxY)
        {
            Points = new Dictionary<int, int>();
            
            foreach (var point in plot.Points)
            {
                var value = maxY - point.Value + minY;
                var x = (int) Math.Round(size.X + (size.Width - size.X - 1) * ((point.Key - minX) / (maxX - minX)));
                var y = (int) Math.Round(size.Y + (size.Height - size.Y - 1) * ((value-minY) / (maxY - minY)));
                Points[x] = y;
            }
        }
    }
}