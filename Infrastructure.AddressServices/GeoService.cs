using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Infrastructure.AddressServices
{

    // Stolen from here:
    // https://github.com/jieter/Leaflet.encoded/blob/master/Polyline.encoded.js

    public class GeoService
    {
        public static string Encode(List<DriveReportPoint> points)
        {
            var flatPoints = new List<double>();
            foreach (var point in points)
            {
                flatPoints.Add(Convert.ToDouble(point.Latitude, CultureInfo.InvariantCulture));
                flatPoints.Add(Convert.ToDouble(point.Longitude, CultureInfo.InvariantCulture));
            }
            return encodeDeltas(flatPoints);

        }

        private static string encodeDeltas(List<double> numbers)
        {
            var lastNumbers = new double[numbers.Count].ToList();

            for (var i = 0; i < numbers.Count; )
            {
                for (var d = 0; d < 2; d++, i++)
                {
                    var num = numbers[i];
                    var delta = new double();
                    if (lastNumbers.Count == 0)
                    {
                        delta = num;
                    }
                    else
                    {
                        delta = num - (lastNumbers[d]);
                    }
                    
                    lastNumbers[d] = num;

                    numbers[i] = delta;
                }
            }
            return encodeFloats(numbers);
        }

        private static string encodeFloats(List<double> numbers)
        {

            var intList = new Int64[numbers.Count].ToList();
            for (var i = 0; i < numbers.Count; ++i)
            {
                intList[i] = Convert.ToInt64(Math.Round(numbers[i] * Math.Pow(10, 5)));
            }
            return encodeSignedIntegers(intList);
        }

        private static string encodeSignedIntegers(List<Int64> numbers)
        {
            for (var i = 0; i < numbers.Count; ++i)
            {
                var num = numbers[i];
                numbers[i] = (num < 0) ? ~(num << 1) : (num << 1);
            }
            return encodeUnsignedIntegers(numbers);
        }

        private static string encodeUnsignedIntegers(List<Int64> numbers)
        {
            var encoded = "";
            for (var i = 0; i < numbers.Count; ++i)
            {
                encoded += encodeUnsignedInteger(numbers[i]);
            }
            return encoded;
        }

        private static string encodeUnsignedInteger(Int64 num)
        {
            long value;
            var encoded = "";
            while (num >= 0x20)
            {
                value = (0x20 | (num & 0x1f)) + 63;
                var bytes = new byte[1] {(byte)(value)};
                encoded += Encoding.UTF8.GetString(bytes)[0];
                num >>= 5;
            }
            value = (num + 63);
            encoded += Encoding.UTF8.GetString(new byte[1] {(byte) (value)})[0];
            return encoded;
        }
    }
}
