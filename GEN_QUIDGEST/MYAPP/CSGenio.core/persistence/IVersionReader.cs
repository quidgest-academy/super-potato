namespace CSGenio.core.persistence
{
    
    public interface IVersionReader
    {
        int GetDbVersion();

        int GetDbIndexVersion();

        int GetDbUpgradeVersion();
    }
}
    