using ASTEM_DB.Tools;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System;
using System.Linq;

namespace ASTEM_DB.Views;

public partial class AdminWindow : Window
{
    private ToggleButton? _selectedToolButton;

    private readonly ToolSystem _toolSystem;

    private readonly PanTool _panTool;
    private readonly SelectorTool _selectorTool;
    private readonly RectangleTool _rectangleTool;
    private readonly DeleteTool _deleteTool;

    private readonly ScaleTransform
        _imageScaleTransform = new();

    private readonly TranslateTransform
        _imageTranslateTransform = new();

    public AdminWindow()
    {
        InitializeComponent();

        var imageTransform =
            new TransformGroup();

        imageTransform.Children.Add(
            _imageScaleTransform);

        imageTransform.Children.Add(
            _imageTranslateTransform);

        CanvasImage.RenderTransform =
            imageTransform;

        _toolSystem = new ToolSystem();

        _panTool = new PanTool(
            _toolSystem,
            DrawingCanvas);

        _selectorTool = new SelectorTool(
            _toolSystem,
            DrawingCanvas);

        _rectangleTool =
            new RectangleTool(
                _toolSystem,
                DrawingCanvas);

        _deleteTool =
            new DeleteTool(_toolSystem);

        _toolSystem.RegisterTool(_panTool);
        _toolSystem.RegisterTool(_selectorTool);
        _toolSystem.RegisterTool(_rectangleTool);
        _toolSystem.RegisterTool(_deleteTool);

        _toolSystem.StateChanged +=
            RenderAnnotations;

        _toolSystem.CurrentToolChanged +=
            OnCurrentToolChanged;

        _toolSystem.SetCurrentTool(
            _selectorTool);

        AddHandler(
            KeyDownEvent,
            OnWindowKeyDown,
            RoutingStrategies.Tunnel);

        AddHandler(
            KeyUpEvent,
            OnWindowKeyUp,
            RoutingStrategies.Tunnel);

        RenderAnnotations();
    }

    private void OnCurrentToolChanged(
        Tool? tool)
    {
        ToggleButton? button =
            tool switch
            {
                PanTool =>
                    PanToolButton,

                SelectorTool =>
                    SelectToolButton,

                RectangleTool =>
                    RectangleToolButton,

                _ => null
            };

        if (button is not null)
        {
            SelectToolButtonInUi(button);
        }
    }

    private void OnToolButtonClicked(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender == PanToolButton)
        {
            _toolSystem.SetCurrentTool(
                _panTool);
        }
        else if (sender == SelectToolButton)
        {
            _toolSystem.SetCurrentTool(
                _selectorTool);
        }
        else if (sender ==
                 RectangleToolButton)
        {
            _toolSystem.SetCurrentTool(
                _rectangleTool);
        }
    }

    private void SelectToolButtonInUi(
        ToggleButton toolButton)
    {
        if (_selectedToolButton is not null &&
            _selectedToolButton != toolButton)
        {
            _selectedToolButton.IsChecked =
                false;
        }

        toolButton.IsChecked = true;
        _selectedToolButton = toolButton;
    }

    private void OnWindowKeyDown(
        object? sender,
        KeyEventArgs e)
    {
        var commandPressed =
            e.KeyModifiers.HasFlag(
                KeyModifiers.Control) ||
            e.KeyModifiers.HasFlag(
                KeyModifiers.Meta);

        if (commandPressed)
        {
            var zoomIn =
                e.Key is Key.Add or
                    Key.OemPlus ||
                e.KeySymbol is "+" or "=";

            var zoomOut =
                e.Key is Key.Subtract or
                    Key.OemMinus;

            if (zoomIn)
            {
                ChangeZoom(1.1);
                e.Handled = true;
                return;
            }

            if (zoomOut)
            {
                ChangeZoom(1 / 1.1);
                e.Handled = true;
                return;
            }

            if (e.Key is Key.D0 or
                Key.NumPad0 ||
                e.KeySymbol == "0")
            {
                ResetViewport();
                e.Handled = true;
                return;
            }
        }

        if (e.Key == Key.Delete ||
            e.Key == Key.Back)
        {
            _deleteTool.Execute();
            e.Handled = true;
            return;
        }

        _toolSystem.HandleKeyDown(e);
    }

    private void OnWindowKeyUp(
        object? sender,
        KeyEventArgs e)
    {
        _toolSystem.HandleKeyUp(e);
    }

    private void OnDeleteClicked(
        object? sender,
        RoutedEventArgs e)
    {
        _deleteTool.Execute();
    }

    private async void OnOpenImageClicked(
        object? sender,
        RoutedEventArgs e)
    {
        var files =
            await StorageProvider
                .OpenFilePickerAsync(
                    new FilePickerOpenOptions
                    {
                        Title = "Open Image",
                        AllowMultiple = false,
                        FileTypeFilter =
                        [
                            new FilePickerFileType(
                                "Image files")
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

        await using var stream =
            await files[0].OpenReadAsync();

        CanvasImage.Source =
            new Bitmap(stream);

        CanvasImage.IsVisible = true;
        CanvasPlaceholder.IsVisible = false;

        ResetViewport();
    }

    private void OnCloseImageClicked(
        object? sender,
        RoutedEventArgs e)
    {
        CanvasImage.Source = null;
        CanvasImage.IsVisible = false;
        CanvasPlaceholder.IsVisible = true;

        _toolSystem.Annotations.Clear();
        _toolSystem.SelectAnnotations([]);

        ResetViewport();
    }

    private void OnExitClicked(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private void OnZoomInClicked(
        object? sender,
        RoutedEventArgs e)
    {
        ChangeZoom(1.1);
    }

    private void OnZoomOutClicked(
        object? sender,
        RoutedEventArgs e)
    {
        ChangeZoom(1 / 1.1);
    }

    private void OnResetZoomClicked(
        object? sender,
        RoutedEventArgs e)
    {
        ResetViewport();
    }

    private void ChangeZoom(
        double factor)
    {
        var viewport =
            _toolSystem.Viewport;

        _toolSystem.SetViewport(
            viewport with
            {
                Scale =
                    viewport.Scale * factor
            });
    }

    private void ResetViewport()
    {
        _toolSystem.SetViewport(
            new ViewportState(
                0,
                0,
                1));
    }

    private void OnCanvasPointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        _toolSystem.HandlePointerPressed(e);
    }

    private void OnCanvasPointerMoved(
        object? sender,
        PointerEventArgs e)
    {
        _toolSystem.HandlePointerMoved(e);
    }

    private void OnCanvasPointerReleased(
        object? sender,
        PointerReleasedEventArgs e)
    {
        _toolSystem.HandlePointerReleased(e);
    }

    private void OnCanvasPointerExited(
        object? sender,
        PointerEventArgs e)
    {
        _toolSystem.HandlePointerExited(e);
    }

    private void OnCanvasPointerWheelChanged(
        object? sender,
        PointerWheelEventArgs e)
    {
        var commandPressed =
            e.KeyModifiers.HasFlag(
                KeyModifiers.Control) ||
            e.KeyModifiers.HasFlag(
                KeyModifiers.Meta);

        if (commandPressed)
        {
            ChangeZoom(
                e.Delta.Y > 0
                    ? 1.1
                    : 1 / 1.1);

            e.Handled = true;
            return;
        }

        _panTool.OnPointerWheelChanged(e);
    }

    private void RenderAnnotations()
    {
        DrawingCanvas.Children.Clear();

        var viewport =
            _toolSystem.Viewport;

        _imageScaleTransform.ScaleX =
            viewport.Scale;

        _imageScaleTransform.ScaleY =
            viewport.Scale;

        _imageTranslateTransform.X =
            viewport.X * viewport.Scale;

        _imageTranslateTransform.Y =
            viewport.Y * viewport.Scale;

        foreach (
            var annotation in
            _toolSystem.Annotations)
        {
            if (annotation.Type != "rectangle" ||
                annotation.Bounds.Count < 2)
            {
                continue;
            }

            var start =
                annotation.Bounds[0];

            var end =
                annotation.Bounds[1];

            var left =
                (Math.Min(start.X, end.X) +
                 viewport.X) *
                viewport.Scale;

            var top =
                (Math.Min(start.Y, end.Y) +
                 viewport.Y) *
                viewport.Scale;

            var width =
                Math.Abs(end.X - start.X) *
                viewport.Scale;

            var height =
                Math.Abs(end.Y - start.Y) *
                viewport.Scale;

            var selected =
                _toolSystem
                    .SelectedAnnotationIds
                    .Contains(annotation.Id);

            var rectangle =
                new Rectangle
                {
                    Width = width,
                    Height = height,
                    Stroke =
                        selected
                            ? Brushes.OrangeRed
                            : Brushes.Red,

                    Fill =
                        selected
                            ? new SolidColorBrush(
                                Color.FromArgb(
                                    70,
                                    255,
                                    120,
                                    50))
                            : new SolidColorBrush(
                                Color.FromArgb(
                                    35,
                                    255,
                                    0,
                                    0)),

                    StrokeThickness =
                        selected ? 3 : 2,

                    IsHitTestVisible = false
                };

            Canvas.SetLeft(
                rectangle,
                left);

            Canvas.SetTop(
                rectangle,
                top);

            DrawingCanvas.Children.Add(
                rectangle);

            if (selected)
            {
                var label =
                    new TextBlock
                    {
                        Text =
                            annotation.Name,

                        Foreground =
                            Brushes.OrangeRed,

                        FontSize = 16,
                        FontWeight =
                            FontWeight.Bold,

                        IsHitTestVisible =
                            false
                    };

                Canvas.SetLeft(
                    label,
                    left + 5);

                Canvas.SetTop(
                    label,
                    top + 5);

                DrawingCanvas.Children.Add(
                    label);
            }
        }
    }
}