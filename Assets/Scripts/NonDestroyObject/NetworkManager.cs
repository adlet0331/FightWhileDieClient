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
        public int Id;
    }

    public class BaseResponse
    {
        public bool success;
    }

    public class CheckConnectionRes : BaseResponse
    {
        
    }

    public class CreateNewUserReq : BaseRequest
    {
        public string Name;
        public int TopStage;
        public int BaseAtk;
        public int Coin;
    }
    
    public class NetworkManager : Singleton<NetworkManager>
    {
        public int playerId;
        public string _rootURL = "http://localhost:8000";
        public bool connectable;

        private void Start()
        {
            CheckConnection();
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
        
        private void CheckConnection()
        {
            var json = new BaseRequest()
            {
                Id = SLManager.Instance.Id
            };
            var jsonString = json.ToString();
            Debug.Log(jsonString);

            var resultJson = RequestPost("/playerserver/checkconnection", jsonString);
            Debug.Log(resultJson);
            CheckConnectionRes res = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            connectable = res is { success: true } ? true : false;
        }

        public bool CreateNewUser(string userName)
        {
            if (!connectable) return false;
            
            var json = new CreateNewUserReq()
            {
                Id = SLManager.Instance.Id,
                TopStage = SLManager.Instance.TopStage,
                Name = userName,
                BaseAtk = SLManager.Instance.BaseAtk,
                Coin = SLManager.Instance.Coin
            };
            var jsonString = json.ToString();

            var resultJson = RequestPost("/playerserver/createnewuser", jsonString);
            if (resultJson == string.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
