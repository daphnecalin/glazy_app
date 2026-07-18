using ASTEM_DB.Resources.Languages;
using ASTEM_DB.ViewModels;
using ASTEM_DB.Views;
using Avalonia;
using Avalonia.Diagnostics;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ASTEM_DB
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Resource.Culture = new CultureInfo("en");
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                #if DEBUG
                desktop.MainWindow.AttachDevTools();
                #endif
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}