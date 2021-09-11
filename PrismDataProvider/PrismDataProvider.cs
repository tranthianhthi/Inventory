using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace PrismDataProvider
{
    public class PrismDataProvider
    {
        private PrismConnection connection;

        public PrismDataProvider(PrismConnection connection)
        {
            this.connection = connection;
        }

        public DataTable executeRead(string queryString, DateTime fromDate, DateTime toDate)
        {
            DataTable tb = new DataTable();
            return tb;
        }


        public async Task<DataTable> executeThreadingQueryDataAsync(string queryString, DateTime fromDate, DateTime toDate, int totalThreads = 2)
        {
            DataTable tb = new DataTable();

            double totalDays = (toDate.Date - fromDate.Date).TotalDays;
            

            if (totalDays > 14)
            {
                List<TimeRange> timeRanges = new List<TimeRange>();
                int step = (int)(totalDays / totalThreads);
                int thread = 0;
                while (thread < totalThreads)
                {
                    DateTime ttDate = fromDate.AddDays(step);
                    ttDate = ttDate > toDate ? toDate : ttDate;

                    timeRanges.Add(new TimeRange(fromDate, ttDate));
                    fromDate = ttDate.AddDays(1);

                    thread += 1;
                }

                IEnumerable<Task<DataTable>> getdataTasksList = from timeRange in timeRanges
                                                            select executeQuery(queryString, timeRange, connection);

                List<Task<DataTable>> queryTasks = getdataTasksList.ToList();

                while (queryTasks.Any())
                {
                    Task<DataTable> finishedTask = await Task.WhenAny(queryTasks);
                    tb.Merge(finishedTask.Result);
                    queryTasks.Remove(finishedTask);
                }

            }

            return tb;
        }

        private async Task<DataTable> executeQuery(string queryString, TimeRange timeRange, PrismConnection connection)
        {
            return await Task.Run(() =>
                {
                    DataTable tb = new DataTable();

                    string query = queryString.Replace("fDate", timeRange.fDate.ToString("MM/dd/yyyy")).Replace("tDate", timeRange.tDate.ToString("MM/dd/yyyy"));
                    using (OracleConnection conn = connection.GetConnection()) ;
                    OracleCommand cmd = new OracleCommand(query, connection.GetConnection());
                    OracleDataAdapter da = new OracleDataAdapter(cmd);

                    try
                    {
                        da.Fill(tb);
                    }
                    catch
                    {

                    }

                    return tb;
                });
        }
    }
}