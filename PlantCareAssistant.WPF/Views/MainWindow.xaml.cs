using System.Windows;
using PlantCareAssistant.WPF.ViewModels;

namespace PlantCareAssistant.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                await vm.LoadPlantsCommand.ExecuteAsync(null);
            }
        }
    }
}