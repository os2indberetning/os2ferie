using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Infrastructure.AddressServices.Classes;
using Newtonsoft.Json.Linq;

namespace Infrastructure.AddressServices
{
    public static class AddressCoordinates
    {
        /// <summary>
        /// Specifies number of decimals used for coordinates.
        /// </summary>
        const int COORD_DECIMALS = 4;

        /// <summary>
        /// Get coordinates for a given addresses.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        /// <exception cref="AddressCoordinatesException">Thrown if address has critical spelling erros(see inner exception) or if no address coordinates correspond to the entered address.</exception>
        /// <returns></returns>
        public static Coordinates GetAddressCoordinates(Address address, Coordinates.CoordinatesType type = Coordinates.CoordinatesType.Unkown)
        {
            Address correctedAddress;
            try
            {
                correctedAddress = AddressLaundering.LaunderAddress(address);
            }
            catch (AddressLaunderingException e)
            {
                throw new AddressCoordinatesException("Errors in address, see inner exception.", e);
            }

            var request = CreateRequest(correctedAddress.Street, correctedAddress.StreetNr, correctedAddress.ZipCode);

            var addresses = ExecuteAndRead(request);

            if(!addresses.Any())
            {
                request = CreateRequest(correctedAddress.Street, null, correctedAddress.ZipCode);

                addresses = ExecuteAndRead(request);
            }

            var addressCoordinates = new Coordinates {Type = type};

            if (addresses[0].adgangsadresse.vejstykke.navn == correctedAddress.Street 
                && addresses[0].adgangsadresse.postnummer.nr == correctedAddress.ZipCode)
            {
                addressCoordinates.Longitude = addresses[0].adgangsadresse.adgangspunkt.koordinater[0].ToString().Replace(",", ".");
                addressCoordinates.Latitude = addresses[0].adgangsadresse.adgangspunkt.koordinater[1].ToString().Replace(",", ".");

                addressCoordinates.Longitude = addressCoordinates.Longitude.Remove(addressCoordinates.Longitude.IndexOf('.') + 1 + COORD_DECIMALS);
                addressCoordinates.Latitude = addressCoordinates.Latitude.Remove(addressCoordinates.Latitude.IndexOf('.') + 1 + COORD_DECIMALS);
            }
            else
            {
                throw new AddressCoordinatesException("The addresses returned differ highly from the original, streetname does not exist in zipcode area.");
            }

            return addressCoordinates;
        }

        /// <summary>
        /// Create a request following the service API url specifications. (dawa.aws.dk)
        /// </summary>
        /// <param name="street">Uppercase street name</param>
        /// <param name="streetNr">Uppercase street number</param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateRequest(string street, string streetNr, string zipCode)
        {
            var query = streetNr == null ? string.Format("vejnavn={0}&postnr={1}", street, zipCode) : string.Format("vejnavn={0}&husnr={1}&postnr={2}", street, streetNr, zipCode);

            return (HttpWebRequest)WebRequest.Create(UrlDefinitions.CoordinatesUrl + query);
        }

        /// <summary>
        /// Execute HTTP request and read the response.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Formatted response from service.</returns>
        private static List<RootAddressObject> ExecuteAndRead(HttpWebRequest request)
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

        /// <summary>
        /// Formats and structures the response. 
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Response reprensented in custom class.</returns>
        private static List<RootAddressObject> ParseJson(string response)
        {
            List<RootAddressObject> addressObject = new List<RootAddressObject>();
            JArray jAddressList = JArray.Parse(response);

            if (jAddressList == null)
            {
                return addressObject;
            }

            foreach (JToken address in jAddressList)
            {
                var tmpAddress = new RootAddressObject
                {
                    status = (int) address["status"],
                    adressebetegnelse = (string) address["adressebetegnelse"],
                    adgangsadresse = new Adgangsadresse()
                };

                JToken accessObject = address["adgangsadresse"];

                tmpAddress.adgangsadresse.vejstykke = new Vejstykke(accessObject["vejstykke"]);
                tmpAddress.adgangsadresse.postnummer = new Postnummer(accessObject["postnummer"]);
                tmpAddress.adgangsadresse.husnr = (string)accessObject["husnr"];
                tmpAddress.adgangsadresse.adgangspunkt = new Adgangspunkt(accessObject["adgangspunkt"]);

                addressObject.Add(tmpAddress);
            }

            return addressObject;
        }
    }
}
