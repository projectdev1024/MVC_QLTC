using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteMVC.Models
{
    partial class Quy
    {
        public string mName { get => $"{MaQuy} {(Money ?? 0).ToString("#,###")}VND"; }
    }

    partial class CongNo
    {
        public bool Cong { get; set; }
    }

    partial class ThuChi
    {
        public bool Thu { get; set; }
    }

    partial class CongNo
    {
        public decimal PhaiTra { get => (SoTien * (100 + (decimal)LaiSuat) / 100) ?? 0; }
    }
}