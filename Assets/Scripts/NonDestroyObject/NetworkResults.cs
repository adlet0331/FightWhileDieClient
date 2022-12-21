namespace NonDestroyObject
{
    public enum BaseResult
    {
        No_Connection_To_Server,
        Success,
        Fail
    }
    public enum CheckConnectionResult
    {
        No_Connection_To_Server,
        Success,
        Success_But_No_Id_In_Server
    }
    public enum CreateNewUserResult
    {
        No_Connection_To_Server,
        Success,
        Fail
    }

    public enum DeleteUserResult
    {
        No_User_In_Server,
        Success,
        Fail
    }

    public enum FetchUserResult
    {
        No_Connection_To_Server,
        Success,
        Fail
    }
}