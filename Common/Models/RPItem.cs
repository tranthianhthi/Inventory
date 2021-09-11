using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACFC.Common.Models
{
    public class RPItem
    {
        string upc;
        string alu;
        string description1;
        string description2;
        string description3;
        string description4;
        string attr;
        string delimiter;

        public string Upc { get => upc; set => upc = value; }
        public string Alu { get => alu; set => alu = value; }
        public string Description1 { get => description1; set => description1 = value; }
        public string Description2 { get => description2; set => description2 = value; }
        public string Description3 { get => description3; set => description3 = value; }
        public string Description4 { get => description4; set => description4 = value; }
        public string Attr { get => attr; set => attr = value; }
        public string Delimiter { get => delimiter; set => delimiter = value; }
        public string StyleCC => string.Format("{0}{1}{2}", description1, delimiter, attr);

        public RPItem(string upc, string alu, string description1, string description2, string description3, string description4, string attr, string delimiter)
        {
            this.upc = upc;
            this.alu = alu;
            this.description1 = description1;
            this.description2 = description2;
            this.description3 = description3;
            this.description4 = description4;
            this.attr = attr;
            this.delimiter = delimiter;
        }



    }
}
