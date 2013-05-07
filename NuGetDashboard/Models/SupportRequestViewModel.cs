using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetDashboard.Models
{
    /// <summary>
    /// This class models a support request (TFS work item).
    /// </summary>
    public class SupportRequestViewModel
    {
        public SupportRequestViewModel(int id, string title, DateTime dateCreated, string status,string url,string history= null)
        {
            this.Id = id;
            this.Title = title;
            this.DateCreated = dateCreated;
            this.Status = status;
            this.History = history;
            this.Url = url;
        }
               
        public string Title;
        public int Id;
        public string Url;
        public DateTime DateCreated;
        public string Status;
        public string History;

    }
}