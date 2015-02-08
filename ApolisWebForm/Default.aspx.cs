using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SODA;
using Newtonsoft.Json;
using System.Web.Services;
using System.Web.Script.Services;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Net;
using System.Collections.Specialized;


namespace ApolisWebForm
{
      
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class BlockGroups
        {

            public List<BlockGroup> data { get; set; }
        }

        public class BlockGroup
        {

            public string id { get; set; }
            public string name { get; set; }
        }

        public static void RetrievAffordability()
        {

            string baseUri = "http://services.arcgis.com/VTyQ9soqVukalItT/arcgis/rest/services/LocationAffordabilityIndexData/FeatureServer/0/";

            string requestUri = string.Format(baseUri);
			// Create a new WebClient instance.
			WebClient wc = new WebClient();
 
            NameValueCollection myQueryStringCollection = new NameValueCollection();
            myQueryStringCollection.Add("outFields", "*");
            myQueryStringCollection.Add("where", "1=1");
            myQueryStringCollection.Add("geometry", "-123.219%2C47.457%2C-121.421%2C47.735");
            myQueryStringCollection.Add("f", "json");
  
            wc.QueryString = myQueryStringCollection;

           using (wc)
            {
                string result = wc.DownloadString(requestUri);

                var json = new JavaScriptSerializer().Serialize(result);

                        foreach (var itemvalue in json)
                        {
                            int dummy = 1;
                        }
             }
        }

        public static void RetrieveFormatedAddress(string lat, string lng, Place currentPlace)
        {
           //string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
           string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false&type=neighborhood";
           string location = string.Empty;
          // string thisNeighborhood = "";

            string requestUri = string.Format(baseUri, lat, lng);

            using (WebClient wc = new WebClient())
            {
                string result = wc.DownloadString(requestUri);
                var xmlElm = XElement.Parse(result);
                var status = (from elm in xmlElm.Descendants()
                              where
                                  elm.Name == "status"
                              select elm).FirstOrDefault();
                if (status.Value.ToLower() == "ok")
                {
                    currentPlace.Neighborhood = "";
                    currentPlace.NestedNeighborhood = "";
                    currentPlace.City = "";

                    var addresscomponents = (from elm2 in xmlElm.Descendants("address_component")
                                             select elm2);

                    foreach (var item in addresscomponents)
                    {
                        var longname = (from elm4 in item.Descendants("long_name")
                                        select elm4).FirstOrDefault().Value;

                        var elements = (from elm3 in item.Descendants("type")
                                        select elm3);

                        string test = "";

                        foreach (var itemvalue in elements)
                        {
                            test = test + itemvalue.Value;
                            
                        }
                        if (test.Contains("neighborhood"))
                        {
                            if (currentPlace.Neighborhood != "")
                            {
                                string thisN = longname.ToString();
                                if (!currentPlace.NestedNeighborhood.Contains(thisN))
                                {
                                    currentPlace.NestedNeighborhood = currentPlace.Neighborhood + ":" + thisN;
                                    currentPlace.Neighborhood = thisN.ToString();
                                }
                            } else {
                                currentPlace.Neighborhood = longname.ToString();
                            }
                        }

                        if (test.Contains("locality"))
                        {
                            currentPlace.City = longname.ToString();
                        }

                        int dummy = 1;

                    }

                    /*var res = (from elm in xmlElm.Descendants()
                               where elm.Name == "long_name"
                               select elm).FirstOrDefault();
                    requestUri = res.Value;
                     */
                }
            }

        }

        public class Place
        {
            public string ID { get; set; }
            public string Type { get; set; }
            public string DisplayName { get; set; }
          //  public string Address { get; set; }
         //   public string Website { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public string LongitudeStr { get; set; }
            public string LatitudeStr { get; set; }
            public string Neighborhood { get; set; }
            public string NestedNeighborhood { get; set; }
            public string City {get; set; }
            public string hood { get; set; }
            public double distnorm { get; set; }
        }

        public class Neighborhood
        {
            public string Name { get; set; }
            //   public string Type { get; set; }
            public string DisplayName { get; set; }
            //  public string Address { get; set; }
            //   public string Website { get; set; }
            public double LongitudeSum { get; set; }
            public double LatitudeSum { get; set; }
            public double DistanceSum { get; set; }
            public double DistanceAve { get; set; }
            public double LongitudeN { get; set; }
            public double LatitudeN { get; set; }
            public double LongitudeCentroid { get; set; }
            public double LatitudeCentroid { get; set; }
            public string City { get; set; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)] 
        public static List<Place> getPlaces()
        {

       //     RetrievAffordability();

            var client = new SodaClient("data.seattle.gov", "eDhP3BH0cLYPgbEC56eNWnraC");
            var dataset = client.GetResource<Dictionary<string, object>>("82su-5fxf");
            var theseRows = dataset.GetRows(2500);

         //   var placelistall = new List<Place>();
            var placelist = new List<Place>();
            Dictionary<string, Neighborhood> neighCentroids = new Dictionary<string, Neighborhood>();

           string id = "";
           int idint = 3;

            /*
           Place testplace = new Place();
           testplace.ID = "1";
           testplace.LatitudeStr = "127.23432";
           testplace.LongitudeStr = "-199.232343";
           testplace.Latitude = (47.62011337);
           testplace.Longitude = (-122.316169);

           Place testplace2 = new Place();
           testplace2.ID = "2";
           testplace2.LatitudeStr = "127.23432";
           testplace2.LongitudeStr = "-199.232343";
           testplace2.Latitude = (47.62011337);
           testplace2.Longitude = (-122.316169);

           placelist.Add(testplace);
           placelist.Add(testplace2);
            */

           foreach (var item in theseRows)
           {
               Place thisPlace = new Place();
               thisPlace.distnorm = 1;
               thisPlace.ID = idint.ToString();
               bool skip1 = false;
               thisPlace.Type = "regular";

               if (item.ContainsKey("city_feature"))
               {
                   thisPlace.Type = (string)item["city_feature"];
               }

               if (thisPlace.Type == "Boat Launches" || thisPlace.Type == "Cemeteries" || thisPlace.Type == "Drivers Licenses" || thisPlace.Type == "Emission Inspections" || thisPlace.Type == "Fire Stations" || thisPlace.Type == "Fishing" || thisPlace.Type == "Hospitals" || thisPlace.Type == "Light Rail" || thisPlace.Type == "Monorail" || thisPlace.Type == "ParkNRide" || thisPlace.Type == "Pet License Sales" || thisPlace.Type == "Police Precincts" || thisPlace.Type == "South Lake Union Trolley" || thisPlace.Type == "Traffic Cameras" || thisPlace.Type == "Transfer Stations")
               {
                   skip1 = true;
               }

               if (skip1 == false)
               {

                   thisPlace.DisplayName = "none";

                   if (item.ContainsKey("common_name"))
                   {
                       thisPlace.DisplayName = (string)item["common_name"];
                   }
                   /*

                   if (item.ContainsKey("address"))
                   {
                       thisPlace.Address = (string)item["address"];                   
                   }
                   if (item.ContainsKey("website"))
                   {
                       //thisPlace.Website = (string)item["website"];

                   }
                   */
                   thisPlace.LongitudeStr = (string)item["longitude"];
                   thisPlace.LatitudeStr = (string)item["latitude"];
                   thisPlace.Longitude = Convert.ToDouble(thisPlace.LongitudeStr);
                   thisPlace.Latitude = Convert.ToDouble(thisPlace.LatitudeStr);

                   //  thisPlace.Neighborhood = (string)item["common_name"];
                   bool skip = false;

                   if (idint > 2000)
                   {
                       skip = true;
                   };
                   if (String.IsNullOrEmpty(thisPlace.LatitudeStr) || (String.IsNullOrEmpty(thisPlace.LongitudeStr)) || (thisPlace.Longitude == 0) || (thisPlace.Latitude == 0))
                   {
                       skip = true;
                   }
                   if (skip == false)
                   {
                       Console.WriteLine(thisPlace.Longitude + " " + thisPlace.Latitude);
                       RetrieveFormatedAddress(thisPlace.LatitudeStr, thisPlace.LongitudeStr, thisPlace);
                       placelist.Add(thisPlace);
                   }
                   idint = idint + 1;
               }//end skip

           }//end for each row hear

          
               foreach (Place currentPlace in placelist)
               {
                   string thishood = "";
                   if (currentPlace.NestedNeighborhood != "")
                   {
                       thishood = currentPlace.NestedNeighborhood;
                       if (thishood == "" && currentPlace.Neighborhood != "")
                       {
                           thishood = currentPlace.Neighborhood;
                           if (thishood == "" && currentPlace.City != "")
                           {
                               thishood = currentPlace.City;
                           }
                       }
                   }
                   if (thishood != "" && thishood != null)
                   {
                       if (!neighCentroids.Keys.Contains(thishood))
                       {
                           Neighborhood thisNeighborhood = new Neighborhood();
                           thisNeighborhood.Name = thishood;
                           thisNeighborhood.LatitudeSum = currentPlace.Latitude;
                           thisNeighborhood.LongitudeSum = currentPlace.Longitude;
                           thisNeighborhood.LatitudeN = 1;
                           thisNeighborhood.LongitudeN = 1;
                           thisNeighborhood.City = currentPlace.City;
                           currentPlace.hood = thishood;
                           neighCentroids.Add(thishood, thisNeighborhood);
                       }
                       else
                       {
                           neighCentroids[thishood].LatitudeSum = neighCentroids[thishood].LatitudeSum + currentPlace.Latitude;
                           neighCentroids[thishood].LongitudeSum = neighCentroids[thishood].LongitudeSum + currentPlace.Longitude;
                           neighCentroids[thishood].LatitudeN = neighCentroids[thishood].LatitudeN + 1;
                           neighCentroids[thishood].LongitudeN = neighCentroids[thishood].LongitudeN + 1;
                       }
                   }
               }

//            List<Place> nPlaces = new List<Place>();
            double maxdist = 0;

            foreach (Neighborhood thisNeigh in neighCentroids.Values)
            {
                thisNeigh.LatitudeCentroid = thisNeigh.LatitudeSum / thisNeigh.LatitudeN;
                thisNeigh.LongitudeCentroid = thisNeigh.LongitudeSum / thisNeigh.LongitudeN;
                Place nPlace = new Place();
                nPlace.distnorm = 1;
                nPlace.Latitude = thisNeigh.LatitudeCentroid;
                nPlace.Longitude = thisNeigh.LongitudeCentroid;
                nPlace.LatitudeStr = thisNeigh.LatitudeCentroid.ToString();
                nPlace.LongitudeStr = thisNeigh.LongitudeCentroid.ToString();
                nPlace.Type = "centroid";
                nPlace.DisplayName = thisNeigh.Name;
                Debug.WriteLine("adding neighborhood: " + thisNeigh.Name);

                thisNeigh.DistanceSum = 0;
                double count = 0;

                //getting hubbiness measure here
               foreach (Place yPlace in placelist)
                {
                    if (thisNeigh.Name == yPlace.hood)
                    {
                        //  c2 = a2 + b2
                        double distance = Math.Sqrt(Math.Pow((thisNeigh.LatitudeCentroid - yPlace.Latitude), 2) + (Math.Pow((thisNeigh.LongitudeCentroid - yPlace.Longitude), 2)));
                        thisNeigh.DistanceSum = thisNeigh.DistanceSum + distance;
                        count = count + 1;
                    }
                }
                if (count > 0)
                {
                    thisNeigh.DistanceAve = thisNeigh.DistanceSum / count;
                    if (thisNeigh.DistanceAve > maxdist) maxdist = thisNeigh.DistanceAve;
                }
                //inverted to the higher the number, them more dense the hub;

              nPlace.distnorm = 1 - (thisNeigh.DistanceAve / maxdist);
                /*meant  was div by 0; */
                
              if (nPlace.distnorm == null) nPlace.distnorm = 1;

              placelist.Add(nPlace);
            }
      
       //     foreach (Place item in Places.Values)
         //   {
           //     placelist.Add(item);
            //}
            
   //         string myJsonString = JsonConvert.SerializeObject(placelist);

        //    object thisobject = "{id: happy, lat: 30}";
         //   return thisobject;
            return placelist;

        }
 
    }
}