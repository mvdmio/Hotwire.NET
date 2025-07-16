using System;

namespace mvdmio.Hotwire.NET.ASP.Broadcasting.ValueObjects;

/// <summary>
///   Value object for Connection IDs.
/// </summary>
public readonly record struct ConnectionId(Guid Value)
{
   /// <summary>
   ///   Constructor that initializes the connection ID with a new <see cref="Guid"/>.
   /// </summary>
   public ConnectionId()
      : this(Guid.NewGuid())
   {
   }
}