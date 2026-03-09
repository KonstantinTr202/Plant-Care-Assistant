using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace PlantCareAssistant.WPF.ViewModels
{
    [ObservableObject]
    public partial class MainWindowViewModel
    {
        private readonly IPlantRepository _repository;
        private readonly ICareService _careService;

        [ObservableProperty]
        private ObservableCollection<Plant> _plants = new();

        [ObservableProperty]
        private Plant? _selectedPlant;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _selectedLocation = "Все";

        [ObservableProperty]
        private string _selectedCareType = "Все";

        [ObservableProperty]
        private string _selectedStatus = "Все";

        [ObservableProperty]
        private ObservableCollection<Plant> _plantsRequiringCare = new();

        [ObservableProperty]
        private int _totalPlantsCount;

        [ObservableProperty]
        private int _healthyPlantsCount;

        [ObservableProperty]
        private int _attentionPlantsCount;

        [ObservableProperty]
        private int _sickPlantsCount;

        [ObservableProperty]
        private ObservableCollection<Plant> _filteredPlants = new();

        public ObservableCollection<string> Locations { get; } =
            new() { "Все", "Комната", "Кухня", "Балкон", "Сад" };

        public ObservableCollection<string> CareTypes { get; } =
            new() { "Все", "Кактус", "Декоративно-лиственный", "Цветущий", "Овощи", "Травы" };

        public ObservableCollection<string> Statuses { get; } =
            new() { "Все", "Здоровое", "Требует внимания", "Болеет", "Погибло" };

        public bool HasSelectedPlant => SelectedPlant != null;

        public MainWindowViewModel(IPlantRepository repository, ICareService careService)
        {
            _repository = repository;
            _careService = careService;
        }

        partial void OnSelectedPlantChanged(Plant? value)
        {
            OnPropertyChanged(nameof(HasSelectedPlant));
        }

        [RelayCommand]
        private async Task LoadPlantsAsync()
        {
            var plants = await _repository.GetAllAsync();
            Plants.Clear();
            foreach (var p in plants) Plants.Add(p);
            UpdateFilters();
        }

        partial void OnSearchQueryChanged(string value) => UpdateFilters();
        partial void OnSelectedLocationChanged(string value) => UpdateFilters();
        partial void OnSelectedCareTypeChanged(string value) => UpdateFilters();
        partial void OnSelectedStatusChanged(string value) => UpdateFilters();

        private void UpdateFilters()
        {
            var filtered = Plants.Where(p =>
                (string.IsNullOrEmpty(SearchQuery) ||
                 p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                 (p.Species != null && p.Species.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))) &&
                (SelectedLocation == "Все" || p.Location == SelectedLocation) &&
                (SelectedCareType == "Все" || p.CareType == SelectedCareType) &&
                (SelectedStatus == "Все" || p.Status == SelectedStatus)
            ).ToList();

            FilteredPlants.Clear();
            foreach (var p in filtered) FilteredPlants.Add(p);

            UpdateStatistics();
            UpdateCarePanel();
        }

        private void UpdateStatistics()
        {
            TotalPlantsCount = Plants.Count;
            HealthyPlantsCount = Plants.Count(p => p.Status == "Здоровое");
            AttentionPlantsCount = Plants.Count(p => p.Status == "Требует внимания");
            SickPlantsCount = Plants.Count(p => p.Status == "Болеет");
        }

        private void UpdateCarePanel()
        {
            PlantsRequiringCare.Clear();
            foreach (var p in Plants)
            {
                if (_careService.IsPlantRequiringCare(p, DateTime.Now))
                    PlantsRequiringCare.Add(p);
            }
        }

        [RelayCommand]
        private void AddPlant()
        {
            var window = new PlantWindow();
            var viewModel = new PlantViewModel(_repository, _careService, window);
            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true)
            {
                LoadPlantsCommand.Execute(null);
            }
        }

        [RelayCommand]
        private void EditPlant()
        {
            if (SelectedPlant != null)
            {
                var window = new PlantWindow();
                var viewModel = new PlantViewModel(_repository, _careService, window);
                viewModel.LoadPlantForEdit(SelectedPlant);
                window.DataContext = viewModel;
                window.Owner = Application.Current.MainWindow;

                if (window.ShowDialog() == true)
                {
                    LoadPlantsCommand.Execute(null);
                }
            }
        }

        [RelayCommand]
        private async Task DeletePlantAsync()
        {
            if (SelectedPlant != null)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить растение \"{SelectedPlant.Name}\"?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _repository.DeleteAsync(SelectedPlant.Id);
                    Plants.Remove(SelectedPlant);
                    UpdateFilters();
                }
            }
        }

        [RelayCommand]
        private void OpenCareWindow()
        {
            if (SelectedPlant != null)
            {
                var window = new CareWindow();
                var viewModel = new CareViewModel(_repository, _careService, window);
                window.DataContext = viewModel;
                window.Owner = Application.Current.MainWindow;

                _ = viewModel.LoadPlantDataAsync(SelectedPlant.Id);
                window.ShowDialog();
            }
        }

        [RelayCommand]
        private async Task MarkCareAsync(Plant plant)
        {
            if (plant != null)
            {
                var record = new CareRecord
                {
                    PlantId = plant.Id,
                    Date = DateTime.Now,
                    Type = "Полив",
                    Notes = "Быстрый уход"
                };
                await _repository.AddCareRecordAsync(record);
                UpdateCarePanel();
                LoadPlantsCommand.Execute(null);
            }
        }
    }
}