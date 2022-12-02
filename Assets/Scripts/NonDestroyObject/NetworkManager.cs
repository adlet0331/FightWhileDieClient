using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace NonDestroyObject
{
    [Serializable]
    public enum ClientCondition
    {
        Idle,
        Localhost,
        NoConnectionTest
    }
    public class NetworkManager : Singleton<NetworkManager>
    {
        public int playerId;
        [SerializeField] private ClientCondition _condition;
        [SerializeField] private string _rootURL;
        public bool connectable;

        private void Start()
        {
            switch (_condition)
            {
                case ClientCondition.Idle:
                    _rootURL = "http://fwt-server.haje.org";
                    return;
                case ClientCondition.Localhost:
                    _rootURL = "http://localhost:8000";
                    return;
                case ClientCondition.NoConnectionTest:
                    _rootURL = "http://NoConnectionTest:1234";
                    return;
            }
        }

        private string RequestPost(string url, string jsonString)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_rootURL + url);
            request.Method = "POST";
            request.Timeout = 3 * 1000;
            request.ContentType = "text/json";

            Debug.Log(jsonString);
            var jsonByte = Encoding.UTF8.GetBytes(jsonString);
            
            Stream streamRequest = request.GetRequestStream();
            streamRequest.Write(jsonByte, 0, jsonByte.Length);
            streamRequest.Flush();
            streamRequest.Close();

            var response = (HttpWebResponse)request.GetResponse();
            HttpStatusCode statusCode = response.StatusCode;
            Debug.Log(statusCode);
            if (statusCode != HttpStatusCode.OK)
                return string.Empty;
                
            Stream respStream = response.GetResponseStream();
            using (StreamReader streamReader = new StreamReader(respStream))
            {
                return streamReader.ReadToEnd();
            }
        }
        
        public CheckConnectionResult CheckConnection()
        {
            var req = new CheckConnectionReq()
            {
                Id = SLManager.Instance.Id
            };
            var reqJson = JsonConvert.SerializeObject(req);
            string resultJson;
            
            try
            {
                resultJson = RequestPost("/playerserver/checkconnection/", reqJson);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e);
                return CheckConnectionResult.No_Connection_To_Server;
            }
            
            CheckConnectionRes res = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            connectable = res.success;
            if (connectable && res.NeedInit)
            {
                return CheckConnectionResult.Success_But_No_Id_In_Server;
            }

            return CheckConnectionResult.Success;
        }

        public CreateNewUserResult CreateNewUser(string userName)
        {
            var request = new CreateNewUserReq()
            {
                Id = SLManager.Instance.Id,
                TopStage = SLManager.Instance.TopStage,
                Name = userName,
                BaseAtk = SLManager.Instance.BaseAtk,
                Coin = SLManager.Instance.Coin
            };
            var reqJson = JsonConvert.SerializeObject(request);
            string resultJson;

            try
            {
                resultJson = RequestPost("/playerserver/createnewuser/", reqJson);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e);
                return CreateNewUserResult.No_Connection_To_Server;
            }

            var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
            if (result.success)
            {
                playerId = result.Id;
                return CreateNewUserResult.Success;
            }
            else
            {
                return CreateNewUserResult.Fail;
            }
        }

        public FetchUserResult FetchUser()
        {
            var request = new CreateNewUserReq()
            {
                Id = SLManager.Instance.Id,
                TopStage = SLManager.Instance.TopStage,
                Name = SLManager.Instance.Name,
                BaseAtk = SLManager.Instance.BaseAtk,
                Coin = SLManager.Instance.Coin
            };
            var reqJson = JsonConvert.SerializeObject(request);
            
            var resultJson = RequestPost("/playerserver/fetchuser/", reqJson);
            Debug.Log(resultJson);
            var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
            if (result.success)
            {
                playerId = result.Id;
                return FetchUserResult.Success;
            }
            else
            {
                return FetchUserResult.Fail;
            }
        }
    }
}
