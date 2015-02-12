using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Infrastructure.AddressServices.Classes
{
    #region ADDRESS LAUNDERING

    public class LaunderedAddress
    {
        public string streetbuildingidentifier { get; set; }
        public string postcode { get; set; }
        public string streetname { get; set; }
    }

    public class RootLaunderedObject
    {
        public string input { get; set; }
        public string validatedescription { get; set; }
        public int validateresult { get; set; }
        public LaunderedAddress laundered_address { get; set; }
    }

    #endregion

    #region ADDRESS TO COORDINATES

    public class Adgangsadresse
    {
        public int status { get; set; }
        public Vejstykke vejstykke { get; set; }
        public string husnr { get; set; }
        public object supplerendebynavn { get; set; }
        public Postnummer postnummer { get; set; }
        public Kommune kommune { get; set; }
        public Adgangspunkt adgangspunkt { get; set; }
    }

    public class Adgangspunkt
    {
        public Adgangspunkt(JToken token)
        {
            koordinater = new List<double>();

            foreach (double point in token["koordinater"])
            {
                koordinater.Add(point);
            }
        }
        public List<double> koordinater { get; set; }
        public string __invalid_name__nøjagtighed { get; set; }
        public int kilde { get; set; }
        public string tekniskstandard { get; set; }
        public int tekstretning { get; set; }
        public string __invalid_name__ændret { get; set; }
    }

    public class Vejstykke
    {
        public Vejstykke(JToken token)
        {
            href = (string)token["href"];
            navn = (string)token["navn"];
            kode = (string)token["kode"];
        }
        public string href { get; set; }
        public string navn { get; set; }
        public string kode { get; set; }
    }

    public class Postnummer
    {
        public Postnummer(JToken token)
        {
            href = (string)token["href"];
            nr = (string)token["nr"];
            navn = (string)token["navn"];
        }
        public string href { get; set; }
        public string nr { get; set; }
        public string navn { get; set; }
    }

    public class Kommune
    {
        public string href { get; set; }
        public string kode { get; set; }
        public string navn { get; set; }
    }

    public class RootAddressObject
    {
        public string id { get; set; }
        public string kvhx { get; set; }
        public int status { get; set; }
        public string href { get; set; }
        public string etage { get; set; }
        public object __invalid_name__dør { get; set; }
        public string adressebetegnelse { get; set; }
        public Adgangsadresse adgangsadresse { get; set; }
    }
    #endregion


    #region ROUTE SEPTIMA

    public class RouteSummary
    {
        public RouteSummary(JToken token)
        {
            total_distance = (int)token["total_distance"];
            total_time = (int)token["total_time"];
            start_point = (string)token["start_point"];
            end_point = (string)token["end_point"];
        }

        public int total_distance { get; set; }
        public int total_time { get; set; }
        public string start_point { get; set; }
        public string end_point { get; set; }
    }

    public class AlternativeSummary
    {
        public AlternativeSummary(JToken token)
        {
            total_distance = (int)token["total_distance"];
            total_time = (int)token["total_time"];
            start_point = (string)token["start_point"];
            end_point = (string)token["end_point"];
        }
        public int total_distance { get; set; }
        public int total_time { get; set; }
        public string start_point { get; set; }
        public string end_point { get; set; }
    }

    public class RootRouteObject
    {
        public RootRouteObject(JToken token)
        {
            version = (double)token["version"];
            status = (int)token["status"];
            status_message = (string)token["status_message"];
            route_geometry = (string)token["route_geometry"];

            alternative_geometries = new List<string>();
            alternative_summaries = new List<AlternativeSummary>();
        }

        public double version { get; set; }
        public int status { get; set; }
        public string status_message { get; set; }
        public string route_geometry { get; set; }
        public RouteSummary route_summary { get; set; }
        public List<string> alternative_geometries { get; set; }
        public List<AlternativeSummary> alternative_summaries { get; set; }
    }

    #endregion
}
