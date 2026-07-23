namespace ASTEM_DB.Tools;

public class DeleteTool : Tool
{
    public DeleteTool(
        ToolSystem toolSystem)
        : base(toolSystem, "Delete")
    {
    }

    public void Execute()
    {
        ToolSystem.RemoveSelectedAnnotations();
    }
}