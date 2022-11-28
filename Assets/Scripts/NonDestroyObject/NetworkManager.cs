using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NonDestroyObject
{
    [Serializable]
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
        public bool NeedInit;
    }

    [Serializable]
    public class CreateNewUserReq : BaseRequest
    {
        public string Name;
        public int TopStage;
        public int BaseAtk;
        public int Coin;
    }

    [Serializable]
    public class CreateNewUserRes : BaseResponse
    {
        public int Id;
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
        
        public void CheckConnection()
        {
            var req = new BaseRequest()
            {
                Id = SLManager.Instance.Id
            };
            var reqJson = JsonConvert.SerializeObject(req);

            var resultJson = RequestPost("/playerserver/checkconnection/", reqJson);
            CheckConnectionRes res = JsonConvert.DeserializeObject<CheckConnectionRes>(resultJson);

            try
            {
                connectable = res.success;
                if (connectable && res.NeedInit)
                {
                    UIManager.Instance.ShowPopupEnterYourNickname();
                }
            }
            catch (NullReferenceException e)
            {
                connectable = false;
                return;
            }
        }

        public bool CreateNewUser(string userName)
        {
            if (!connectable) return false;
            
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
                return false;
            }
            else
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<CreateNewUserRes>(resultJson);
                    if (!result.success)
                    {
                        Debug.LogAssertion("CreateNewUser Failed");
                        return false;
                    }
                    else
                    {
                        playerId = result.Id;
                        return true;
                    }
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }
    }
}
