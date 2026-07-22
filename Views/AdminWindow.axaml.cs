using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace ASTEM_DB.Views;

public partial class AdminWindow : Window
{
    private ToggleButton? _selectedToolButton;
    private ToggleButton? _previousToolButton;
    private bool _spaceIsHeld;

    public AdminWindow()
    {
        InitializeComponent();

        SelectToolButton.IsChecked = true;
        _selectedToolButton = SelectToolButton;

        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        AddHandler(KeyUpEvent, OnWindowKeyUp, RoutingStrategies.Tunnel);
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
}