using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

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
                    _rootURL = "http://fwt-server.haje.org/playerserver/";
                    return;
                case ClientCondition.Localhost:
                    _rootURL = "http://localhost:8000/playerserver/";
                    return;
                case ClientCondition.NoConnectionTest:
                    _rootURL = "http://NoConnectionTest:1234";
                    return;
            }
        }

        private async UniTask<string> RequestPost(string url, string reqString, int retryNum = 3)
        {
            Debug.Log(url + "\n" + reqString);

            var httpContent = new StringContent(reqString, Encoding.UTF8, "text/json");
            HttpResponseMessage response;
            var cnt = 0;

            while(true)
            {
                try
                {
                    response = await _httpClient.PostAsync(_rootURL + url, httpContent);
                }
                catch (WebException e)
                {
                    Debug.Log($"{cnt}" + e);
                    return string.Empty;
                }
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    break;
                }
                // Retry $retryNum time
                cnt += 1;
                if (cnt > retryNum)
                    return string.Empty;
            }

            var resString = await response.Content.ReadAsStringAsync();
            Debug.Log(resString);
            return resString;
        }
        
        public async UniTask<CheckConnectionResult> CheckConnection()
        {
            var req = new CheckConnectionReq()
            {
                id = DataManager.Instance.PlayerDataManager.Id
            };
            var reqJson = JsonConvert.SerializeObject(req);
            
            string resultJson = string.Empty;
            resultJson = await RequestPost("checkconnection/", reqJson);
            
            if (resultJson == string.Empty)
            {
                return CheckConnectionResult.NoConnectionToServer;
            }
            
            CheckConnectionRes result = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            if (result is { success: true, needInit: true })
            {
                return CheckConnectionResult.SuccessButNoIdInServer;
            }

            return CheckConnectionResult.Success;
        }

        public async UniTask<CreateNewUserResult> CreateNewUser(string userName)
        {
            var request = new CreateNewUserReq()
            {
                id = DataManager.Instance.PlayerDataManager.Id,
                topStage = DataManager.Instance.PlayerDataManager.TopStage,
                name = userName,
                baseAtk = DataManager.Instance.PlayerDataManager.BaseAtk,
                coin = DataManager.Instance.PlayerDataManager.Coin,
                enhanceIngredientList = DataManager.Instance.PlayerDataManager.EnhanceIngredientList,
            };
            var reqJson = JsonConvert.SerializeObject(request);
            string resultJson = string.Empty;
            resultJson = await RequestPost("createnewuser/", reqJson);

            if (resultJson == string.Empty)
            {
                return CreateNewUserResult.NoConnectionToServer;
            }

            var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
            if (result is { success: false })
                return CreateNewUserResult.Fail;
            
            await UniTask.SwitchToMainThread();
            DataManager.Instance.PlayerDataManager.InitUser(result.id, userName);
            return CreateNewUserResult.Success;
        }

        public async UniTask<DeleteUserResult> DeleteUser(int id)
        {
            var request = new DeleteUserReq()
            {
                id = id,
            };
            var reqJson = JsonConvert.SerializeObject(request);
            string resultJson = string.Empty;
            resultJson = await RequestPost("deleteuser/", reqJson);

            if (resultJson == string.Empty) 
                return DeleteUserResult.Fail;

            var result = JsonConvert.DeserializeObject<DeleteUserRes>(resultJson);
            if (result is { success: false })
                return DeleteUserResult.NoUserInServer;

            return DeleteUserResult.Success;
        }

        public async UniTask<FetchUserResult> FetchUser()
        {
            var request = new FetchUserReq()
            {
                id = DataManager.Instance.PlayerDataManager.Id,
                topStage = DataManager.Instance.PlayerDataManager.TopStage,
                name = DataManager.Instance.PlayerDataManager.UserName,
                baseAtk = DataManager.Instance.PlayerDataManager.BaseAtk,
                coin = DataManager.Instance.PlayerDataManager.Coin,
                enhanceIngredientList = DataManager.Instance.PlayerDataManager.EnhanceIngredientList,
            };
            var reqJson = JsonConvert.SerializeObject(request);
            
            var resultJson = await RequestPost("fetchuser/", reqJson);

            if (resultJson == String.Empty)
            {
                return FetchUserResult.NoConnectionToServer;
            }
            
            var result = JsonConvert.DeserializeObject<FetchUserRes>(resultJson);
            if (result is { success: true })
            {
                return FetchUserResult.Success;
            }
            else
            {
                return FetchUserResult.Fail;
            }
        }
    }
}
