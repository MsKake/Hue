using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HueDevicesOverView
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BibleStuff();

            string hueIp = ConfigurationManager.AppSettings["hueip"];
            string hueAPIKey = ConfigurationManager.AppSettings["hueapikey"];


            List<OverView> ovs = new List<OverView>();

            ovs.AddRange(GetJson("http://" + hueIp + "/api/" + hueAPIKey + "/lights"));
            ovs.AddRange(GetJson("http://" + hueIp + "/api/" + hueAPIKey + "/groups"));
            ovs.AddRange(GetJson("http://" + hueIp + "/api/" + hueAPIKey + "/sensors"));
           // ovs.AddRange(GetJson("http://192.168.1.133/api/UJOyatciz47HUb-oIxOc7K4oIkJBGae4ZRqhWdag/groups")); 
            //ovs.AddRange(GetJson("http://192.168.1.133/api/UJOyatciz47HUb-oIxOc7K4oIkJBGae4ZRqhWdag/sensors"));
            GridView1.DataSource = ovs;
            GridView1.DataBind();
        }

        private void BibleStuff()
        {
            //https://luther.k-r.ch/books
            string url= "https://luther.k-r.ch/books";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            List<string> data = new List<string>();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                //************** testing ******************
                //dynamic stuff = JsonConvert.DeserializeObject(result);

                //JObject all = JObject.Parse(result);
                //List<JToken> objects = new List<JToken>();
                //objects = all["data"].Children().ToList();
                //foreach (var x in all)
                //{
                //    string name = x.Key;
                //    JToken value = x.Value;
                //}

                JObject obj = JObject.Parse(result);
                var attributes = obj["data"];//list with jtokens

                foreach (var attributeProperty in attributes)
                {
                    var attribute = attributeProperty;
                    var test = attributeProperty.First();
                    var my_data = attribute["book"];
                    var bookValue = my_data.Value<string>();//dette er navnet på boka, gen etc
                    data.Add(bookValue);
                }
            }
        }

        private List<OverView> GetJson(string url)//http://192.168.1.133/api/UJOyatciz47HUb-oIxOc7K4oIkJBGae4ZRqhWdag/groups
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            List<OverView> ovs = new List<OverView>();
            string[] urlSplit = url.Split('/');

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic stuff = JsonConvert.DeserializeObject(result);

                JObject googleSearch = JObject.Parse(result);
                //IList<JToken> results = googleSearch["1"].Children().ToList();
                //IList<JToken> results2 = googleSearch.Children().ToList();

                //foreach(dynamic token in results2)
                //{
                //    //Response.Write(token.ToString() +"<br>");
                //    //dynamic temp = JsonConvert.DeserializeObject(token.ToString());
                //    //JObject obj = JObject.Parse(temp);
                //    //Response.Write(token.name);
                //}

                foreach (var x in googleSearch)
                {
                    OverView ov = new OverView();
                    string id = x.Key;
                    var value = x.Value["name"].ToString();
                    //var lastUpdated=
                    ov.HueId = id;
                    ov.Name = value;
                    ov.Type = urlSplit[urlSplit.Length - 1];
                    ov.JsonStr = x.Value.ToString();

                    ovs.Add(ov);
                }

            }
            return ovs;

        }
    }

    public class OverView
    {
        public string HueId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string JsonStr { get; set; }

    }
}
