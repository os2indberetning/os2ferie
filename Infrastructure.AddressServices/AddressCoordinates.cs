using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices;
using Newtonsoft.Json.Linq;
using Core.DomainModel;

namespace Infrastructure.AddressServices
{
    public class AddressCoordinates : IAddressCoordinates
    {
        #region Properties

        /// <summary>
        /// Specifies number of decimals used for coordinates.
        /// </summary>
        const int COORD_DECIMALS = 4;

        /// <summary>
        /// Specifies number of decimals used for coordinates.
        /// </summary>
        public static int CoordDecimals
        {
            get { return COORD_DECIMALS; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get address information for given coordinates.
        /// </summary>
        /// <param name="addressCoord"></param>
        /// <exception cref="AddressCoordinatesException">Thrown if the coordinates did not return an address.</exception>
        /// <returns></returns>
        public Address GetAddressFromCoordinates(Address addressCoord)
        {
            var request = CreateAddressRequest(addressCoord.Longitude, addressCoord.Latitude);

            var responseString = ExecuteAndRead(request);
            var coordAddress = ParseSingleJson(responseString);

            if (!coordAddress.Any())
            {
                throw new AddressCoordinatesException("No address found at the specified coordinates");
            }

            var singleAddress = coordAddress.First();

            addressCoord.StreetName = singleAddress.vejstykke.navn;
            addressCoord.StreetNumber = singleAddress.husnr;
            addressCoord.ZipCode = Int32.Parse(singleAddress.postnummer.nr);
            addressCoord.Town = singleAddress.postnummer.navn;

            return addressCoord;
        }

        /// <summary>
        /// Get coordinates for a given addresses. Use this method for single calls outside the service class.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="correctAddresses">Set this to use address laundering prior to each request, the corrected values will be replaced. Default: false</param>
        /// <exception cref="AddressCoordinatesException">Thrown if address has critical spelling errors(see inner exception) or if no address coordinates correspond to the entered address.</exception>
        /// <returns></returns>
        public Address GetAddressCoordinates(Address address, bool correctAddresses = false)
        {
            Address correctedAddress = address;
            try
            {
                if (address.Latitude != null && address.Longitude != null && address.Latitude != "" && address.Longitude != "")
                {
                    return correctedAddress;
                }
                AddressLaundering launderer = new AddressLaundering();
                correctedAddress = launderer.LaunderAddress(address);
                return correctedAddress;
            }
            catch (AddressLaunderingException e)
            {
                throw new AddressCoordinatesException("En valgt adresse kunne ikke vaskes.", e);
            }
            //var request = CreateCoordRequest(correctedAddress.StreetName, correctedAddress.StreetNumber, correctedAddress.ZipCode.ToString());

            //string addressesString = ExecuteAndRead(request);
            //var addresses = ParseJson(addressesString);

            //if (!addresses.Any())
            //{
            //    request = CreateCoordRequest(address.StreetName, null, address.ZipCode.ToString());

            //    addressesString = ExecuteAndRead(request);
            //    addresses = ParseJson(addressesString);
            //}

            //if (!addresses.Any())
            //{
            //    throw new AddressCoordinatesException("No coordinates returned.");
            //}

            //if (addresses[0].adgangsadresse.vejstykke.navn == address.StreetName
            //    && addresses[0].adgangsadresse.postnummer.nr == address.ZipCode.ToString())
            //{
            //    correctedAddress.Longitude = addresses[0].adgangsadresse.adgangspunkt.koordinater[0].ToString().Replace(",", ".");
            //    correctedAddress.Latitude = addresses[0].adgangsadresse.adgangspunkt.koordinater[1].ToString().Replace(",", ".");

            //    correctedAddress.Longitude = correctedAddress.Longitude.Remove(correctedAddress.Longitude.IndexOf('.') + 1 + CoordDecimals);
            //    correctedAddress.Latitude = correctedAddress.Latitude.Remove(correctedAddress.Latitude.IndexOf('.') + 1 + CoordDecimals);
            //}
            //else
            //{
            //    throw new AddressCoordinatesException("The addresses returned differ highly from the original, streetname does not exist in zipcode area.");
            //}

            //return correctedAddress;
        }

        /// <summary>
        /// Get coordinates for a given addresses. Only used by the service class, do not call elsewhere.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        /// <param name="correctAddresses">Set this to use address laundering prior to each request. Default: false</param>
        /// <exception cref="AddressCoordinatesException">Thrown if address has critical spelling erros(see inner exception) or if no address coordinates correspond to the entered address.</exception>
        /// <returns></returns>
        public Coordinates GetCoordinates(Address address, Coordinates.CoordinatesType type, bool correctAddresses = false)
        {
            Address correctedAddress = address;
            if (!correctAddresses)
            {
                try
                {
                    AddressLaundering launderer = new AddressLaundering();
                    correctedAddress = launderer.LaunderAddress(address);
                }
                catch (AddressLaunderingException e)
                {
                    throw new AddressCoordinatesException("Errors in address, see inner exception.", e);
                }
            }

            var request = CreateCoordRequest(correctedAddress.StreetName, correctedAddress.StreetNumber, correctedAddress.ZipCode.ToString());

            string addressesString = ExecuteAndRead(request);
            var addresses = ParseJson(addressesString);

            if (!addresses.Any())
            {
                request = CreateCoordRequest(correctedAddress.StreetName, null, correctedAddress.ZipCode.ToString());

                addressesString = ExecuteAndRead(request);
                addresses = ParseJson(addressesString);
            }

            if (!addresses.Any())
            {
                throw new AddressCoordinatesException("No coordinates returned.");
            }

            Coordinates addressCoordinates = new Coordinates { Type = type };

            if (addresses[0].adgangsadresse.vejstykke.navn == correctedAddress.StreetName
                && addresses[0].adgangsadresse.postnummer.nr == correctedAddress.ZipCode.ToString())
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

        #endregion

        #region Private methods

        /// <summary>
        /// Create a request for getting address coordinates following the service API url specifications. (dawa.aws.dk)
        /// </summary>
        /// <param name="street">Uppercase street name</param>
        /// <param name="streetNr">Uppercase street number</param>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        private HttpWebRequest CreateCoordRequest(string street, string streetNr, string zipCode)
        {
            var query = streetNr == null ? string.Format("vejnavn={0}&postnr={1}", street, zipCode) : string.Format("vejnavn={0}&husnr={1}&postnr={2}", street, streetNr, zipCode);

            return (HttpWebRequest)WebRequest.Create(UrlDefinitions.CoordinatesUrl + query);
        }

        /// <summary>
        /// Create a request for getting coordinates address following the service API url specifications. (dawa.aws.dk)
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        private HttpWebRequest CreateAddressRequest(string longitude, string latitude)
        {
            var query = string.Format("x={0}&y={1}", longitude, latitude);

            return (HttpWebRequest)WebRequest.Create(UrlDefinitions.CoordinateToAddressUrl + query);
        }

        /// <summary>
        /// Execute HTTP request and read the response.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Formatted response from service.</returns>
        private string ExecuteAndRead(HttpWebRequest request, bool returnSingle = false)
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
                throw new AddressCoordinatesException("Server error, coordinates invalid", e);
            }

            if (responseStream == null) return null;

            var streamReader = new StreamReader(responseStream);
            responseString = streamReader.ReadToEnd();
            streamReader.Close();
            return responseString;
        }

        /// <summary>
        /// Formats and structures the response. 
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Response reprensented in custom class.</returns>
        private List<RootAddressObject> ParseJson(string response)
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
                    status = (int)address["status"],
                    adressebetegnelse = (string)address["adressebetegnelse"],
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

        private List<RootCoordinateToAddressObject> ParseSingleJson(string response)
        {
            List<RootCoordinateToAddressObject> addressObject = new List<RootCoordinateToAddressObject>();

            JToken jAddress = JToken.Parse(response);

            if (jAddress == null)
            {
                return addressObject;
            }

            addressObject.Add(new RootCoordinateToAddressObject(jAddress));

            return addressObject;
        }

        #endregion
    }
}
