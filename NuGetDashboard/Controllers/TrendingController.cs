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
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;
namespace NuGetDashboard.Controllers
{
    public class TrendingController : Controller
    {
        public ActionResult Index()       
        {
            return View();
        }

        //Returns the Net trend chart for packages 
        public ActionResult PackagesChart()
        {
            DotNet.Highcharts.Highcharts chart = GetChart("UploadsMayMonthlyReport.json", "Packages");
            return PartialView("~/Views/Home/PackagesChart.cshtml", chart);
        }

        //Returns the Net trend chart for Downloads
        public ActionResult DownloadsChart()
        {
            DotNet.Highcharts.Highcharts chart = GetChart("DownloadsMayMonthlyReport.json", "Downloads");
            return PartialView("~/Views/Home/PackagesChart.cshtml", chart);
        }

        //Returns the Net trend chart for Downloads
        public ActionResult UsersChart()
        {
            DotNet.Highcharts.Highcharts chart = GetChart("UsersMayMonthlyReport.json", "Users");
            return PartialView("~/Views/Home/PackagesChart.cshtml", chart);
        }

        private static Highcharts GetChart(string blobName, string chartName)
        {
            List<string> xValues = new List<string>();
            List<Object> yValues = new List<Object>();
            BlobStorageService.GetJsonDataFromBlob(blobName, out xValues, out yValues);

            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts(chartName);
            chart.SetXAxis(new XAxis
            {
                Categories = xValues.ToArray(),
            });
            chart.SetSeries(new DotNet.Highcharts.Options.Series
            {
                Data = new Data(yValues.ToArray()),
                Name = chartName
            });
            chart.SetTitle(new DotNet.Highcharts.Options.Title { Text = chartName });
            return chart;
        }


        public ActionResult ClientVersionChart()
        {
            List<string> xValues = new List<string>();
            List<Object> yValues = new List<Object>();
            BlobStorageService.GetJsonDataFromBlob("ClientVersionWeeklyReport.json", out xValues, out yValues);
            
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("DownloadsPerClient")
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
            chart.SetSeries(new DotNet.Highcharts.Options.Series
            {
                Data = new Data(yValues.ToArray()),
                Name = "DownloadsPerClient"
            });

            chart.SetTitle(new DotNet.Highcharts.Options.Title { Text = "Downloads Per Client last week" });
            return View("~/Views/Trending/PackagesChart.cshtml", chart);
        }
       

    }
}
