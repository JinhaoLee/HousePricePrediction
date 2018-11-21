//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json;
namespace HousePriceScraper
{
    //public static class RealEstate
    //{
    //    public static string baseUrl = @"https://www.realestate.com.au/property/";

    //    public static SearchResult SearchRealEstate(this ChromeDriver chromeDriver, Property property)
    //    {
    //        var address = property.BuildRealEstateAddress();

    //        chromeDriver.Url = $@"{baseUrl}{address}";

    //        chromeDriver.Navigate();

    //        SearchResult result = new SearchResult();

    //        if (chromeDriver.Url == @"https://www.realestate.com.au/property")
    //        {
    //            // invalid address
    //            result.InvalidAddress = true;
    //            return result;
    //        }


    //        var noHistory = chromeDriver.TryFindElementByCss("p.property-timeline__no-event__highlight");


    //        chromeDriver.TryFindElementByCss("div.property-timeline__collapse-button")?
    //            .TryFindElementByCss("button")?
    //            .Click();

    //        // find history 

    //        var dataHistory = new List<HouseHistory>();

    //        var historyEntries = chromeDriver.FindElementsByCssSelector("div.property-timeline__item-content");

    //        if (historyEntries.Any())
    //            foreach (var historyCard in historyEntries)
    //            {
    //                var rentLabel = historyCard.TryFindElementByCss("span.property-timeline__label.property-timeline__label--rent");
    //                var soldLabel = historyCard.TryFindElementByCss("span.property-timeline__label.property-timeline__label--sold");

    //                //property-timeline__price
    //                var price = historyCard.TryFindElementByCss("div.property-timeline__price");
    //                //property-timeline__date
    //                var date = historyCard.TryFindElementByCss("span.property-timeline__date");

    //                if (rentLabel != null)
    //                {
    //                    dataHistory.Add(new HouseHistory()
    //                    {
    //                        Action = "Rent",
    //                        Date = date.Text,
    //                        Value = price.Text
    //                    });
    //                }
    //                else if (soldLabel != null)
    //                {
    //                    dataHistory.Add(new HouseHistory()
    //                    {
    //                        Action = "Sold",
    //                        Date = date.Text,
    //                        Value = price.Text
    //                    });
    //                }
    //            }

    //        result.Data = dataHistory;

    //        // value range
    //        var summary = chromeDriver.TryFindElementByCss("div.property-summary");
    //        if (summary != null)
    //        {
    //            var attributes = summary.TryFindElementByCss("div.property-details").TryFindElementByCss("div.property-info").TryFindElementByCss("div.property-info__attributes");
    //            if (attributes != null)
    //            {
    //                var atts = attributes.TryFindElementsByCss("span.rui-property-feature");
    //                if (atts.Count == 3)
    //                {
    //                    result.Bedroom = atts[0].TryFindElementByCss("span.config-num")?.Text;
    //                    result.Bathroom = atts[1].TryFindElementByCss("span.config-num")?.Text;
    //                    result.Parking = atts[2].TryFindElementByCss("span.config-num")?.Text;
    //                }
    //            }

    //            result.ValueRange = summary.TryFindElementByCss("p.avm-price__estimate")?.Text;
    //        }

    //        // details;

    //        var detailTable = chromeDriver.FindElementsByCssSelector("div.property-details")?
    //            .Where(div => div.TryFindElementByCss("table.info-table") != null)?.FirstOrDefault();

    //        if (detailTable != null)
    //        {
    //            foreach (var row in detailTable.TryFindElementsByCss("tr"))
    //            {
    //                var cells = row.TryFindElementsByCss("td");
    //                if (cells != null && cells.Count == 2)
    //                {
    //                    switch (cells[0].Text)
    //                    {
    //                        case "Land size":
    //                            result.LandSize = cells[1].Text;
    //                            break;
    //                        case "Floor area":
    //                            result.FloorArea = cells[1].Text;
    //                            break;
    //                        case "Year built":
    //                            result.YearBuilt = cells[1].Text;
    //                            break;
    //                    }
    //                }
    //            }
    //        }

    //        return result;
    //    }
    //}
    public static class RealEstate
    {
        public static string baseUrl = @"https://www.realestate.com.au/property/";


        public static SearchResult SearchRealEstate(Property property)
        {
            var address = property.BuildRealEstateAddress();

            return SearchRealEstate(address);
        }

        public static SearchResult SearchRealEstate(string path)
        {
            var url = $@"{baseUrl}{path}";

            SearchResult result = new SearchResult();

            using (HttpClient baseHttpClient = new HttpClient())
            {

                baseHttpClient.BaseAddress = new Uri(@"https://www.realestate.com.au/property/");

                baseHttpClient.DefaultRequestHeaders.Add("user-agent", " Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");


                var response = baseHttpClient.GetAsync(path).GetAwaiter().GetResult();
                var html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                // Property Value and Property Market Data - realestate.com.au

                var title = document.DocumentNode.DescendantsAndSelf().Where(n => n.Name == "title").FirstOrDefault();

                if (title != null && title.InnerText == "Property Value and Property Market Data - realestate.com.au")
                {
                    result.InvalidAddress = true;
                    return result;
                }

                var property_info = document.DocumentNode.DescendantsAndSelf()
                    .Where(n => n.Name.ToLower() == "div" && n.HasClass("property-info__attributes"))
                    .FirstOrDefault();

                if (property_info != null)
                {
                    var bedrooms = property_info.DescendantsAndSelf()
                        .Where(n => n.HasClass("rui-property-feature") && n.DescendantsAndSelf().Any(c => c.InnerText == "Bedrooms"))
                        .FirstOrDefault();
                    if (bedrooms != null)
                    {
                        var num = bedrooms.DescendantsAndSelf().Where(n => n.HasClass("config-num")).FirstOrDefault();
                        result.Bedroom = num.InnerText;
                    }

                    var bathrooms = property_info.DescendantsAndSelf()
                        .Where(n => n.HasClass("rui-property-feature") && n.DescendantsAndSelf().Any(c => c.InnerText == "Bathrooms"))
                        .FirstOrDefault();
                    if (bathrooms != null)
                    {
                        var num = bathrooms.DescendantsAndSelf().Where(n => n.HasClass("config-num")).FirstOrDefault();
                        result.Bathroom = num.InnerText;
                    }

                    var carspaces = property_info.DescendantsAndSelf()
                        .Where(n => n.HasClass("rui-property-feature") && n.DescendantsAndSelf().Any(c => c.InnerText == "Car Spaces"))
                        .FirstOrDefault();
                    if (carspaces != null)
                    {
                        var num = carspaces.DescendantsAndSelf().Where(n => n.HasClass("config-num")).FirstOrDefault();
                        result.Parking = num.InnerText;
                    }
                }

                Regex rgxPrediction = new Regex(@"{""confidence"":""\w+"",""range"":{""text"":""(\$[\d,]+ *- *\$[\d,]+)""}}");

                var predictionMatch = rgxPrediction.Match(html);
                if (predictionMatch.Success)
                {
                    result.ValueRange = predictionMatch.Groups[1].Value;
                }

                Regex rgxPropertyId = new Regex(@"\/property\/purchase_title\/(\d+)");

                var propertyIdMatch = rgxPropertyId.Match(html);
                if (predictionMatch.Success)
                {
                    // query the history
                    string propertyId = propertyIdMatch.Groups[1].Value;

                    string baseQueryUrl = @"https://pexa.realestate.com.au/graphql?query={property(propertyId:%22" +
                        propertyId +
                        "%22){id,propertyTimeline{date,agency,eventType,price}}}";

                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Referrer = new Uri(url);
                        httpClient.DefaultRequestHeaders.Add("user-agent", " Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                        var jsonHistory = httpClient.GetStringAsync(baseQueryUrl).GetAwaiter().GetResult();

                        // use http://json2csharp.com/ to generate the C# object from json
                        var history = JsonConvert.DeserializeObject<HistoryObject>(jsonHistory);

                        var timeline = history?.data?.property?.propertyTimeline;

                        if (timeline != null)
                        {
                            result.Data = timeline.Select(ev =>
                            {
                                return new HouseHistory()
                                {
                                    Action = ev.eventType,
                                    Date = ev.date,
                                    Value = ev.price,
                                    Agent = ev.agency
                                };
                            }).ToList();
                        }
                    }
                }

                var propertyDetails = document.DocumentNode.DescendantsAndSelf()
                    .Where(
                    n => n.Name == "div" &&
                    n.HasClass("property-details") &&
                    n.DescendantsAndSelf().Any(
                        t =>
                        t.Name == "table" && t.HasClass("info-table")
                        )).FirstOrDefault();

                if (propertyDetails != null)
                {
                    var propertyInfoTable = propertyDetails.DescendantsAndSelf().Where(n => n.Name == "table" && n.HasClass("info-table")).FirstOrDefault();
                    if (propertyInfoTable != null)
                    {
                        var propertyInfoTableBody = propertyInfoTable.DescendantsAndSelf().Where(n => n.Name == "tbody").FirstOrDefault();
                        if (propertyInfoTableBody != null)
                        {
                            var propertyInfoRows = propertyInfoTableBody.DescendantsAndSelf().Where(n => n.Name == "tr").ToList();

                            foreach (var infoRow in propertyInfoRows)
                            {
                                var label = infoRow.Descendants().First(n => n.Name == "td").InnerText.ToLower();
                                var value = infoRow.Descendants().Last(n => n.Name == "td").InnerText;
                                switch (label)
                                {
                                    case "land size":
                                        result.LandSize = value;
                                        break;
                                    case "floor area":
                                        result.FloorArea = value;
                                        break;
                                    case "year built":
                                        result.YearBuilt = value;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }


    // the following data is used to deserialize the json from realestate;
    public class PropertyTimeline
    {
        public string date { get; set; }
        public string agency { get; set; }
        public string eventType { get; set; }
        public string price { get; set; }
    }

    public class HistoryProperty
    {
        public string id { get; set; }
        public List<PropertyTimeline> propertyTimeline { get; set; }
    }

    public class HistoryData
    {
        public HistoryProperty property { get; set; }
    }

    public class HistoryObject
    {
        public HistoryData data { get; set; }
    }
}
