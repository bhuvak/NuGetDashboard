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


namespace NuGetDashboard.Controllers
{
    /// <summary>
    /// This controller handles the requests related to pingdom checks.
    /// </summary>
    public class PingdomChecksController : Controller
    {
        public ActionResult Index()
        {
            List<PingdomStatusViewModel> checks = new List<PingdomStatusViewModel>();
            NetworkCredential nc = new NetworkCredential(ConfigurationManager.AppSettings["PingdomUserName"], ConfigurationManager.AppSettings["PingdomPassword"]);
            WebRequest request = WebRequest.Create("https://api.pingdom.com/api/2.0/checks");
            request.Credentials = nc;
            request.Headers.Add(ConfigurationManager.AppSettings["PingdomAppKey"]);
            request.PreAuthenticate = true;
            request.Method = "GET";
            WebResponse respose = request.GetResponse();
            using (var reader = new StreamReader(respose.GetResponseStream()))
            {               
                JavaScriptSerializer js = new JavaScriptSerializer();
                var objects = js.Deserialize<dynamic>(reader.ReadToEnd());
                foreach (var o in objects["checks"])
                {
                    if (o["name"].ToString().Contains("curated"))
                        continue;
                  //Dictionary<string,int> summaryValues = GetCheckSummaryAvgForLastMonth(o["id"], nc);                                   
                 // checks.Add(new PingdomCheckViewModel(o["name"], o["status"], o["lastresponsetime"],o["lasterrortime"],summaryValues["avgresponse"],summaryValues["totalup"]));                  
                  checks.Add(new PingdomStatusViewModel(o["name"], o["status"], o["lastresponsetime"], o["lasterrortime"]));                  
                }
            }            
            return View(checks);
        }

        public ActionResult PartialMonthlyReport()
        {
            string value = Request.Form.Get("Reports");
            if(string.IsNullOrEmpty(value))
            {
                value = DateTimeUtility.GetLastMonthName();
            }
            string[] checkNames = new string[] { "feed", "home", "packages" };
            List<PingdomMonthlyReportViewModel> reports = new List<PingdomMonthlyReportViewModel>();
            foreach (string check in checkNames)
            {
                //Get the response values from pre-created blobs for each check.
                string blobName = check + value + "MonthlyReport.json";
                Dictionary<string,string> dict =  BlobStorageService.GetDictFromBlob(blobName);
                double upTime = Convert.ToDouble(dict["totalup"]);
                int avgResponse = Convert.ToInt32(dict["avgresponse"]);
                int Outages = Convert.ToInt32(dict[ "Outages"]);
                int downTime = Convert.ToInt32(dict["totaldown"]);            
                reports.Add(new PingdomMonthlyReportViewModel(check,upTime,downTime,Outages,avgResponse,value));

            }
            ViewBag.SelectedValue = value;
            return View(reports);
        }

        //Returns the chart for Average response for the last week
        public ActionResult PartialAvgResponse()
        {
            string[] checkNames = new string[] { "feed", "home", "packages" };
            List<DotNet.Highcharts.Options.Series> seriesSet = new List<DotNet.Highcharts.Options.Series>();
            List<string> xValues = new List<string>();
            List<Object> yValues = new List<Object>();
            foreach (string check in checkNames)
            {                               
                //Get the response values from pre-created blobs for each check.
                BlobStorageService.GetJsonDataFromBlob(check +"WeeklyReport.json" , out xValues, out yValues);
                seriesSet.Add(new DotNet.Highcharts.Options.Series
                {
                    Data = new Data(yValues.ToArray()),
                    Name = check
                });
            }
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("AvgResponseTime");
            chart.SetXAxis(new XAxis
            {
                Categories = xValues.ToArray()
            });
            chart.SetSeries(seriesSet.ToArray());
            chart.SetTitle(new DotNet.Highcharts.Options.Title { Text = "Avg Response time during last week" });            
            return PartialView(chart);
        }


        #region PrivateMethods

        //private static Dictionary<string, int> GetCheckSummaryAvgForLastMonth(int checkId, NetworkCredential nc)
        //{
        //    //Get the from to values based on UNIX time stamp.
        //    long currentTime = DateTimeUtility.GetCurrentUnixTimestampSeconds();
        //    long lastMonth = DateTimeUtility.GetLastMonthUnixTimestampSeconds();
        //    WebRequest request = WebRequest.Create(string.Format("https://api.pingdom.com/api/2.0/summary.average/{0}?includeuptime=true&from={1}&to={2}", checkId, lastMonth, currentTime));
        //    request.Credentials = nc;
        //    request.Headers.Add(ConfigurationManager.AppSettings["PingdomAppKey"]);
        //    request.PreAuthenticate = true;
        //    request.Method = "GET";
        //    Dictionary<string, int> summaryValues = new Dictionary<string, int>();
        //    WebResponse respose = request.GetResponse();
        //    using (var reader = new StreamReader(respose.GetResponseStream()))
        //    {
        //        JavaScriptSerializer js = new JavaScriptSerializer();
        //        var summaryObject = js.Deserialize<dynamic>(reader.ReadToEnd());
        //        foreach (var summary in summaryObject["summary"])
        //        {
        //            foreach (var status in summary.Value)
        //            {
        //                summaryValues.Add(status.Key, status.Value);
        //            }
        //        }
        //    }
        //    return summaryValues;
        //}

        #endregion PrivateMethods

    }
}
