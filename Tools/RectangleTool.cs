using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace ASTEM_DB.Tools;

public class RectangleTool : Tool
{
    private readonly Canvas _drawingCanvas;

    private Point _startPoint;
    private Rectangle? _activeRectangle;
    private bool _isDrawingRectangle;

    public RectangleTool(
        ToolSystem toolSystem,
        Canvas drawingCanvas)
        : base(toolSystem, "Rectangle")
    {
        _drawingCanvas = drawingCanvas;
    }

    public override void OnPointerPressed(
        PointerPressedEventArgs e)
    {
        var properties =
            e.GetCurrentPoint(_drawingCanvas).Properties;

        if (!properties.IsLeftButtonPressed)
        {
            return;
        }

        _startPoint = e.GetPosition(_drawingCanvas);
        _isDrawingRectangle = true;

        _activeRectangle = new Rectangle
        {
            Width = 0,
            Height = 0,
            Stroke = Brushes.Red,
            StrokeThickness = 2,
            Fill = Brushes.Transparent
        };

        Canvas.SetLeft(
            _activeRectangle,
            _startPoint.X);

        Canvas.SetTop(
            _activeRectangle,
            _startPoint.Y);

        _drawingCanvas.Children.Add(_activeRectangle);

        e.Pointer.Capture(_drawingCanvas);
        e.Handled = true;
    }

    public override void OnPointerMoved(
        PointerEventArgs e)
    {
        if (!_isDrawingRectangle ||
            _activeRectangle is null)
        {
            return;
        }

        var currentPoint =
            e.GetPosition(_drawingCanvas);

        var left = Math.Min(
            _startPoint.X,
            currentPoint.X);

        var top = Math.Min(
            _startPoint.Y,
            currentPoint.Y);

        var width = Math.Abs(
            currentPoint.X - _startPoint.X);

        var height = Math.Abs(
            currentPoint.Y - _startPoint.Y);

        Canvas.SetLeft(_activeRectangle, left);
        Canvas.SetTop(_activeRectangle, top);

        _activeRectangle.Width = width;
        _activeRectangle.Height = height;

        e.Handled = true;
    }

    public override void OnPointerReleased(
        PointerReleasedEventArgs e)
    {
        if (!_isDrawingRectangle)
        {
            return;
        }

        _isDrawingRectangle = false;
        _activeRectangle = null;

        e.Pointer.Capture(null);
        e.Handled = true;
    }
}
