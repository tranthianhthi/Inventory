using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace OnlineInventoryLib.Shopee.Models
{
    public class ShopeeVariation
    {
        /// <summary>
        /// ID phân loại shopee
        /// </summary>
        public UInt64 variation_id { get; set; }

        /// <summary>
        /// SKU phân nhóm trên shopee ( ~ UPC trên Prism )
        /// </summary>
        [JsonIgnore]
        public string variation_sku { get; set; }

        /// <summary>
        /// Số lượng on-hand của Prism 
        /// </summary>
        [JsonIgnore]
        public int Prism_on_hand { get; set; }

        /// <summary>
        /// Số lượng giữ lại store
        /// </summary>
        [JsonIgnore]
        public int keep_offline { get; set; }


        /// <summary>
        /// Hàm set sku phân loại
        /// </summary>
        [JsonProperty("variation_sku")]
        public string Setvariation_sku { set { variation_sku = value; } }


        /// <summary>
        /// Tên phân loại
        /// </summary>
        [JsonIgnore]
        public string variation_name { get; set; }

        /// <summary>
        /// Số lượng bán on store
        /// </summary>
        [DefaultValue(null)]
        public int stock { get; set; }

        /// <summary>
        /// ID item ( ID của nhóm category ) 
        /// </summary>
        [DefaultValue(null)]
        public UInt64 item_id { get; set; }

        /// <summary>
        /// Format SKU trên Shopee ( SKU trên Shopee có thể là desc1_UPC hoặc UPC hoặc định dạng khác )
        /// </summary>
        [JsonIgnore]
        public string shopeeVariationSKUFormat { get; set; }

        /// <summary>
        /// Dấu phân cách giữa các field Prism trên Shopee
        /// </summary>
        [JsonIgnore]
        public string shopeeVariationSKUSeparator { get; set; }

        /// <summary>
        /// Prism UPC ( tính toán từ SKU Code & format SKU & phân cách field ) 
        /// </summary>
        [JsonIgnore]
        public string PrismUPC
        {
            get
            {
                if (string.IsNullOrEmpty(shopeeVariationSKUFormat))
                    return variation_sku;
                else
                {

                    if (!shopeeVariationSKUFormat.Contains("(UPC)"))
                        return "";
                    //throw new ArgumentException("SKU Format - Missing (UPC) in format string.");

                    if (string.IsNullOrEmpty(shopeeVariationSKUSeparator.Trim()))
                        return "";
                    //throw new ArgumentException("SKU Separator - Missing separator.");

                    string[] fields = shopeeVariationSKUFormat.Split(shopeeVariationSKUSeparator.ToCharArray());
                    int index = 0;
                    string field;
                    while (index < fields.Length)
                    {
                        field = fields[index];
                        if (string.Compare(field, "(UPC)", true) == 0)
                        {
                            break;
                        }
                        index += 1;
                    }

                    fields = variation_sku.Split(shopeeVariationSKUSeparator.ToCharArray());
                    if (fields.Length <= index)
                    {
                        //throw new Exception("Data is not in valid format.");
                        return "";
                    }
                    return fields[index];
                }
            }
            set { }
        }

    }
}
