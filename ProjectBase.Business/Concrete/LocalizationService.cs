using ProjectBase.Business.Abstract;
using ProjectBase.Core.Abstract;
using ProjectBase.Core.Concrete;
using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Database;
using ProjectBase.Entity.Global;
using Microsoft.AspNetCore.Http;

namespace ProjectBase.Business.Concrete
{
    [IocContainerItem(typeof(ILocalizationService))]
    public class LocalizationService : ILocalizationService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IResourceValueRepository _resourceValueRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CacheManager _cacheManager;

        public LocalizationService(ILanguageRepository languageRepository,IResourceRepository resourceRepository,IResourceValueRepository resourceValueRepository,
            IHttpContextAccessor httpContextAccessor,CacheManager cacheManager)
        {
            _languageRepository = languageRepository;
            _resourceRepository = resourceRepository;
            _resourceValueRepository = resourceValueRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
        }
        public async Task<string> GetStringAsync(string resourceCode, string key, string language = null)
        {
            language ??= _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].ToString().Split('-').FirstOrDefault() ?? "en";

            var item = await _resourceValueRepository.GetSingleAsync(p => p.ResourceCode == resourceCode && p.Key == key && p.LanguageCode == language);
            if(item == null)
            {
                return string.Empty;
            }
            else
            {
                return item.Value;
            }
        }
        private async Task<List<Language>> GetLanguages()
        {
            string cacheKey = $"{CommonConstantGenerator.CachePrefix.Languages}-{CommonConstantGenerator.CachceKey.AllLanguages}";
            var languages =  await _cacheManager.GetAsync<List<Language>>(cacheKey);
            if(languages == null)
            {
                var items = _languageRepository.GetAll().ToList();
                _cacheManager.SetAsync(cacheKey, items);
                return items;
            }
            return languages;
        }

        public void SetCulture(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var culture = new System.Globalization.CultureInfo(language);
                System.Globalization.CultureInfo.CurrentCulture = culture;
                System.Globalization.CultureInfo.CurrentUICulture = culture;
            }
        }
    }
}
