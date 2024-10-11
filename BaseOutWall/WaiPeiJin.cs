using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseOutWall
{
    public class WaiPeiJin
    {
        private double tf = 0.167;
        private double Ef = 230000;
        public double Zhijin { get; set; }
        public double Duoyu { get; set; }
        public double Jianju { get; set; }
        public string Section { get; set; }
        public bool isImportant { get; set; }
        private double ff { get => isImportant ? 1600 : 2300; }
        //private double Km { get => Math.Min(1.16 - this.TLayers * this.Ef * this.tf / 308000, 0.9); }
        //public double NCapacity { get => this.Width * this.tf * this.TLayers * this.Km * this.ff * this.locationNum; }
        public WaiPeiJin(double zhijin, double jianju, double duoyu)
        {
            this.Zhijin = zhijin;
            this.Jianju = jianju;
            this.Duoyu = duoyu;
        }
    }
}
