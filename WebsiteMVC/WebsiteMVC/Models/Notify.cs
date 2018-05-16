using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteMVC.Models
{
    public class Notify
    {
        public static byte CANCLE = 0;
        public static byte ACTIVE = 1;
        public static byte INIT = 2;
        public static byte SEEN = 3;
        public static byte SUCCESS = 4;
        public static byte ORTHER = 5;

        public string Type { get; set; }
        public string Message { get; set; }
        public int ID { get; set; }
        public int State { get; set; }
        public DateTime? CreateTime { get; set; }
        public TaiKhoan CreateBy { get; set; }
        public string ControllerName { get; set; }
    }
}