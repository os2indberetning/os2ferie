using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Core.DomainServices.RoutingClasses
{

    #region ADDRESS TO COORDINATES //Also used by address laundering after switch away from septima

    public class Adgangsadresse
    {
        public Vejstykke vejstykke { get; set; }
        public string husnr { get; set; }
        public Postnummer postnummer { get; set; }
        public Adgangspunkt adgangspunkt { get; set; }
    }

    public class Adgangspunkt
    {
        public Adgangspunkt(JToken token)
        {
            koordinater = new List<string>();

            foreach (string point in token["koordinater"])
            {
                koordinater.Add(point);
            }
        }
        public List<string> koordinater { get; set; }
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

    #region COORDINATES TO ADDRESS

    public class RootCoordinateToAddressObject
    {
        public RootCoordinateToAddressObject(JToken token)
        {
            href = (string)token["href"];
            id = (string)token["id"];
            kvh = (string)token["kvh"];
            status = (int)token["status"];
            vejstykke = new Vejstykke(token["vejstykke"]);
            husnr = (string)token["husnr"];
            postnummer = new Postnummer(token["postnummer"]);
        }

        public string href { get; set; }
        public string id { get; set; }
        public string kvh { get; set; }
        public int status { get; set; }
        public Vejstykke vejstykke { get; set; }
        public string husnr { get; set; }
        public Postnummer postnummer { get; set; }
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
        public int distance_not_including_ferry { get; set; }
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
            
            status = (int)token["status"];
            status_message = (string)token["status_message"];
            route_geometry = (string)token["route_geometry"];
            alternative_geometries = new List<string>();
            alternative_summaries = new List<AlternativeSummary>();
        }

        public int status { get; set; }
        public string status_message { get; set; }
        public string route_geometry { get; set; }
        public RouteSummary route_summary { get; set; }
        public List<string> alternative_geometries { get; set; }
        public List<AlternativeSummary> alternative_summaries { get; set; }
    }

    #endregion
}
