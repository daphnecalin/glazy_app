using ASTEM_DB.Tools;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ASTEM_DB.Views;

public partial class AdminWindow : Window
{
    private ToggleButton? _selectedToolButton;
    private ToggleButton? _previousToolButton;
    private bool _spaceIsHeld;
    private readonly ToolSystem _toolSystem;
    private readonly RectangleTool _rectangleTool;

    public AdminWindow()
{
    InitializeComponent();

    _toolSystem = new ToolSystem();
    _rectangleTool = new RectangleTool(
        _toolSystem,
        DrawingCanvas);

    _toolSystem.RegisterTool(_rectangleTool);

    CanvasImage.RenderTransform = _imageScaleTransform;

    SelectToolButton.IsChecked = true;
    _selectedToolButton = SelectToolButton;

    AddHandler(
        KeyDownEvent,
        OnWindowKeyDown,
        RoutingStrategies.Tunnel);

    AddHandler(
        KeyUpEvent,
        OnWindowKeyUp,
        RoutingStrategies.Tunnel);
}

    private void OnToolButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton clickedButton)
        {
            return;
        }

        SelectTool(clickedButton);
    }

    private void SelectTool(ToggleButton toolButton)
    {
        if (_selectedToolButton != null &&
            _selectedToolButton != toolButton)
        {
            _selectedToolButton.IsChecked = false;
        }

        toolButton.IsChecked = true;
        _selectedToolButton = toolButton;
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        var commandPressed =
        e.KeyModifiers.HasFlag(KeyModifiers.Control) ||
        e.KeyModifiers.HasFlag(KeyModifiers.Meta);

    if (commandPressed)
        {
            bool zoomIn =
                e.Key is Key.Add or Key.OemPlus ||
                e.KeySymbol is "+" or "=" ||
        (
            e.Key == Key.OemSemicolon &&
            e.KeyModifiers.HasFlag(KeyModifiers.Shift)
        );

            bool zoomOut =
                e.Key is Key.Subtract or Key.OemMinus &&
                !e.KeyModifiers.HasFlag(KeyModifiers.Shift);

            if (zoomIn)
            {
        SetZoom(_zoomLevel + 0.1);
        e.Handled = true;
        return;
    }

    if (zoomOut)
    {
        SetZoom(_zoomLevel - 0.1);
        e.Handled = true;
        return;
    }

    if (e.Key is Key.D0 or Key.NumPad0 || e.KeySymbol == "0")
    {
        SetZoom(1.0);
        e.Handled = true;
        return;
    }
}
        if (e.Key != Key.Space || _spaceIsHeld)
        {
            return;
        }

        _spaceIsHeld = true;
        _previousToolButton = _selectedToolButton;

        SelectTool(PanToolButton);
        e.Handled = true;
    }

    private void OnWindowKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Space)
        {
            return;
        }

        _spaceIsHeld = false;

        if (_previousToolButton != null)
        {
            SelectTool(_previousToolButton);
            _previousToolButton = null;
        }

        e.Handled = true;
    }

    private void OnDeleteClicked(object? sender, RoutedEventArgs e)
    {
        // Delete logic will be added later.
    }
    private async void OnOpenImageClicked(object? sender, RoutedEventArgs e)
{
    var files = await StorageProvider.OpenFilePickerAsync(
        new FilePickerOpenOptions
        {
            Title = "Open Image",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Image files")
                {
                    Patterns =
                    [
                        "*.png",
                        "*.jpg",
                        "*.jpeg",
                        "*.bmp",
                        "*.webp"
                    ]
                }
            ]
        });

    if (files.Count == 0)
    {
        return;
    }

    await using var stream = await files[0].OpenReadAsync();

    CanvasImage.Source = new Bitmap(stream);
    CanvasImage.IsVisible = true;
    CanvasPlaceholder.IsVisible = false;
}

private void OnCloseImageClicked(object? sender, RoutedEventArgs e)
{
    CanvasImage.Source = null;
    CanvasImage.IsVisible = false;
    CanvasPlaceholder.IsVisible = true;
}

private void OnExitClicked(object? sender, RoutedEventArgs e)
{
    Close();
}
private double _zoomLevel = 1.0;

private void OnZoomInClicked(object? sender, RoutedEventArgs e)
{
    SetZoom(_zoomLevel + 0.1);
}

private void OnZoomOutClicked(object? sender, RoutedEventArgs e)
{
    SetZoom(_zoomLevel - 0.1);
}

private void OnResetZoomClicked(object? sender, RoutedEventArgs e)
{
    SetZoom(1.0);
}

private void SetZoom(double zoom)
{
    _zoomLevel = Math.Clamp(zoom, 0.2, 5.0);

    _imageScaleTransform.ScaleX = _zoomLevel;
    _imageScaleTransform.ScaleY = _zoomLevel;
}
private readonly ScaleTransform _imageScaleTransform = new()
{
    ScaleX = 1,
    ScaleY = 1
};
private void OnCanvasPointerWheelChanged(
    object? sender,
    PointerWheelEventArgs e)
{
    var commandPressed =
        e.KeyModifiers.HasFlag(KeyModifiers.Control) ||
        e.KeyModifiers.HasFlag(KeyModifiers.Meta);

    if (!commandPressed)
    {
        return;
    }

    if (e.Delta.Y > 0)
    {
        SetZoom(_zoomLevel + 0.1);
    }
    else if (e.Delta.Y < 0)
    {
        SetZoom(_zoomLevel - 0.1);
    }

    e.Handled = true;
}
private void OnCanvasPointerPressed(
    object? sender,
    PointerPressedEventArgs e)
{
    if (_selectedToolButton == RectangleToolButton)
    {
        _toolSystem.HandlePointerPressed(e);
    }
}

private void OnCanvasPointerMoved(
    object? sender,
    PointerEventArgs e)
{
    if (_selectedToolButton == RectangleToolButton)
    {
        _toolSystem.HandlePointerMoved(e);
    }
}

private void OnCanvasPointerReleased(
    object? sender,
    PointerReleasedEventArgs e)
{
    if (_selectedToolButton == RectangleToolButton)
    {
        _toolSystem.HandlePointerReleased(e);
    }
}
}
