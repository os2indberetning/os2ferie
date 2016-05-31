using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainServices
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static long ToTimestamp(this DateTime date)
        {
            return (long) date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

    }
}
