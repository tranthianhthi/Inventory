using System;
using System.Collections.Generic;
using System.Text;

namespace PrismDataProvider
{
    class TimeRange
    {
        public DateTime fDate;
        public DateTime tDate;

        public TimeRange(DateTime fDate, DateTime tDate)
        {
            if (tDate < fDate)
            {
                throw new ArgumentException($"'{nameof(fDate)}' cannot be greater than '{nameof(tDate)}'", nameof(fDate));
            }
            this.fDate = fDate;
            this.tDate = tDate;
        }
    }
}
