using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetDashboard.Models
{
    public class PackagesViewModel
    {
        public PackagesViewModel(string id, string url)
        {
            this.Id = id;
            this.GalleryUrl = url;
        }

        public string Id;
        public string GalleryUrl;

    }
}