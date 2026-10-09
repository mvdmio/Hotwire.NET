namespace mvdmio.Hotwire.NET.Tests._Stubs;

/// <summary>
///   A temporary folder that stands in for a host's persisted Data Protection key ring. Deleted on dispose.
/// </summary>
public sealed class TemporaryKeyDirectory : IDisposable
{
   public DirectoryInfo Directory { get; } = System.IO.Directory.CreateTempSubdirectory("hotwire-dp-keys-");

   public void Dispose()
   {
      Directory.Delete(recursive: true);
   }
}
