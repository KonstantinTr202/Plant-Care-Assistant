using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace PlantCareAssistant.WPF.ViewModels
{
    [ObservableObject]
    public partial class CareViewModel
    {
        private readonly IPlantRepository _repository;
        private readonly ICareService _careService;
        private readonly Window _window;

        [ObservableProperty]
        private Plant? _currentPlant;

        [ObservableProperty]
        private string _plantName = string.Empty;

        [ObservableProperty]
        private string _plantSpecies = string.Empty;

        [ObservableProperty]
        private DateTime _lastWateringDate;

        [ObservableProperty]
        private DateTime _nextWateringDate;

        [ObservableProperty]
        private DateTime _nextFertilizerDate;

        [ObservableProperty]
        private ObservableCollection<CareRecord> _careRecords = new();

        [ObservableProperty]
        private string _careNotes = string.Empty;

        public CareViewModel(IPlantRepository repository, ICareService careService, Window window)
        {
            _repository = repository;
            _careService = careService;
            _window = window;
        }

        public async Task LoadPlantDataAsync(int plantId)
        {
            CurrentPlant = await _repository.GetByIdAsync(plantId);
            if (CurrentPlant != null)
            {
                PlantName = CurrentPlant.Name;
                PlantSpecies = CurrentPlant.Species ?? string.Empty;
                LastWateringDate = CurrentPlant.LastWateringDate ?? DateTime.Now;
                NextWateringDate = CurrentPlant.NextWateringDate;
                NextFertilizerDate = CurrentPlant.NextFertilizerDate;

                var history = await _repository.GetCareHistoryAsync(plantId);
                CareRecords.Clear();
                foreach (var r in history) CareRecords.Add(r);
            }
        }

        [RelayCommand]
        private async Task AddWateringAsync()
        {
            await AddCareRecordAsync("Полив");
        }

        [RelayCommand]
        private async Task AddFertilizerAsync()
        {
            await AddCareRecordAsync("Удобрение");
        }

        [RelayCommand]
        private async Task AddSprayingAsync()
        {
            await AddCareRecordAsync("Опрыскивание");
        }

        private async Task AddCareRecordAsync(string type)
        {
            if (CurrentPlant == null) return;

            var record = new CareRecord
            {
                PlantId = CurrentPlant.Id,
                Date = DateTime.Now,
                Type = type,
                Notes = CareNotes
            };

            await _repository.AddCareRecordAsync(record);
            await LoadPlantDataAsync(CurrentPlant.Id);
            CareNotes = string.Empty;
        }

        [RelayCommand]
        private void Close()
        {
            _window.DialogResult = true;
            _window.Close();
        }
    }
}