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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Returns the Net trend chart for packages 
        public ActionResult PackagesChart()
        {
            DotNet.Highcharts.Highcharts chart = GetChart("UploadDataPoints.json", "Packages");
            return PartialView("~/Views/Home/PackagesChart.cshtml", chart);
        }

        //Returns the Net trend chart for Downloads
        public ActionResult DownloadsChart()
        {
            DotNet.Highcharts.Highcharts chart = GetChart("NetDownloadTrend.json", "Downloads");           
            return PartialView("~/Views/Home/PackagesChart.cshtml", chart);
        }

        //Returns the top 10 packages in the last 6 weeks.
        public ActionResult TopPackages()
        {
            WebRequest request = WebRequest.Create("http://www.nuget.org/api/v2/stats/downloads/last6weeks?count=10");          
            request.Method = "GET";
            List<PackagesViewModel> packages = new List<PackagesViewModel>();
            WebResponse respose = request.GetResponse();
            using (var reader = new StreamReader(respose.GetResponseStream()))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var summaryObject = js.Deserialize<dynamic>(reader.ReadToEnd());
                foreach (var summary in summaryObject)
                {                   
                        packages.Add(new PackagesViewModel(summary["PackageId"],summary["Gallery"]));
                   
                }
            }
            return PartialView("~/Views/Home/TopPackages.cshtml", packages);
        }


        #region PrivateMethod
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

        #endregion PrivateMathod
    }
}
