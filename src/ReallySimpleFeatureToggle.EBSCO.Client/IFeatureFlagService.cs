namespace ReallySimpleFeatureToggle.EBSCO.Client
{
    public interface IFeatureFlagService
    {
        void Initialize();
        bool IsAvailable(string featureName);
    }
}