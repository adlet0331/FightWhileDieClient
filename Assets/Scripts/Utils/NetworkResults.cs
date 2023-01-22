using System.Collections.Generic;
using Data;

namespace NonDestroyObject
{
    public enum CheckConnectionResult
    {
        NoConnectionToServer,
        Success,
        SuccessButNoIdInServer
    }
    public enum CreateNewUserResult
    {
        NoConnectionToServer,
        Success,
        Fail
    }

    public enum DeleteUserResult
    {
        NoUserInServer,
        Success,
        Fail
    }

    public enum FetchUserResult
    {
        NoConnectionToServer,
        Success,
        Fail
    }

    public class AddItemsResult
    {
        public bool Success;
        public List<EquipItemObject> ItemEquipmentList;
    }

    public class StaticDataJsonResult
    {
        public bool Success;
        public List<string> Data;
    }
}