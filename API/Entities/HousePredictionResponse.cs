using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcAngular;

namespace API.Entities
{
    [AngularType]
    public class HousePredictionResponse
    {
        public double Price { get; set; }
        public List<string> SimilarHoueses { get; set; }
    }
}
