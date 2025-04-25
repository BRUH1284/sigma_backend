namespace sigma_backend.Interfaces
{
    public interface IDataChecksumProvider
    {
        Task<string> ComputeUserDataChecksum(string userId);
    }
}