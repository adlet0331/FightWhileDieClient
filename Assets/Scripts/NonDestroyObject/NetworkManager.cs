using System;
using System.IO;
using System.Net;
using System.Text;
using Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NonDestroyObject
{
    public class BaseRequest
    {
        public int id;
    }

    public class BaseResponse
    {
        public bool success;
    }

    public class CheckConnectionRes : BaseResponse
    {
        
    }
    
    public class NetworkManager : Singleton<NetworkManager>
    {
        public int playerId;
        public string _rootURL = "http://localhost:8000";
        public bool connectable;

        private void Start()
        {
            
        }

        private string RequestPost(string url, string jsonString)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_rootURL + url);
            request.Method = "POST";
            request.Timeout = 3 * 1000;
            request.ContentType = "text/json";

            var jsonByte = Encoding.UTF8.GetBytes(jsonString);
            
            Stream streamRequest = request.GetRequestStream();
            streamRequest.Write(jsonByte, 0, jsonByte.Length);
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode statusCode = response.StatusCode;
                Debug.Log(statusCode);
                if (statusCode != HttpStatusCode.OK)
                    return string.Empty;
                
                Debug.Log(statusCode);
                Stream respStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(respStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        
        private void CheckConnectionReq()
        {
            var json = new BaseRequest()
            {
                id = SLManager.Instance.Id
            };
            var jsonString = json.ToString();
            Debug.Log(jsonString);

            var resultJson = RequestPost("/playerserver/checkconnection", jsonString);
            Debug.Log(resultJson);
            CheckConnectionRes res = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            connectable = res is { success: true } ? true : false;
        }

        private void CreateNewUserReq()
        {
            var json = new BaseRequest()
            {
                id = SLManager.Instance.Id
            };
            var jsonString = json.ToString();
            Debug.Log(jsonString);
        }
        
    }
}
