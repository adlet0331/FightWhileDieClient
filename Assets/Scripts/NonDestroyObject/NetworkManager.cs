using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        private HttpClient _httpClient;
        public bool connectable;

        private void Start()
        {
            _httpClient = new HttpClient();
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

        private async UniTask<string> RequestPost(string url, string reqString)
        {
            Debug.Log(url + "\n" + reqString);

            var httpContent = new StringContent(reqString, Encoding.UTF8, "text/json");
            HttpResponseMessage response;
            
            try
            {
                response = await _httpClient.PostAsync(_rootURL + url, httpContent);
            }
            catch (WebException e)
            {
                Debug.Log("Error in Request Post");
                Debug.Log(e);
                return string.Empty;
            }
            HttpStatusCode statusCode = response.StatusCode;
            if (statusCode != HttpStatusCode.OK)
                return string.Empty;
            
            var resString = await response.Content.ReadAsStringAsync();
            Debug.Log(resString);
            return resString;
        }
        
        public async UniTask<CheckConnectionResult> CheckConnection()
        {
            var req = new CheckConnectionReq()
            {
                Id = SLManager.Instance.Id
            };
            var reqJson = JsonConvert.SerializeObject(req);
            
            string resultJson;
            resultJson = await RequestPost("/playerserver/checkconnection/", reqJson);
            
            if (resultJson == string.Empty)
            {
                return CheckConnectionResult.No_Connection_To_Server;
            }
            
            CheckConnectionRes result = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            if (result is { success: true, NeedInit: true })
            {
                return CheckConnectionResult.Success_But_No_Id_In_Server;
            }

            return CheckConnectionResult.Success;
        }

        public async UniTask<CreateNewUserResult> CreateNewUser(string userName)
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

            resultJson = await RequestPost("/playerserver/createnewuser/", reqJson);

            if (resultJson == string.Empty)
            {
                return CreateNewUserResult.No_Connection_To_Server;
            }

            var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
            if (result is { success: true })
            {
                playerId = result.Id;
                return CreateNewUserResult.Success;
            }
            else
            {
                return CreateNewUserResult.Fail;
            }
        }

        public async UniTask<FetchUserResult> FetchUser()
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
            
            var resultJson = await RequestPost("/playerserver/fetchuser/", reqJson);

            if (resultJson == String.Empty)
            {
                return FetchUserResult.No_Connection_To_Server;
            }
            
            var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
            if (result is { success: true })
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
