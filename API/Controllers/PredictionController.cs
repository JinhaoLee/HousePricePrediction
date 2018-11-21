using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using House.Prediction;
using MvcAngular;
using API.Entities;
using API.AutoFac;
using Newtonsoft.Json;
using HousePriceScraper;

namespace API.Controllers
{
    [Angular, Route("[controller]/[action]")]

    public class PredictionController: Controller
    {
        private readonly PredictionService.PredictionServiceClient predictionServiceClient;
        private readonly Dictionary<int, HouseIndex> houseIndices;
        private readonly HouseDataSource houseDataSource;
        public PredictionController(
            PredictionService.PredictionServiceClient predictionServiceClient,
            Dictionary<int, HouseIndex> houseIndices,
            HouseDataSource houseDataSource)
        {
            this.predictionServiceClient = predictionServiceClient;
            this.houseIndices = houseIndices;
            this.houseDataSource = houseDataSource;
        }

        [HttpPost]
        public async Task<HousePredictionResponse> Predict([FromBody] HousePredictionRequest request)
        {

            var response = await predictionServiceClient.FindNearestHouseIndicesAsync(new PredictionRequest()
            {
                NumberOfBedrooms = request.NumberOfBedrooms,
                NumberOfBathrooms = request.NumberOfBathrooms,
                NumberOfParkings = request.NumberOfParkings,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            });

            List<double> values = new List<double>();
            List<string> houses = new List<string>();
            foreach (var index in response.Indices)
            {
                var hi = houseIndices[index];

                string path = $@"{this.houseDataSource.DirectoryPath}/{hi.Postcode}/{hi.Key}.json";
                string json = System.IO.File.ReadAllText(path);
                var prop = JsonConvert.DeserializeObject<Property>(json);

                double value = 0d;

                if (double.TryParse(prop.DomainMidValue, out value))
                {
                    values.Add(value);
                }
                houses.Add(prop.BuildGoogleAddress());
            }

            return new HousePredictionResponse()
            {
                Price = values.Average(),
                SimilarHoueses = houses
            };
        }
    }
}