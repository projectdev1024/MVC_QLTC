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
        public decimal? TyLe => Payed * 100 / (Payed + ConNo);
        public decimal? Tra1Ngay => (int)((SoTien / (NgayTra - CreateTime).Value.Days) * (100 + (decimal)LaiSuat) / 100);
        public decimal? PhaiTra => Payed + (NgayTra - DateTime.Now).Value.Days * Tra1Ngay;
    }

    public partial class ThanhToan
    {
        public int? Timed => (DateTime.Now - CongNo.CreateTime)?.Days;
    }

    partial class ThuChi
    {
        public bool Thu { get; set; }
    }

}