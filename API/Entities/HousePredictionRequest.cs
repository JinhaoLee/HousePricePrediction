using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcAngular;

namespace API.Entities
{
    [AngularType]
    public class HousePredictionRequest
    {
        public int NumberOfBedrooms { get; set; }
        public int NumberOfBathrooms { get; set; }
        public int NumberOfParkings { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}