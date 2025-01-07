namespace ProjectBase.Business.Abstract
{
    public interface ILocalizationService
    {
        void SetCulture(string language);
        Task<string> GetStringAsync(string resourceCode, string key, string language = null);
    }
}
