# 01 — Data Protection signs broadcast channel names by default

Status: pending
Depends on: none

## What to build

An app that calls `AddTurboStreamsBroadcasting()` signs its channel names with ASP.NET Core Data Protection instead of an RSA key pair held in memory. A socket URL rendered by one process verifies in a later process, or in another instance, that uses the same Data Protection key ring. A forged, edited, foreign, or malformed token still gets HTTP 400 from the websocket middleware and the same "invalid signature" warning. Apps get this by updating the package. They change no code.

The new signer:

- It is a new public sealed class `DataProtectionChannelEncryption : IChannelEncryption` in `mvdmio.Hotwire.NET.ASP.Broadcasting`, next to `RsaChannelEncryption`. Its constructor takes an `IDataProtectionProvider`. It creates one `IDataProtector` once, in the constructor, with the purpose string `mvdmio.Hotwire.NET.Broadcasting.Channel`. That string is a private constant and is not public API. It must never change, because changing it makes every issued token invalid. No other code uses that purpose.
- `Encrypt` returns the protector's string output: `Protect(string)`, which is Base64 URL without padding. The channel name sits encrypted inside the token, so it no longer appears in the URL.
- `Decrypt` returns `Unprotect(string)`. It turns every verification failure into `ArgumentException("Invalid channel name", nameof(signedChannelName), inner)`. Those failures include `CryptographicException` (wrong key ring, edited payload, other purpose) and `FormatException` (input that is not Base64 URL). A token from the old RSA format, `name.signature`, therefore also gets a 400.
- Tokens have no expiry: do not use a time-limited protector. The plain `Unprotect` already accepts payloads from expired keys that were not revoked, so tokens survive key rotation without extra code.
- It needs no lock, because `IDataProtector` is thread-safe. It is registered as a singleton.

Registration and surrounding code:

- `AddTurboStreamsBroadcasting()` calls `services.AddDataProtection()`, which is safe to call more than once. It then registers `DataProtectionChannelEncryption` as the singleton `IChannelEncryption` in place of `RsaChannelEncryption`. Keep plain `AddSingleton`, not `TryAdd`, so that an `IChannelEncryption` the app registers afterwards still wins, as it does today.
- `IChannelEncryption` keeps its two methods and their names. Add only an `<exception cref="ArgumentException">` doc line on `Decrypt`. It states that a token that fails verification throws `ArgumentException`, which is the contract the middleware relies on.
- `RsaChannelEncryption`, the websocket middleware, the tag helper, and the HTML helper keep their code and behaviour.
- Bump the package version from 1.7.2 to 1.8.0.
- In the README's "Turbo Broadcasting" section, say three things. Channel names are signed with ASP.NET Core Data Protection. Signatures survive restarts only when the app keeps its Data Protection keys between restarts, for example with `PersistKeysToFileSystem`. More than one instance needs a shared key ring and the same `SetApplicationName`. You may also mention that the app can register its own `IChannelEncryption`, for example `RsaChannelEncryption`, after `AddTurboStreamsBroadcasting()`.

Tests prove what an app sees. Never assert the exact text or length of a token. Each test that uses keys on disk gets its own temporary folder and deletes it afterwards. To stand in for a host's key ring, use `DataProtectionProvider.Create(new DirectoryInfo(...))` or `AddDataProtection().PersistKeysToFileSystem(...)`.

## Footprint

Projects: `mvdmio.Hotwire.NET`, `mvdmio.Hotwire.NET.Tests`, `Example.ASP.NetCore.RazorPages` (it references the library as a project and calls `AddTurboStreamsBroadcasting()`; building it rewrites `Example.ASP.NetCore.RazorPages/Tailwind/tailwind.config.js`, so revert that file before you commit)

- `mvdmio.Hotwire.NET/ASP/Broadcasting/DataProtectionChannelEncryption.cs` — new `DataProtectionChannelEncryption`
- `mvdmio.Hotwire.NET/ASP/Broadcasting/Interfaces/IChannelEncryption.cs` — `IChannelEncryption.Decrypt` doc comment only
- `mvdmio.Hotwire.NET/ASP/Extensions/AspWebExtensions.cs` — `AspWebExtensions.AddTurboStreamsBroadcasting`
- `mvdmio.Hotwire.NET/mvdmio.Hotwire.NET.csproj` — `<Version>`
- `Readme.md` — "Turbo Broadcasting" section
- `mvdmio.Hotwire.NET.Tests/Broadcasting/DataProtectionChannelEncryptionTests.cs` — new: foreign key ring, edited token, garbage input, other purpose, round trip, path-safe token, concurrency
- `mvdmio.Hotwire.NET.Tests/Extensions/AspWebExtensions/AddTurboStreamsBroadcastingTests.cs` — new: restart test, registration tests
- `mvdmio.Hotwire.NET.Tests/Broadcasting/TurboStreamsWebsocketMiddlewareTests.cs` — middleware refusal test; reuse `TestTurboBroadcaster`, `TestHostApplicationLifetime`, `TestHttpWebSocketFeature`
- `mvdmio.Hotwire.NET.Tests/Broadcasting/RsaChannelEncryptionTests.cs` — unchanged, must stay green

## Acceptance criteria

- [ ] Restart: provider A is built with `AddTurboStreamsBroadcasting()`, with Data Protection keys persisted to a temporary folder. It signs a channel name and is then disposed. Provider B is built the same way over the same folder. B's `IChannelEncryption.Decrypt` returns the original name for A's token.
- [ ] Foreign key ring: a token signed through a provider over a different key folder makes `Decrypt` throw `ArgumentException`.
- [ ] Edited token: changing one character in the middle of a valid token to a different Base64 URL character makes `Decrypt` throw `ArgumentException`. Do not edit the last character, because its low bits can be unused.
- [ ] Garbage input: input with a period or a space, such as `abc.def` or `abc def`, makes `Decrypt` throw `ArgumentException`, not `FormatException`.
- [ ] Other purpose: a payload protected by the same provider under a different purpose makes `Decrypt` throw `ArgumentException`.
- [ ] Round trip: `record-page:2:Requirement:iso-27001-2022|5.2`, `eu-gdpr-2016|Art. 5(1)(a)`, and a name with non-ASCII letters come back unchanged.
- [ ] Path-safe token: for every round-trip token, `Uri.EscapeDataString(token)` equals `token`.
- [ ] Concurrency: 100 parallel sign-and-verify calls on one signer all round-trip.
- [ ] Middleware refusal: the websocket middleware runs with a real `DataProtectionChannelEncryption` and the path `/turbo/ws/<foreign-key-ring token>`. The response is 400, and the broadcaster's `AddConnection` is not called.
- [ ] Registration: after `AddTurboStreamsBroadcasting()`, resolving `IChannelEncryption` gives a `DataProtectionChannelEncryption`. An `IChannelEncryption` registered afterwards replaces it.
- [ ] `IChannelEncryption` keeps its two members. `RsaChannelEncryption` is still public and unchanged, and its existing tests pass.
- [ ] The package version is 1.8.0, and the README's Turbo Broadcasting section covers Data Protection signing, persisted keys for restarts, and a shared key ring with the same application name for more than one instance.
- [ ] `DOTNET_ROLL_FORWARD=Major dotnet test mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj` passes. This machine has only the .NET 10 runtime, and the tests target net8.0. `dotnet build Example.ASP.NetCore.RazorPages/Example.ASP.NetCore.RazorPages.csproj` succeeds.
