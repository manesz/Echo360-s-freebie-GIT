using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Freebie.ViewModels
{
    public class AcctInfoQuota
    {
        public string Quota_Cd { get; set; }
        public string Quota_Type_Cd { get; set; }
        public byte? Quota_Freq_Val { get; set; }
        public byte? Quota_Dur_Val { get; set; }
        public System.DateTime? Created_Dttm { get; set; }
        public System.DateTime? Start_Dttm { get; set; }
        public System.DateTime? End_Dttm { get; set; }
    }
}