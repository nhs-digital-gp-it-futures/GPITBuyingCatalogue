namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session
{
    public interface ISessionService
    {
        string GetString(string key);

        void SetString(string key, string value);

        T GetObject<T>(string key);

        void SetObject(string key, object value);

        void ClearSession();
    }
}
