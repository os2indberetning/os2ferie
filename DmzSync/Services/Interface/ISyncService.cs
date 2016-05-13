namespace DmzSync.Services.Interface
{
    public interface ISyncService
    {
        void SyncFromDmz();
        void SyncToDmz();
        void ClearDmz();
    }
}
