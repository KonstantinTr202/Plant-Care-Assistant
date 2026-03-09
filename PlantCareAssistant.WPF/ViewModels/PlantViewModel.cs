using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace PlantCareAssistant.WPF.ViewModels
{
    [ObservableObject]
    public partial class PlantViewModel
    {
        private readonly IPlantRepository _repository;
        private readonly ICareService _careService;
        private readonly Window _window;

        [ObservableProperty]
        private Plant _plant = new();

        [ObservableProperty]
        private string _windowTitle = "Добавить растение";

        [ObservableProperty]
        private bool _hasNameError;

        [ObservableProperty]
        private string _nameErrorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasWateringError;

        [ObservableProperty]
        private string _wateringErrorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasFertilizerError;

        [ObservableProperty]
        private string _fertilizerErrorMessage = string.Empty;

        public ObservableCollection<string> Locations { get; } =
            new() { "Комната", "Кухня", "Балкон", "Сад" };

        public ObservableCollection<string> CareTypes { get; } =
            new() { "Кактус", "Декоративно-лиственный", "Цветущий", "Овощи", "Травы" };

        public ObservableCollection<string> Statuses { get; } =
            new() { "Здоровое", "Требует внимания", "Болеет", "Погибло" };

        public ObservableCollection<string> LightingTypes { get; } =
            new() { "Тень", "Полутень", "Солнечно" };

        public bool IsEditMode { get; set; } = false;

        public PlantViewModel(IPlantRepository repository, ICareService careService, Window window)
        {
            _repository = repository;
            _careService = careService;
            _window = window;
        }

        public void LoadPlantForEdit(Plant plant)
        {
            Plant = plant;
            IsEditMode = true;
            WindowTitle = "Редактировать растение";
        }

        partial void OnPlantChanged(Plant value)
        {
            ValidateName();
            ValidateWatering();
            ValidateFertilizer();
        }

        public void ValidateName()
        {
            HasNameError = string.IsNullOrWhiteSpace(Plant.Name);
            NameErrorMessage = HasNameError ? "Название обязательно" : string.Empty;
        }

        public void ValidateWatering()
        {
            HasWateringError = Plant.WateringFrequencyDays <= 0;
            WateringErrorMessage = HasWateringError ? "Должно быть > 0" : string.Empty;
        }

        public void ValidateFertilizer()
        {
            HasFertilizerError = Plant.FertilizerFrequencyWeeks <= 0;
            FertilizerErrorMessage = HasFertilizerError ? "Должно быть > 0" : string.Empty;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            ValidateName();
            ValidateWatering();
            ValidateFertilizer();

            if (HasNameError || HasWateringError || HasFertilizerError)
            {
                MessageBox.Show("Исправьте ошибки перед сохранением!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsEditMode)
                await _repository.UpdateAsync(Plant);
            else
                await _repository.AddAsync(Plant);

            _window.DialogResult = true;
            _window.Close();
        }

        [RelayCommand]
        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}