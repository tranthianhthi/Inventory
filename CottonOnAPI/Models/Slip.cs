using System;
using System.Data;

namespace CottonOnAPI.Models
{
    public class Slip
    {
        public long slip_sid { get; set;  }
        public int sbs_no { get; set; }
        public int out_store_no { get; set; }
        public int in_sbs_no { get; set; }
        public int in_store_no { get; set; }
        public int slip_no { get; set; }
        public int slip_type { get; set; }
        public int unverified { get; set; }
        public int out_slip_no { get; set; }
        public int to_no { get; set; }
        public int orig_store_no { get; set; }
        public int orig_station { get; set; }
        public int clerk_id { get; set; }
        public int status { get; set; }


        public Slip()
        {

        }

        public Slip(DataRow row)
        {
            this.slip_sid = row["slip_sid"] == DBNull.Value? 0 : long.Parse(row["slip_sid"].ToString());
            this.sbs_no = row["sbs_no"] == DBNull.Value ? 0 : int.Parse(row["sbs_no"].ToString());
            this.out_store_no = row["out_store_no"] == DBNull.Value ? 0 : int.Parse(row["out_store_no"].ToString());
            this.in_sbs_no = row["in_sbs_no"] == DBNull.Value ? 0 : int.Parse(row["in_sbs_no"].ToString());
            this.in_store_no = row["in_store_no"] == DBNull.Value ? 0 : int.Parse(row["in_store_no"].ToString());
            this.slip_no = row["slip_no"] == DBNull.Value ? 0 : int.Parse(row["slip_no"].ToString());
            this.slip_type = row["slip_type"] == DBNull.Value ? 0 : int.Parse(row["slip_type"].ToString());
            this.unverified = row["unverified"] == DBNull.Value ? 0 : int.Parse(row["unverified"].ToString());
            this.out_slip_no = row["out_slip_no"] == DBNull.Value ? 0 : int.Parse(row["out_slip_no"].ToString());
            this.to_no = row["to_no"] == DBNull.Value ? 0 : int.Parse(row["to_no"].ToString());
            this.orig_store_no = row["orig_store_no"] == DBNull.Value ? 0 : int.Parse(row["orig_store_no"].ToString());
            this.orig_station = row["orig_station"] == DBNull.Value ? 0 : int.Parse(row["orig_station"].ToString());
            this.clerk_id = row["clerk_id"] == DBNull.Value ? 0 : int.Parse(row["clerk_id"].ToString());
            this.status = row["status"] == DBNull.Value ? 0 : int.Parse(row["status"].ToString());
        }
        
    }
}
