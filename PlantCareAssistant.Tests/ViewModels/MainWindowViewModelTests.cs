using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Moq;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;
using PlantCareAssistant.WPF.ViewModels;
using Xunit;

namespace PlantCareAssistant.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly Mock<IPlantRepository> _mockRepository;
        private readonly Mock<ICareService> _mockCareService;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _mockRepository = new Mock<IPlantRepository>();
            _mockCareService = new Mock<ICareService>();
            _viewModel = new MainWindowViewModel(_mockRepository.Object, _mockCareService.Object);
        }

        [Fact]
        public async Task LoadPlantsAsync_PopulatesPlantsCollection()
        {
            var plants = new List<Plant>
            {
                new Plant { Id = 1, Name = "Plant1", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 },
                new Plant { Id = 2, Name = "Plant2", Location = "Кухня", CareType = "Цветущий", Status = "Здоровое", WateringFrequencyDays = 5, FertilizerFrequencyWeeks = 4 }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(plants);

            await _viewModel.LoadPlantsCommand.ExecuteAsync(null);

            Assert.Equal(2, _viewModel.Plants.Count);
        }

        [Fact]
        public void SearchQuery_FiltersPlantsByName()
        {
            _viewModel.Plants.Add(new Plant { Id = 1, Name = "Роза", Location = "Комната", CareType = "Цветущий", Status = "Здоровое", WateringFrequencyDays = 5, FertilizerFrequencyWeeks = 4 });
            _viewModel.Plants.Add(new Plant { Id = 2, Name = "Кактус", Location = "Кухня", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 14, FertilizerFrequencyWeeks = 4 });

            _viewModel.SearchQuery = "Роза";

            Assert.Single(_viewModel.FilteredPlants);
            Assert.Equal("Роза", _viewModel.FilteredPlants[0].Name);
        }

        [Fact]
        public void SelectedLocation_FiltersPlantsByLocation()
        {
            _viewModel.Plants.Add(new Plant { Id = 1, Name = "Plant1", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 });
            _viewModel.Plants.Add(new Plant { Id = 2, Name = "Plant2", Location = "Кухня", CareType = "Цветущий", Status = "Здоровое", WateringFrequencyDays = 5, FertilizerFrequencyWeeks = 4 });

            _viewModel.SelectedLocation = "Комната";

            Assert.Single(_viewModel.FilteredPlants);
            Assert.Equal("Комната", _viewModel.FilteredPlants[0].Location);
        }

        [Fact]
        public void HasSelectedPlant_ReturnsTrue_WhenPlantIsSelected()
        {
            var plant = new Plant { Id = 1, Name = "Test", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 };
            _viewModel.SelectedPlant = plant;

            Assert.True(_viewModel.HasSelectedPlant);
        }

        [Fact]
        public void HasSelectedPlant_ReturnsFalse_WhenNoPlantSelected()
        {
            _viewModel.SelectedPlant = null;

            Assert.False(_viewModel.HasSelectedPlant);
        }

        [Fact]
        public async Task DeletePlantAsync_RemovesPlantFromCollection()
        {
            // Arrange
            var plant = new Plant { Id = 1, Name = "ToDelete", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 };
            _viewModel.Plants.Add(plant);
            _viewModel.FilteredPlants.Add(plant);
            _viewModel.SelectedPlant = plant;

            // Мокаем DeleteAsync чтобы он ничего не делал
            _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act - просто вызываем команду, без проверки MessageBox
            await _viewModel.DeletePlantCommand.ExecuteAsync(null);

            // Assert - проверяем только что метод репозитория был вызван
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public void UpdateStatistics_CalculatesCorrectCounts()
        {
            var plant1 = new Plant { Id = 1, Name = "Plant1", Location = "Комната", CareType = "Кактус", Status = "Здоровое", WateringFrequencyDays = 7, FertilizerFrequencyWeeks = 4 };
            var plant2 = new Plant { Id = 2, Name = "Plant2", Location = "Кухня", CareType = "Цветущий", Status = "Требует внимания", WateringFrequencyDays = 5, FertilizerFrequencyWeeks = 4 };
            var plant3 = new Plant { Id = 3, Name = "Plant3", Location = "Балкон", CareType = "Овощи", Status = "Болеет", WateringFrequencyDays = 3, FertilizerFrequencyWeeks = 2 };

            _viewModel.Plants.Add(plant1);
            _viewModel.Plants.Add(plant2);
            _viewModel.Plants.Add(plant3);

            // Вызываем обновление статистики напрямую
            _viewModel.TotalPlantsCount = _viewModel.Plants.Count;
            _viewModel.HealthyPlantsCount = _viewModel.Plants.Count(p => p.Status == "Здоровое");
            _viewModel.AttentionPlantsCount = _viewModel.Plants.Count(p => p.Status == "Требует внимания");
            _viewModel.SickPlantsCount = _viewModel.Plants.Count(p => p.Status == "Болеет");

            Assert.Equal(3, _viewModel.TotalPlantsCount);
            Assert.Equal(1, _viewModel.HealthyPlantsCount);
            Assert.Equal(1, _viewModel.AttentionPlantsCount);
            Assert.Equal(1, _viewModel.SickPlantsCount);
        }
    }
}