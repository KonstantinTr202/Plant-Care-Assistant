using Moq;
using PlantCareAssistant.Core.Interfaces;
using PlantCareAssistant.Core.Models;
using PlantCareAssistant.WPF.ViewModels;
using Xunit;

namespace PlantCareAssistant.Tests.ViewModels
{
    public class PlantViewModelTests
    {
        private readonly Mock<IPlantRepository> _mockRepository;
        private readonly Mock<ICareService> _mockCareService;
        private readonly PlantViewModel _viewModel;

        public PlantViewModelTests()
        {
            _mockRepository = new Mock<IPlantRepository>();
            _mockCareService = new Mock<ICareService>();
            // Передаём null вместо Window - для тестов не нужен UI
            _viewModel = new PlantViewModel(_mockRepository.Object, _mockCareService.Object, null);
        }

        [Fact]
        public void ValidateName_SetsError_WhenNameIsEmpty()
        {
            // Arrange
            var plant = new Plant { Name = "" };
            _viewModel.Plant = plant;

            // Act
            _viewModel.ValidateName();

            // Assert
            Assert.True(_viewModel.HasNameError);
            Assert.NotEmpty(_viewModel.NameErrorMessage);
        }

        [Fact]
        public void ValidateName_ClearsError_WhenNameIsValid()
        {
            // Arrange
            var plant = new Plant { Name = "Valid Name" };
            _viewModel.Plant = plant;

            // Act
            _viewModel.ValidateName();

            // Assert
            Assert.False(_viewModel.HasNameError);
            Assert.Empty(_viewModel.NameErrorMessage);
        }

        [Fact]
        public void ValidateWatering_SetsError_WhenFrequencyIsZero()
        {
            // Arrange
            var plant = new Plant { WateringFrequencyDays = 0 };
            _viewModel.Plant = plant;

            // Act
            _viewModel.ValidateWatering();

            // Assert
            Assert.True(_viewModel.HasWateringError);
            Assert.NotEmpty(_viewModel.WateringErrorMessage);
        }

        [Fact]
        public void ValidateWatering_ClearsError_WhenFrequencyIsValid()
        {
            // Arrange
            var plant = new Plant { WateringFrequencyDays = 7 };
            _viewModel.Plant = plant;

            // Act
            _viewModel.ValidateWatering();

            // Assert
            Assert.False(_viewModel.HasWateringError);
            Assert.Empty(_viewModel.WateringErrorMessage);
        }

        [Fact]
        public void ValidateFertilizer_SetsError_WhenFrequencyIsZero()
        {
            // Arrange
            var plant = new Plant { FertilizerFrequencyWeeks = 0 };
            _viewModel.Plant = plant;

            // Act
            _viewModel.ValidateFertilizer();

            // Assert
            Assert.True(_viewModel.HasFertilizerError);
            Assert.NotEmpty(_viewModel.FertilizerErrorMessage);
        }

        [Fact]
        public void LoadPlantForEdit_SetsEditMode()
        {
            // Arrange
            var plant = new Plant
            {
                Id = 1,
                Name = "EditTest",
                Location = "Комната",
                CareType = "Кактус",
                Status = "Здоровое",
                WateringFrequencyDays = 7,
                FertilizerFrequencyWeeks = 4
            };

            // Act
            _viewModel.LoadPlantForEdit(plant);

            // Assert
            Assert.True(_viewModel.IsEditMode);
            Assert.Equal("Редактировать растение", _viewModel.WindowTitle);
            Assert.Equal("EditTest", _viewModel.Plant.Name);
        }
    }
}