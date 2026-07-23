using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;

namespace ASTEM_DB.Tools;

public class RectangleTool : Tool
{
    private readonly Canvas _drawingCanvas;

    private Point? _startPoint;
    private Point? _dragStartScreen;
    private Annotation? _currentAnnotation;

    private bool _isDragging;

    private const double DragThreshold = 3;

    public RectangleTool(
        ToolSystem toolSystem,
        Canvas drawingCanvas)
        : base(toolSystem, "Rectangle")
    {
        _drawingCanvas = drawingCanvas;
    }

    public override void OnToolSelected()
    {
        CancelUnfinishedAnnotation();
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

        var screenPosition =
            e.GetPosition(_drawingCanvas);

        var canvasRect = new Rect(
            0,
            0,
            _drawingCanvas.Bounds.Width,
            _drawingCanvas.Bounds.Height);

        var worldPosition =
            CoordinateHelpers.ScreenToWorld(
                screenPosition,
                ToolSystem.Viewport,
                canvasRect);

        if (_startPoint is null)
        {
            _startPoint = worldPosition;
            _dragStartScreen = screenPosition;
            _isDragging = false;

            _currentAnnotation = new Annotation(
                "rectangle",
                [_startPoint.Value, _startPoint.Value],
                [],
                ToolSystem.CurrentAnnotationClass);

            ToolSystem.AddAnnotation(
                _currentAnnotation);
        }
        else
        {
            _currentAnnotation!.Bounds =
            [
                _startPoint.Value,
                worldPosition
            ];

            FinishAnnotation();
        }

        e.Pointer.Capture(_drawingCanvas);
        e.Handled = true;
    }

    public override void OnPointerMoved(
        PointerEventArgs e)
    {
        if (_startPoint is null ||
            _currentAnnotation is null)
        {
            return;
        }

        var screenPosition =
            e.GetPosition(_drawingCanvas);

        if (_dragStartScreen is not null &&
            !_isDragging)
        {
            var dx =
                screenPosition.X -
                _dragStartScreen.Value.X;

            var dy =
                screenPosition.Y -
                _dragStartScreen.Value.Y;

            if (Math.Sqrt(
                    dx * dx +
                    dy * dy) > DragThreshold)
            {
                _isDragging = true;
            }
        }

        if (!_isDragging)
        {
            return;
        }

        var canvasRect = new Rect(
            0,
            0,
            _drawingCanvas.Bounds.Width,
            _drawingCanvas.Bounds.Height);

        var worldPosition =
            CoordinateHelpers.ScreenToWorld(
                screenPosition,
                ToolSystem.Viewport,
                canvasRect);

        _currentAnnotation.Bounds =
        [
            _startPoint.Value,
            worldPosition
        ];

        ToolSystem.NotifyStateChanged();

        e.Handled = true;
    }

    public override void OnPointerReleased(
        PointerReleasedEventArgs e)
    {
        if (!_isDragging ||
            _startPoint is null ||
            _currentAnnotation is null)
        {
            return;
        }

        var screenPosition =
            e.GetPosition(_drawingCanvas);

        var canvasRect = new Rect(
            0,
            0,
            _drawingCanvas.Bounds.Width,
            _drawingCanvas.Bounds.Height);

        var worldPosition =
            CoordinateHelpers.ScreenToWorld(
                screenPosition,
                ToolSystem.Viewport,
                canvasRect);

        _currentAnnotation.Bounds =
        [
            _startPoint.Value,
            worldPosition
        ];

        FinishAnnotation();

        e.Pointer.Capture(null);
        e.Handled = true;
    }

    public override void OnPointerExited(
        PointerEventArgs e)
    {
        CancelUnfinishedAnnotation();
    }

    private void FinishAnnotation()
    {
        _startPoint = null;
        _dragStartScreen = null;
        _currentAnnotation = null;
        _isDragging = false;

        ToolSystem.NotifyStateChanged();
    }

    private void CancelUnfinishedAnnotation()
    {
        if (_currentAnnotation is not null)
        {
            ToolSystem.RemoveAnnotation(
                _currentAnnotation.Id);
        }

        _startPoint = null;
        _dragStartScreen = null;
        _currentAnnotation = null;
        _isDragging = false;
    }
}