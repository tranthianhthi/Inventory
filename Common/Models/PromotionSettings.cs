using System;

namespace ACFC.Common
{
    public class PromotionSettings
    {
        private string sbsNo;
        private string vendorCode;
        private string promotionFilter;

        private string networkPromotionFolder;
        private string localPromotionFolder;
        private string networkOutletPromotionFolder;
        private string localOutletPromotionFolder;

        public PromotionSettings(string sbsNo, string vendorCode, string promotionFilter, string networkPromotionFolder, string localPromotionFolder, string networkOutletPromotionFolder, string localOutletPromotionFolder)
        {
            this.sbsNo = sbsNo;
            this.vendorCode = vendorCode;
            this.promotionFilter = promotionFilter;
            this.networkPromotionFolder = networkPromotionFolder;
            this.localPromotionFolder = localPromotionFolder;
            this.networkOutletPromotionFolder = networkOutletPromotionFolder;
            this.localOutletPromotionFolder = localOutletPromotionFolder;
        }

        public string SbsNo { get => sbsNo; set => sbsNo = value; }
        public string VendorCode { get => vendorCode; set => vendorCode = value; }
        public string PromotionFilter { get => promotionFilter; set => promotionFilter = value; }
        public string NetworkPromotionFolder { get => networkPromotionFolder; set => networkPromotionFolder = value; }
        public string LocalPromotionFolder { get => localPromotionFolder; set => localPromotionFolder = value; }
        public string NetworkOutletPromotionFolder { get => networkOutletPromotionFolder; set => networkOutletPromotionFolder = value; }
        public string LocalOutletPromotionFolder { get => localOutletPromotionFolder; set => localOutletPromotionFolder = value; }
    }
}
