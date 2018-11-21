﻿using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace HousePriceScraper
{
    public class Spider
    {


        public int NullCount { get; set; } = 0;
        public int FailureCount { get; set; } = 0;
        public int SuccessCount { get; set; } = 0;


        public bool Search(Property prop)
        {
            try
            {
                 var baseDir = AppContext.BaseDirectory;

                Console.WriteLine($@"Downloading {prop._key}");

                // set prop as Searching
                //prop.Searching = true;
                //prop.SearchingStartDate = DateTime.Now;
                //arango.Update<Property>(prop);

                //ChromeDriver chromeDriver;
                //ChromeOptions options = new ChromeOptions();

                //options.AddArgument("--headless");
                //options.AddArgument("--disable-gpu");
                //options.AddArgument("--no-sandbox");
                //options.AddArgument("--log-level=3");

                //chromeDriver = new ChromeDriver(options);

                //if (prop.RealEstateRecords == null || prop.RealEstateRecords.Count == 0)
                //{
                //    //var result = chromeDriver.SearchRealEstate(prop);
                //    var result = RealEstate.SearchRealEstate(prop);
                //    if (result.InvalidAddress)
                //    {
                //        prop.RealEstateInvalidAddress = true;
                //    }
                //    else
                //    {
                //        prop.RealEstateLandSize = result.LandSize;
                //        prop.RealEstateFloorSize = result.FloorArea;
                //        prop.RealEstateYearBuilt = result.YearBuilt;
                //        prop.RealEstateRecords = result.Data;
                //        prop.RealEstateValueRange = result.ValueRange;
                //        prop.RealEstateBedroom = result.Bedroom;
                //        prop.RealEstateBathroom = result.Bathroom;
                //        prop.RealEstateParking = result.Parking;
                //    }
                //}
                //chromeDriver.Dispose();

                //chromeDriver = new ChromeDriver(options);
                if (prop.DomainRecords == null || prop.DomainRecords.Count == 0)
                {
                    //var result = chromeDriver.SearchDomain(prop);
                    var result = Domain.SearchDomain(prop);
                    if (result.InvalidAddress)
                    {
                        prop.DomainInvalidAddress = true;
                    }
                    else
                    {
                        prop.DomainPropertyStory = result.PropertyStory;
                        prop.DomainRecords = result.Data;
                        prop.DomainLowValue = result.LowValue;
                        prop.DomainMidValue = result.MidValue;
                        prop.DomainHighValue = result.HighValue;
                        prop.DomainBedroom = result.Bedroom;
                        prop.DomainBathroom = result.Bathroom;
                        prop.DomainParking = result.Parking;
                    }
                }
                //chromeDriver.Dispose();

                if (prop.RealEstateRecords != null && prop.RealEstateRecords.Count > 0 &&
                    prop.DomainRecords != null && prop.DomainRecords.Count > 0)
                {
                    prop.LastSearchSuccess = true;
                }
                else
                {
                    prop.LastSearchSuccess = false;
                }


                prop.LastSearch = DateTime.Now;

                prop.LastSearchSuccess = prop.DomainRecords != null && prop.RealEstateRecords != null;

                prop.TryCount += 1;

                prop.Searching = false;

                prop.ClientName = Dns.GetHostName();


                Console.WriteLine($@"Download {prop.LastSearchSuccess} {prop._key}");
                Console.WriteLine($@"Time: {prop.LastSearch}");

                File.WriteAllText($@"{CreatePostcodeFolder(prop.Postcode)}/{prop.BuildKey()}.json", JsonConvert.SerializeObject(prop, Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string CreatePostcodeFolder(string postcode)
        {
            var postcodeDir = $@"{AppContext.BaseDirectory}/{postcode}";

            if (!Directory.Exists(postcodeDir))
                Directory.CreateDirectory(postcodeDir);

            return postcodeDir;
        }
    }
}
