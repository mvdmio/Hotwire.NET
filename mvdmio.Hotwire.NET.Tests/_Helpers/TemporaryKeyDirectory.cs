namespace mvdmio.Hotwire.NET.Tests._Helpers;

/// <summary>
///   A temporary folder that stands in for a host's persisted Data Protection key ring. Deleted on dispose.
/// </summary>
public sealed class TemporaryKeyDirectory : IDisposable
{
   public DirectoryInfo DirectoryInfo { get; } = Directory.CreateTempSubdirectory("hotwire-dp-keys-");

   public void Dispose()
   {
      DirectoryInfo.Delete(recursive: true);
   }
}
