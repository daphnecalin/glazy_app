using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace ASTEM_DB.Tools;

public class SelectorTool : Tool
{
    private readonly Canvas _drawingCanvas;

    public SelectorTool(
        ToolSystem toolSystem,
        Canvas drawingCanvas)
        : base(toolSystem, "Selector")
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

        string? topmostAnnotationId = null;

        for (
            var index =
                ToolSystem.Annotations.Count - 1;
            index >= 0;
            index--)
        {
            var annotation =
                ToolSystem.Annotations[index];

            if (CoordinateHelpers.InBounds(
                    worldPosition,
                    annotation.Bounds))
            {
                topmostAnnotationId =
                    annotation.Id;

                break;
            }
        }

        if (topmostAnnotationId is null)
        {
            ToolSystem.SelectAnnotations([]);
        }
        else
        {
            ToolSystem.SelectAnnotations(
                [topmostAnnotationId]);
        }

        e.Handled = true;
    }
}