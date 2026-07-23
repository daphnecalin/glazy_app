using Avalonia.Input;

namespace ASTEM_DB.Tools;

public abstract class Tool
{
    protected ToolSystem ToolSystem { get; }

    public string Name { get; }

    protected Tool(
        ToolSystem toolSystem,
        string name)
    {
        ToolSystem = toolSystem;
        Name = name;
    }

    public virtual void OnToolSelected()
    {
    }

    public virtual void OnPointerPressed(
        PointerPressedEventArgs e)
    {
    }

    public virtual void OnPointerMoved(
        PointerEventArgs e)
    {
    }

    public virtual void OnPointerReleased(
        PointerReleasedEventArgs e)
    {
    }

    public virtual void OnPointerWheelChanged(
        PointerWheelEventArgs e)
    {
    }

    public virtual void OnPointerExited(
        PointerEventArgs e)
    {
    }

    public virtual void OnKeyDown(
        KeyEventArgs e)
    {
    }

    public virtual void OnKeyUp(
        KeyEventArgs e)
    {
    }
}