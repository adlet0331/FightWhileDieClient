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
}