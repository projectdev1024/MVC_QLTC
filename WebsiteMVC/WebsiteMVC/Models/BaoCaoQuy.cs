using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteMVC.Models
{
    public class BaoCaoQuy
    {
        public string Code { get; set; }
        public DateTime? CreateTime { get; set; }
        public int Money { get; set; }
        public Quy Quy { get; set; }
    }
}