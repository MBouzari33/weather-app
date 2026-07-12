using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherViewModel = WeatherApp.ViewModel.ViewModel;

namespace WeatherApp.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        [TestMethod]
        public void Search_is_disabled_for_blank_city()
        {
            var viewModel = new WeatherViewModel(new FakeWeatherService());
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));
            viewModel.City = "Berlin";
            Assert.IsTrue(viewModel.SearchCommand.CanExecute(null));
        }

        [TestMethod]
        public async Task Successful_search_populates_dashboard()
        {
            var service = new FakeWeatherService
            {
                Result = new WeatherResponse
                {
                    Name = "Berlin",
                    Main = new MainInfo { Temp = 18.4, FeelsLike = 17.1, Humidity = 63 },
                    Weather = new List<WeatherInfo>
                    {
                        new WeatherInfo { Main = "Clouds", Description = "scattered clouds" }
                    }
                }
            };
            var viewModel = new WeatherViewModel(service) { City = "  Berlin  " };

            await viewModel.SearchCommand.ExecuteAsync();

            Assert.IsTrue(viewModel.HasResult);
            Assert.IsFalse(viewModel.HasError);
            Assert.AreEqual("Berlin", service.RequestedCity);
            Assert.AreEqual("Berlin", viewModel.LocationName);
            Assert.AreEqual("Scattered clouds", viewModel.Description);
            Assert.AreEqual("Clouds", viewModel.Condition);
            Assert.AreEqual(63, viewModel.Humidity);
        }

        [TestMethod]
        public async Task Failed_search_exposes_friendly_error_state()
        {
            var viewModel = new WeatherViewModel(new FakeWeatherService
            {
                Error = new Exception("City not found.")
            }) { City = "Atlantis" };

            await viewModel.SearchCommand.ExecuteAsync();

            Assert.IsTrue(viewModel.HasError);
            Assert.IsFalse(viewModel.HasResult);
            Assert.AreEqual("City not found.", viewModel.ErrorMessage);
        }

        private sealed class FakeWeatherService : IWeatherService
        {
            public WeatherResponse Result { get; set; }
            public Exception Error { get; set; }
            public string RequestedCity { get; private set; }

            public Task<WeatherResponse> GetWeatherAsync(string city)
            {
                RequestedCity = city;
                if (Error != null) throw Error;
                return Task.FromResult(Result);
            }
        }
    }
}