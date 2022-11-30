using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace NonDestroyObject
{
    public enum ResultStatus
    {
        Success = 0,
        NoResponse = 1,
        NotProperResponse = 2,
        Failed = 3
    }
    public class NetworkManager : Singleton<NetworkManager>
    {
        public int playerId;
        [SerializeField] private string _rootURL;
        public bool connectable;

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
        
        public ResultStatus CheckConnection()
        {
            var req = new CheckConnectionReq()
            {
                Id = SLManager.Instance.Id
            };
            var reqJson = JsonConvert.SerializeObject(req);

            var resultJson = RequestPost("/playerserver/checkconnection/", reqJson);
            CheckConnectionRes res = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            connectable = res.success;
            if (connectable && res.NeedInit)
            {
                UIManager.Instance.ShowPopupEnterYourNickname();
            }

            return ResultStatus.Success;
        }

        public ResultStatus CreateNewUser(string userName)
        {
            if (!connectable) return ResultStatus.NoResponse;
            
            var request = new CreateNewUserReq()
            {
                Id = SLManager.Instance.Id,
                TopStage = SLManager.Instance.TopStage,
                Name = userName,
                BaseAtk = SLManager.Instance.BaseAtk,
                Coin = SLManager.Instance.Coin
            };
            var reqJson = JsonConvert.SerializeObject(request);
            
            var resultJson = RequestPost("/playerserver/createnewuser/", reqJson);
            Debug.Log(resultJson);
            if (resultJson == string.Empty)
            {
                return ResultStatus.NotProperResponse;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
                if (!result.success)
                {
                    return ResultStatus.Failed;
                }
                else
                {
                    playerId = result.Id;
                    return ResultStatus.Success;
                }
            }
        }

        public ResultStatus FetchUser()
        {
            if (!connectable) return ResultStatus.NoResponse;
            
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
            if (resultJson == string.Empty)
            {
                return ResultStatus.NotProperResponse;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
                if (!result.success)
                {
                    return ResultStatus.Failed;
                }
                else
                {
                    playerId = result.Id;
                    return ResultStatus.Success;
                }
            }
        }
    }
}
