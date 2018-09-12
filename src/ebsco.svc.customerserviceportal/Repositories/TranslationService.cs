using ebsco.svc.customerserviceportal.translations;

namespace ebsco.svc.customerserviceportal.Repositories
{
    public class TranslationService : ITranslationsService
    {
        public string TranslateResource(string translationKey)
        {
            return Resources.Translate(translationKey);
        }
    }
}
