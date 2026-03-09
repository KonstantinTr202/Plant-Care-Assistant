using System.Windows;
using Microsoft.EntityFrameworkCore;
using PlantCareAssistant.Core.Data;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Services;
using PlantCareAssistant.WPF.ViewModels;

namespace PlantCareAssistant.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var dbContext = new AppDbContext())
            {
                dbContext.Database.EnsureCreated();
            }

            var context = new AppDbContext();
            var repository = new PlantRepository(context);
            var careService = new CareService();


            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(repository, careService);
            mainWindow.Show();
        }
    }
}