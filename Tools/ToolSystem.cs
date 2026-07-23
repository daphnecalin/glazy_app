using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASTEM_DB.Tools;

public class ToolSystem
{
    public List<Tool> Tools { get; } = [];

    public List<Annotation> Annotations { get; } = [];

    public List<string> SelectedAnnotationIds { get; } = [];

    public Tool? CurrentTool { get; private set; }

    public Tool? PreviousTool { get; private set; }

    public ViewportState Viewport { get; private set; } =
        new(0, 0, 1);

    public string CurrentAnnotationClass { get; set; } =
        "Default";

    public event Action? StateChanged;

    public event Action<Tool?>? CurrentToolChanged;

    public void RegisterTool(Tool tool)
    {
        Tools.Add(tool);

        if (CurrentTool is null)
        {
            SetCurrentTool(tool);
        }
    }

    public T? GetTool<T>()
        where T : Tool
    {
        return Tools.OfType<T>().FirstOrDefault();
    }

    public Tool? GetTool(string name)
    {
        return Tools.FirstOrDefault(
            tool => tool.Name == name);
    }

    public void SetCurrentTool(Tool tool)
    {
        if (CurrentTool == tool)
        {
            return;
        }

        CurrentTool = tool;
        tool.OnToolSelected();

        CurrentToolChanged?.Invoke(tool);
    }

    public void SetViewport(ViewportState viewport)
    {
        var scale = Math.Clamp(
            viewport.Scale,
            0.05,
            10);

        Viewport = viewport with
        {
            Scale = scale
        };

        NotifyStateChanged();
    }

    public void AddAnnotation(Annotation annotation)
    {
        Annotations.Add(annotation);
        NotifyStateChanged();
    }

    public void RemoveAnnotation(string annotationId)
    {
        Annotations.RemoveAll(
            annotation => annotation.Id == annotationId);

        SelectedAnnotationIds.Remove(annotationId);

        NotifyStateChanged();
    }

    public void RemoveSelectedAnnotations()
    {
        if (SelectedAnnotationIds.Count == 0)
        {
            return;
        }

        var selectedIds =
            SelectedAnnotationIds.ToHashSet();

        Annotations.RemoveAll(
            annotation =>
                selectedIds.Contains(annotation.Id));

        SelectedAnnotationIds.Clear();

        NotifyStateChanged();
    }

    public void SelectAnnotations(
        IEnumerable<string> annotationIds)
    {
        SelectedAnnotationIds.Clear();
        SelectedAnnotationIds.AddRange(annotationIds);

        NotifyStateChanged();
    }

    public void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }

    public void HandlePointerPressed(
        PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(null);

        if (point.Properties
            .PointerUpdateKind ==
            PointerUpdateKind.RightButtonPressed)
        {
            GetTool<DeleteTool>()?.Execute();
            e.Handled = true;
            return;
        }

        CurrentTool?.OnPointerPressed(e);
    }

    public void HandlePointerMoved(
        PointerEventArgs e)
    {
        CurrentTool?.OnPointerMoved(e);
    }

    public void HandlePointerReleased(
        PointerReleasedEventArgs e)
    {
        CurrentTool?.OnPointerReleased(e);
    }

    public void HandlePointerWheelChanged(
        PointerWheelEventArgs e)
    {
        CurrentTool?.OnPointerWheelChanged(e);
    }

    public void HandlePointerExited(
        PointerEventArgs e)
    {
        CurrentTool?.OnPointerExited(e);
    }

    public void HandleKeyDown(
        KeyEventArgs e)
    {
        if (e.Key == Key.Space &&
            !e.KeyModifiers.HasFlag(
                KeyModifiers.Control) &&
            !e.KeyModifiers.HasFlag(
                KeyModifiers.Meta))
        {
            var panTool = GetTool<PanTool>();

            if (panTool is not null &&
                CurrentTool != panTool &&
                PreviousTool is null)
            {
                PreviousTool = CurrentTool;
                SetCurrentTool(panTool);
            }

            e.Handled = true;
        }

        CurrentTool?.OnKeyDown(e);
    }

    public void HandleKeyUp(
        KeyEventArgs e)
    {
        if (e.Key == Key.Space &&
            PreviousTool is not null)
        {
            var previousTool = PreviousTool;
            PreviousTool = null;

            SetCurrentTool(previousTool);

            e.Handled = true;
        }

        CurrentTool?.OnKeyUp(e);
    }
}