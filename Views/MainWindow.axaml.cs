// using System;
using ASTEM_DB.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ASTEM_DB.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

        }
        private void OnCardClicked(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (this.DataContext is MainWindowViewModel vm)
                {
                    if (button.DataContext is CardItemViewModel clickedCard)
                    {
                        if (vm.SelectedCard == clickedCard)
                        {
                            vm.IsSidebarVisible = !vm.IsSidebarVisible;
                        }
                        else
                        {
                            vm.SelectedCard = clickedCard;
                            vm.IsSidebarVisible = true;
                        }
                    }
                    else if (button.DataContext is BoardItemViewModel clickedBoard)
                    {
                        if (vm.SelectedBoard == clickedBoard)
                        {
                            vm.IsSidebarVisible = !vm.IsSidebarVisible;
                        }
                        else
                        {
                            vm.SelectedBoard = clickedBoard;
                            vm.IsSidebarVisible = true;
                        }
                    }
                }
            }
        }
        private void ToggleOverlay(object? sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.IsOverlayVisible = !vm.IsOverlayVisible;
            }
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
