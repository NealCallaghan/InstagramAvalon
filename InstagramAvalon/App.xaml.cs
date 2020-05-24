using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InstagramAvalon.ViewModels;
using InstagramAvalon.Views;

namespace InstagramAvalon
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                var dataContext = new MainWindowViewModel(desktop.MainWindow);

                desktop.MainWindow.DataContext = dataContext;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
