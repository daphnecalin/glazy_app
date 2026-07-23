using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;

namespace ASTEM_DB.Tools;

public class PanTool : Tool
{
    private readonly Canvas _drawingCanvas;

    private bool _isPanning;
    private Point? _lastPosition;

    public PanTool(
        ToolSystem toolSystem,
        Canvas drawingCanvas)
        : base(toolSystem, "Pan")
    {
        _drawingCanvas = drawingCanvas;
    }

    public override void OnPointerPressed(
        PointerPressedEventArgs e)
    {
        var currentPoint =
            e.GetCurrentPoint(_drawingCanvas);

        if (!currentPoint.Properties
            .IsLeftButtonPressed)
        {
            return;
        }

        _isPanning = true;
        _lastPosition =
            e.GetPosition(_drawingCanvas);

        e.Pointer.Capture(_drawingCanvas);
        e.Handled = true;
    }

    public override void OnPointerMoved(
        PointerEventArgs e)
    {
        if (!_isPanning ||
            _lastPosition is null)
        {
            return;
        }

        var currentPosition =
            e.GetPosition(_drawingCanvas);

        var dx =
            currentPosition.X -
            _lastPosition.Value.X;

        var dy =
            currentPosition.Y -
            _lastPosition.Value.Y;

        var viewport =
            ToolSystem.Viewport;

        ToolSystem.SetViewport(
            viewport with
            {
                X =
                    viewport.X +
                    dx / viewport.Scale,

                Y =
                    viewport.Y +
                    dy / viewport.Scale
            });

        _lastPosition = currentPosition;

        e.Handled = true;
    }

    public override void OnPointerReleased(
        PointerReleasedEventArgs e)
    {
        StopPanning();

        e.Pointer.Capture(null);
        e.Handled = true;
    }

    public override void OnPointerExited(
        PointerEventArgs e)
    {
        StopPanning();
    }

    public override void OnPointerWheelChanged(
        PointerWheelEventArgs e)
    {
        var oldViewport =
            ToolSystem.Viewport;

        var oldScale =
            oldViewport.Scale;

        var zoomFactor = 1.1;

        var newScale =
            e.Delta.Y > 0
                ? oldScale * zoomFactor
                : oldScale / zoomFactor;

        newScale = Math.Clamp(
            newScale,
            0.05,
            10);

        var mouseScreen =
            e.GetPosition(_drawingCanvas);

        var canvasRect = new Rect(
            0,
            0,
            _drawingCanvas.Bounds.Width,
            _drawingCanvas.Bounds.Height);

        var mouseWorld =
            CoordinateHelpers.ScreenToWorld(
                mouseScreen,
                oldViewport,
                canvasRect);

        var newX =
            mouseScreen.X / newScale -
            mouseWorld.X;

        var newY =
            mouseScreen.Y / newScale -
            mouseWorld.Y;

        ToolSystem.SetViewport(
            new ViewportState(
                newX,
                newY,
                newScale));

        e.Handled = true;
    }

    private void StopPanning()
    {
        _isPanning = false;
        _lastPosition = null;
    }
}