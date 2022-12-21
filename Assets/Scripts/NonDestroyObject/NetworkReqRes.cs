using System;

namespace NonDestroyObject
{
    [Serializable]
    public class BaseRequest
    {
        public int Id;
    }

    [Serializable]
    public class BaseResponse
    {
        public bool success;
    }

    [Serializable]
    public class CheckConnectionReq : BaseRequest
    {
        
    }

    [Serializable]
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
    
    [Serializable]
    public class DeleteUserReq : BaseRequest
    {
        public int Id;
    }

    [Serializable]
    public class DeleteUserRes : BaseResponse
    {
        public bool success;
    }
    
    [Serializable]
    public class FetchUserReq : BaseRequest
    {
        public string Name;
        public int TopStage;
        public int BaseAtk;
        public int Coin;
    }

    [Serializable]
    public class FetchUserRes : BaseResponse
    {
        
    }
}