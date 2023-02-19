using System;
using System.Collections.Generic;
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
        [SerializeField]private bool connectable;
        private HttpClient _httpClient;
        public bool Connectable => connectable;
        
        private void Start()
        {
            _httpClient = new HttpClient();
            switch (_condition)
            {
                case ClientCondition.Idle:
                    _rootURL = "http://fwt-server.haje.org/playerserver/";
                    break;
                case ClientCondition.Localhost:
                    _rootURL = "http://localhost:8000/playerserver/";
                    break;
                case ClientCondition.NoConnectionTest:
                    _rootURL = "http://for-no-connection.haje.org/";
                    break;
            }
#if !UNITY_EDITOR
            _rootURL = "http://fwt-server.haje.org/playerserver/";  
  #endif
            CheckConnection().Forget();
        }

        private async UniTask<string> RequestPost(string url, string reqString, bool triggeredInThreadPool = true, int retryNum = 0)
        {
            if (!triggeredInThreadPool)
            {
                await UniTask.SwitchToMainThread();
                UIManager.Instance.loadingPopup.Open();
                Debug.Log(url + "\n" + reqString);
            }
            await UniTask.SwitchToThreadPool();

            var httpContent = new StringContent(reqString, Encoding.UTF8, "text/json");
            HttpResponseMessage response = new HttpResponseMessage();
            var success = false;
            var cnt = 0;
            var resString = string.Empty;

            while(cnt <= retryNum)
            {
                try
                {
                    response = await _httpClient.PostAsync(_rootURL + url, httpContent);
                }
                catch (Exception e)
                {
                    Debug.Log($"{url}, {cnt}\n" + e);
                    cnt += 1;
                    continue;
                }
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    success = true;
                    break;
                }
                // Retry $retryNum time
                cnt += 1;
            }
            if (success)
                resString = await response.Content.ReadAsStringAsync();

            await UniTask.SwitchToMainThread();
            if (!triggeredInThreadPool)
            {
                UIManager.Instance.loadingPopup.Close();
                Debug.Log(resString);
            }
            
            return resString;
        }
        
        /// <summary>
        /// Only Called in NetworkManager Privately.
        /// Check Before functions, return if connection is not avaliable
        /// </summary>
        /// <returns></returns>
        private async UniTask<CheckConnectionResult> CheckConnection()
        {
            var req = new CheckConnectionReq()
            {
                id = DataManager.Instance.playerDataManager.Id
            };
            var reqJson = JsonConvert.SerializeObject(req);
            
            string resultJson = string.Empty;
            resultJson = await RequestPost("checkconnection/", reqJson);

            if (resultJson == string.Empty)
            {
                connectable = false;
                return CheckConnectionResult.NoConnectionToServer;
            }
            
            CheckConnectionRes result = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            if (result is { success: true, needInit: true })
            {
                connectable = true;
                return CheckConnectionResult.SuccessButNoIdInServer;
            }

            connectable = true;
            return CheckConnectionResult.Success;
        }

        
        public async UniTask<CreateNewUserResult> CreateNewUser(string userName)
        {
            var checkConnection = await CheckConnection();

            if (checkConnection == CheckConnectionResult.NoConnectionToServer)
            {
                return CreateNewUserResult.Fail;
            }

            var request = new CreateNewUserReq()
            {
                id = DataManager.Instance.playerDataManager.Id,
                topStage = DataManager.Instance.playerDataManager.TopStage,
                name = userName,
                baseAtk = DataManager.Instance.playerDataManager.BaseAtk,
                coin = DataManager.Instance.playerDataManager.Coin,
                enhanceIngredientList = DataManager.Instance.playerDataManager.EnhanceIngredientList,
            };
            var reqJson = JsonConvert.SerializeObject(request);
            string resultJson = string.Empty;
            resultJson = await RequestPost("createnewuser/", reqJson, false, 1);

            if (resultJson == string.Empty)
            {
                return CreateNewUserResult.NoConnectionToServer;
            }

            var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
            if (result is { success: false })
                return CreateNewUserResult.Fail;
            
            await UniTask.SwitchToMainThread();
            DataManager.Instance.playerDataManager.InitUser(result.id, userName);
            return CreateNewUserResult.Success;
        }

        public async UniTask<DeleteUserResult> DeleteUser(int id)
        {
            var checkConnection = await CheckConnection();

            if (checkConnection == CheckConnectionResult.NoConnectionToServer)
            {
                return DeleteUserResult.Fail;
            }
            
            var request = new DeleteUserReq()
            {
                id = id,
            };
            var reqJson = JsonConvert.SerializeObject(request);
            string resultJson = string.Empty;
            resultJson = await RequestPost("deleteuser/", reqJson, false, 1);

            if (resultJson == string.Empty) 
                return DeleteUserResult.Fail;

            var result = JsonConvert.DeserializeObject<DeleteUserRes>(resultJson);
            if (result is { success: false })
                return DeleteUserResult.NoUserInServer;

            return DeleteUserResult.Success;
        }

        public async UniTask<FetchUserResult> FetchUser()
        {
            var checkConnection = await CheckConnection();
            switch (checkConnection)
            {
                // No Connection
                case CheckConnectionResult.NoConnectionToServer:
                    return FetchUserResult.Fail;
                
                case CheckConnectionResult.Success:
                    break;
                
                case CheckConnectionResult.SuccessButNoIdInServer:
                    var userName = DataManager.Instance.playerDataManager.UserName;
                    if (userName == String.Empty)
                    {
                        UIManager.Instance.ShowPopupEnterYourNickname();
                        return FetchUserResult.Fail;
                    }
                    var createNewUser = await CreateNewUser(userName);

                    switch (createNewUser)
                    {
                        case CreateNewUserResult.Success:
                            await UniTask.SwitchToMainThread();
                            break;
                    }
                    break;
            }
            
            var request = new FetchUserReq()
            {
                id = DataManager.Instance.playerDataManager.Id,
                topStage = DataManager.Instance.playerDataManager.TopStage,
                name = DataManager.Instance.playerDataManager.UserName,
                baseAtk = DataManager.Instance.playerDataManager.BaseAtk,
                coin = DataManager.Instance.playerDataManager.Coin,
                enhanceIngredientList = DataManager.Instance.playerDataManager.EnhanceIngredientList,
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

        /// <summary>
        /// Cannot Triggered in non-internet environment
        /// </summary>
        /// <returns></returns>
        public async UniTask<AddItemsResult> AddRandomEquipItems(uint count)
        {
            var request = new AddEquipItemsReq()
            {
                id = DataManager.Instance.playerDataManager.Id,
                count = count
            };
            var reqJson = JsonConvert.SerializeObject(request);

            var resultJson = await RequestPost("addrandomequipmentitems/", reqJson, false);

            var returnResult = new AddItemsResult();

            if (resultJson == String.Empty)
            {
                returnResult.Success = false;
                return returnResult;
            }

            var result = JsonConvert.DeserializeObject<AddEquipItemsRes>(resultJson);

            returnResult.Success = true;
            returnResult.ItemEquipmentList = result.itemList;
            
            return returnResult;
        }

        public async UniTask<StaticDataJsonResult> GetStaticDataJsonList(List<string> staticDataReqList)
        {
            var checkConnection = await CheckConnection();

            if (checkConnection == CheckConnectionResult.NoConnectionToServer)
            {
                var ret = new StaticDataJsonResult
                {
                    Success = false
                };
                return ret;
            }
            
            var request = new StaticDataJsonReq()
            {
                id = DataManager.Instance.playerDataManager.Id,
                staticDataNameList = staticDataReqList
            };
            var reqJson = JsonConvert.SerializeObject(request);
            
            var resultJson = await RequestPost("getstaticdatajsonlist/", reqJson, false);
            var result = JsonConvert.DeserializeObject<StaticDataJsonRes>(resultJson);

            var returnResult = new StaticDataJsonResult();

            if (result != null)
            {
                returnResult.Success = result.success;
                returnResult.Data = result.staticDataJsonList;
            }
            else
            {
                Debug.LogAssertion("GetStaticDataJsonList failed: " + staticDataReqList);
                returnResult.Success = false;
            }
            
            return returnResult;
        }
    }
}
