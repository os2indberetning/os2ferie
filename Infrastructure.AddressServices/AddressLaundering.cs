using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Infrastructure.AddressServices.Classes;
using Newtonsoft.Json.Linq;

namespace Infrastructure.AddressServices
{
    public static class AddressLaundering
    {
        /// <summary>
        /// Check an address for errors in spelling and adjust case of street names and building identfiers. If laundering fails an exception is thrown.
        /// </summary>
        /// <param name="address"></param>
        /// <exception cref="AddressLaunderingException">Thrown if no address was returned or if the address could not be validated(see exception message and code).</exception>
        /// <returns>Corrected address.</returns>
        public static Address LaunderAddress(Address address)
        {
            var request = CreateRequest(address.StreetName, address.StreetNumber, address.ZipCode);

            var laundered = ExecuteAndRead(request);

            if (!laundered.Any())
            {
                throw new AddressLaunderingException("The laundering process did not return any elements.", 0);
            }

            foreach (var result in laundered)
            {

                if (result.validateresult <= 1000 && result.validateresult >= 100)
                {
                    address.StreetName = result.laundered_address.streetname;
                    address.StreetNumber = result.laundered_address.streetbuildingidentifier;

                    return address;
                }
                if(result.validateresult < 0)
                {
                    throw new AddressLaunderingException(result.validatedescription, result.validateresult);
                }
            }

            return address;
        }

        /// <summary>
        /// Create a request following the service API url specifications. (addressevask.dk)
        /// </summary>
        /// <param name="street"></param>
        /// <param name="streetNr"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateRequest(string street, string streetNr, string zipCode)
        {
            var query = string.Format("[\"{0} {1}, {2}\"]", street, streetNr, zipCode);

            return (HttpWebRequest)WebRequest.Create(UrlDefinitions.LaunderingUrl + query);
        }

        private static List<RootLaunderedObject> ExecuteAndRead(HttpWebRequest request)
        {
            var responseString = "";

            var distanceResponse = request.GetResponse();
            var responseStream = distanceResponse.GetResponseStream();

            if (responseStream == null) return null;

            var streamReader = new StreamReader(responseStream);
            responseString = streamReader.ReadToEnd();
            streamReader.Close();
            return ParseJson(responseString);
        }

        private static List<RootLaunderedObject> ParseJson(string response)
        {
            List<RootLaunderedObject> laundered = new List<RootLaunderedObject>();
            JObject jObject = JObject.Parse(response);

            if (jObject == null)
            {
                return laundered;
            }

            JToken jLaunder = jObject["results"];

            if (jLaunder == null)
            {
                return laundered;
            }

            foreach (var jToken in jLaunder.ToArray())
            {
                if (jToken == null)
                {
                    continue;
                }

                var jLaundered = jToken["laundered"];

                if (jLaundered == null)
                {
                    continue;
                }

                var launderedAddress = new RootLaunderedObject
                {
                    input = (string)jLaundered["input"],
                    validateresult = (int)jLaundered["validateresult"],
                    validatedescription = (string)jLaundered["validatedescription"],
                    laundered_address = new LaunderedAddress()
                };

                var jAddress = jLaundered["laundered_address"];

                launderedAddress.laundered_address.streetbuildingidentifier = (string)jAddress["streetbuildingidentifier"];
                launderedAddress.laundered_address.streetname = (string)jAddress["streetname"];
                launderedAddress.laundered_address.postcode = (string) jAddress["postcode"];

                laundered.Add(launderedAddress);
            }

            return laundered;
        }
       
    }
}
