﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuGetDashboard.Utilities;

namespace NuGetDashboard.Models
{
    /// <summary>
    /// This class represents a check in pingdom
    /// </summary>
    public class PingdomStatusViewModel
    {
        public PingdomStatusViewModel(string name,string status, int lastResponseTime,int lastErrorTime)
        {
            this.Name = name;
            this.Status = status;
            this.LastResponseTime = lastResponseTime;
            this.LastErrorTime = DateTimeUtility.DateTimeFromUnixTimestampSeconds(lastErrorTime);
           // this.AvgResponseTime = avgResponseTime;
           // this.totalUpTime = Math.Round( ((double)totalUpTime / (UnixTimeStampUtility.GetSecondsFor30Days())) * 100 , 2);
        }

        public string Name;
        public int Id;
        public int LastResponseTime;
        public string Status;
        public DateTime LastErrorTime;
        public int AvgResponseTime;
        public double totalUpTime;       

    }
}