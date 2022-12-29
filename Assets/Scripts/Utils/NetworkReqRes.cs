using System;

namespace Utils
{
    [Serializable]
    public class BaseRequest
    {
        public int id;
    }

    [Serializable]
    public class BaseResponse
    {
        public bool success;
    }

    [Serializable]
    public class CheckConnectionReq : BaseRequest { }

    [Serializable]
    public class CheckConnectionRes : BaseResponse
    {
        public bool needInit;
    }

    [Serializable]
    public class CreateNewUserReq : BaseRequest
    {
        public string name;
        public int topStage;
        public int baseAtk;
        public int coin;
    }

    [Serializable]
    public class CreateNewUserRes : BaseResponse
    {
        public int id;
    }
    
    [Serializable]
    public class DeleteUserReq : BaseRequest { }

    [Serializable]
    public class DeleteUserRes : BaseResponse { }
    
    [Serializable]
    public class FetchUserReq : BaseRequest
    {
        public string name;
        public int topStage;
        public int baseAtk;
        public int coin;
    }

    [Serializable]
    public class FetchUserRes : BaseResponse { }
}