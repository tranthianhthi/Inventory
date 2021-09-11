using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACFC.Common.Models
{
    public class Brand
    {
        string brandName;
        string prefixChar;
        int sbsNo;

        public Brand(string brandName, string prefixChar, int sbsNo)
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                throw new ArgumentException("message", nameof(brandName));
            }

            if (string.IsNullOrWhiteSpace(prefixChar))
            {
                throw new ArgumentException("message", nameof(prefixChar));
            }

            this.BrandName = brandName;
            this.PrefixChar = prefixChar;
            this.SbsNo = sbsNo;
        }

        public string BrandName { get => brandName; set => brandName = value; }
        public string PrefixChar { get => prefixChar; set => prefixChar = value; }
        public int SbsNo { get => sbsNo; set => sbsNo = value; }
    }
}
