using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices.Interfaces;
using Newtonsoft.Json.Linq;
using Core.DomainModel;

namespace Infrastructure.AddressServices
{
    public class AddressLaundering : IAddressLaunderer
    {
        #region Public methods

        /// <summary>
        /// Check an address for errors in spelling and adjust case of street names and building identfiers. If laundering fails an exception is thrown.
        /// </summary>
        /// <param name="address"></param>
        /// <exception cref="AddressLaunderingException">Thrown if no address was returned or if the address could not be validated(see exception message and code).</exception>
        /// <returns>Corrected address.</returns>
        public Address LaunderAddress(Address address)
        {
            var request = CreateRequest(address.StreetName, address.StreetNumber, address.ZipCode.ToString());
            var laundered = ExecuteAndRead(request);
            
            if ( laundered == null)
            {
                throw new AddressLaunderingException("The laundering process did not return any elements.", 0);
            }

            address.StreetName = laundered.vejstykke.navn;
            address.StreetNumber = laundered.husnr;
            address.ZipCode = Convert.ToInt32(laundered.postnummer.nr);
            address.Town = laundered.postnummer.navn;
            address.Longitude = laundered.adgangspunkt.koordinater[0];
            address.Latitude = laundered.adgangspunkt.koordinater[1];

            return address;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Create a request following the service API url specifications. (addressevask.dk)
        /// </summary>
        /// <param name="street"></param>
        /// <param name="streetNr"></param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest(string street, string streetNr, string zipCode)
        {
            street = char.ToUpper(street[0]) + street.Substring(1);
            streetNr = streetNr.Split(',')[0];
            streetNr = streetNr.ToUpper();
            streetNr = streetNr.Replace(" ", "");
            var query = string.Format("vejnavn={0}&husnr={1}&postnr={2}", street, streetNr, zipCode);

            return (HttpWebRequest)WebRequest.Create(UrlDefinitions.LaunderingUrl + query);
        }

        private Adgangsadresse ExecuteAndRead(HttpWebRequest request)
        {
            var responseString = "";

            Stream responseStream;
            try
            {
                var distanceResponse = request.GetResponse();
                responseStream = distanceResponse.GetResponseStream();
            }
            catch (WebException e)
            {
                throw new AddressLaunderingException("Server error, request invalid.", e);
            }

            if (responseStream == null) return null;

            var streamReader = new StreamReader(responseStream);
            responseString = streamReader.ReadToEnd();
            streamReader.Close();
            return ParseJson(responseString);
        }

        private Adgangsadresse ParseJson(string response)
        {
            JArray jObject = JArray.Parse(response);

            if (jObject.Count != 1)
            {
                return null;
            }

            JToken jsonAddress = jObject[0];

            var accessAddress = new Adgangsadresse();


            accessAddress.vejstykke = new Vejstykke(jsonAddress["vejstykke"]);
            accessAddress.postnummer = new Postnummer(jsonAddress["postnummer"]);
            accessAddress.husnr = (string)jsonAddress["husnr"];
            accessAddress.adgangspunkt = new Adgangspunkt(jsonAddress["adgangspunkt"]);

            return accessAddress;
        }

        #endregion

        public Address Launder(Address inputAddress)
        {
            return LaunderAddress(inputAddress);
        }

        public Address Launder(string inputAddress)
        {
            throw new System.NotImplementedException();
        }
    }
}
