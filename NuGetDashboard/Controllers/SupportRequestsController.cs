using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SupportRequestTicketingService;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Common;
using NuGetDashboard.Utilities;
using NuGetDashboard.Models;

namespace NuGetDashboard.Controllers
{
    public class SupportRequestsController : Controller
    {        
        public ActionResult Index()
        {
            WorkItemHelper helper = new WorkItemHelper();
            var newRequests = helper.GetWorkItems("New");
            List<SupportRequestViewModel> requestModels = new List<SupportRequestViewModel>();
            foreach (WorkItem item in newRequests)
            {
                requestModels.Add(new SupportRequestViewModel(item.Id,item.Title,item.CreatedDate,item.State,helper.GetLinkForWorkItemId(item.Id)));
            }
            return View(requestModels);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            WorkItemHelper helper = new WorkItemHelper();
            var currentRequest = helper.GetWorkItem(id);
            SupportRequestViewModel requestModel = new SupportRequestViewModel(currentRequest.Id,currentRequest.Title,currentRequest.CreatedDate,currentRequest.State, helper.GetLinkForWorkItemId(currentRequest.Id), currentRequest.History);
            return View(requestModel);
        }


        [HttpPost]
        public ActionResult Edit()
        {
            WorkItemHelper helper = new WorkItemHelper();       
            int bugId = Convert.ToInt32(Request.Form.Get("Id"));
            var currentRequest = helper.UpdateWorkItem(bugId, Request.Form.Get("Comment"));
            MailMonitor.SendMailForIssue(bugId);
            return RedirectToAction("Index", "SupportRequests");
        }


    }
}
