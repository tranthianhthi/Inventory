using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ACFC.Common.Models
{
    public class Store
    {
        int sbsNo;
        int storeNo;
        string storeCode;
        string storeName;

        public Store(DataRow row)
        {
            SbsNo = int.Parse(row["sbs_no"].ToString());
            StoreNo = int.Parse(row["store_no"].ToString());
            StoreCode = row["store_code"].ToString();
            StoreName = row["store_name"].ToString();
        }

        public Store(int sbsNo, int storeNo, string storeCode, string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeCode))
            {
                throw new ArgumentException("Store code cannot be null.", nameof(storeCode));
            }

            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException("Store name cannot be null.", nameof(storeName));
            }

            this.sbsNo = sbsNo;
            this.storeNo = storeNo;
            this.storeCode = storeCode;
            this.storeName = storeName;
        }

        public int SbsNo { get => sbsNo; set => sbsNo = value; }
        public int StoreNo { get => storeNo; set => storeNo = value; }
        public string StoreCode { get => storeCode; set => storeCode = value; }
        public string StoreName { get => storeName; set => storeName = value; }
    }
}
