using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace HueDevicesOverView
{
    public class Hue
    {
        public bool[] GetGroupStateByGroupId(int groupId)//all_on any_on true/false
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.1.133/api/apikey/groups/" + groupId);//all lights in a group
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            bool[] state = new bool[2];
            state[0] = false;
            state[1] = false;
            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    //string json = "{\"user\":\"test\"," +
            //    //              "\"password\":\"bla\"}";
            //    string json = "{\"on\":true}";

            //    streamWriter.Write(json);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //05032019 virker
                var result = streamReader.ReadToEnd();
                JObject googleSearch = JObject.Parse(result);

                IList<JToken> results = googleSearch["state"].Children().ToList();
                //var definition = new { all_on = "" };
                var allOn = results[0].First();
                var anyOn = results[1].First();
                bool isAllOn = bool.Parse(allOn.ToString());
                bool isAnyOn = bool.Parse(anyOn.ToString()); //ttttewst
                state[0] = isAllOn;
                state[1] = isAnyOn;
            }
            return state;
        }

        /// <summary>
        /// Turns on all lights in a group
        /// </summary>
        /// <param name="groupId"></param>
        public void TurnOnLightsInGroup(int groupId)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.1.133/api/apikey/groups/" + groupId + "/action");//all lights in a group
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"on\":true}";
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        //Todo method for turning lights off in group. 
    }
}