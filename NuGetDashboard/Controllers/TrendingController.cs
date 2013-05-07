using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Script;
using System.Web.Script.Serialization;
using NuGetDashboard.Models;
using NuGetDashboard.Utilities;
using System.Web.Helpers;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using System.Configuration;
using DotNet.Highcharts.Enums;

namespace NuGetDashboard.Controllers
{
    public class TrendingController : Controller
    {
        public ActionResult Index()       
        {
            string[] checkNames = new string[] { "TrendingPackageLastWeek", "TrendingPackageThisWeek" };
            List<DotNet.Highcharts.Options.Series> seriesSet = new List<DotNet.Highcharts.Options.Series>();
            List<string> xValues = new List<string>();
            List<Object> yValues = new List<Object>();
            foreach (string check in checkNames)
            {                               
                //Get the response values from pre-created blobs for each check.
                BlobStorageService.GetJsonDataFromBlob(check + ".json" , out xValues, out yValues);
                yValues.RemoveRange(5, 5);
                xValues.RemoveRange(5, 5);
                seriesSet.Add(new DotNet.Highcharts.Options.Series
                {
                    Data = new Data(yValues.ToArray()),
                    Name = check
                    
                });
            }
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("TopTrendingPackages")
            .InitChart(new DotNet.Highcharts.Options.Chart { DefaultSeriesType = ChartTypes.Column })
            .SetPlotOptions(new PlotOptions
             { 
                 Column = new PlotOptionsColumn
                 { 
                  Stacking = Stackings.Normal,                 
                 }
             });             
        
            chart.SetXAxis(new XAxis
            {
                Categories = xValues.ToArray(),              

            });
            chart.SetSeries(seriesSet.ToArray());         
            
            chart.SetTitle(new DotNet.Highcharts.Options.Title { Text = "Top Trending Packages" });            
            return View(chart);
        }
       

    }
}
