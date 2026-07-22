using System.Collections.Generic;
using Avalonia.Input;

namespace ASTEM_DB.Tools;

public class ToolSystem
{
    public List<Tool> Tools { get; } = [];

    public Tool? CurrentTool { get; private set; }

    public Tool? PreviousTool { get; private set; }

    public ViewportState Viewport { get; set; } =
        new(0, 0, 1);

    public void RegisterTool(Tool tool)
    {
        Tools.Add(tool);

        if (CurrentTool is null)
        {
            CurrentTool = tool;
            tool.OnToolSelected();
        }
    }

    public void SetCurrentTool(Tool tool)
    {
        if (CurrentTool == tool)
        {
            return;
        }

        PreviousTool = CurrentTool;
        CurrentTool = tool;

        tool.OnToolSelected();
    }

    public void HandlePointerPressed(PointerPressedEventArgs e)
    {
        CurrentTool?.OnPointerPressed(e);
    }

    public void HandlePointerMoved(PointerEventArgs e)
    {
        CurrentTool?.OnPointerMoved(e);
    }

    public void HandlePointerReleased(PointerReleasedEventArgs e)
    {
        CurrentTool?.OnPointerReleased(e);
    }

    public void HandlePointerWheelChanged(PointerWheelEventArgs e)
    {
        CurrentTool?.OnPointerWheelChanged(e);
    }

    public void HandleKeyDown(KeyEventArgs e)
    {
        CurrentTool?.OnKeyDown(e);
    }

    public void HandleKeyUp(KeyEventArgs e)
    {
        CurrentTool?.OnKeyUp(e);
    }
}
