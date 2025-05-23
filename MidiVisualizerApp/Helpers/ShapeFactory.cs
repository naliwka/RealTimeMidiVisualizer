﻿using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System;
using MidiVisualizerApp.Models;
using Core.MIDIProcessing.Models;

namespace MidiVisualizerApp.Helpers
{
    public static class ShapeFactory
    {
        public static Shape CreateShape(VisualElementData visual)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(visual.ColorHex));

            Shape shape = visual.Shape
                switch
            {
                "Circle" => new Ellipse
                {
                    Width = visual.Size,
                    Height = visual.Size,
                    Fill = brush,
                    Opacity = visual.Opacity
                },
                "Rectangle" => new Rectangle
                {
                    Width = visual.Size,
                    Height = visual.Size,
                    Fill = brush,
                    Opacity = visual.Opacity
                },
                _ => throw new ArgumentException("Invalid shape type")
            };
            return shape;
        }
    }
}
