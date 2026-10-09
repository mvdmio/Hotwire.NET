# Sign broadcast channel names with Data Protection so signatures survive app restarts

## Problem Statement

A web app that uses Turbo Streams broadcasting loses its live channels on every restart. Each page that shows a `<turbo-broadcast-channel>` gets a signed channel name in its socket URL. The package signs that name with an RSA key pair that `RsaChannelEncryption` creates in memory when the app starts. A restart or deploy creates a new key pair. Every page rendered before the restart then holds a signature the new process cannot verify.

When such a page opens its socket again, the websocket middleware answers HTTP 400. It also logs the warning "Received websocket connection request with invalid signature". The page itself looks fine, but its live updates never arrive. The user has to reload the page by hand.

This happens often in practice:

- A Turbo navigation keeps a `data-turbo-permanent` element in place. That element moves to the new page and opens its socket again with the URL it was first given. If that URL came from the old process, the socket gets a 400 on every navigation until a full reload.
- A page restored from Turbo's snapshot cache, or from the browser's back-forward cache, carries the old URL too.
- An app that runs more than one instance behind a load balancer has a different key pair in each instance. A page rendered by one instance cannot open its socket on another.

The mvdmio suite saw this in production on 2026-10-08 (mvdmio-suite #262, Sentry COMPLIANCE-4). A user opened the Compliance Assistant panel at 17:57 UTC. The panel is a permanent element. Compliance deployed twice after that. At 19:40 the user moved between pages within the same tab. The panel reconnected with the 17:57 signature, got 400 twice, and stopped receiving the Assistant's streamed replies. The suite deploys several times a day, so every open tab is exposed after each deploy.

## Solution

Sign channel names with ASP.NET Core Data Protection instead of an in-memory key. Data Protection is the key system ASP.NET Core already uses for authentication cookies and antiforgery tokens. A host that keeps its Data Protection keys across restarts then also keeps its channel signatures valid across restarts. A host that shares its key ring across instances also shares valid signatures across instances.

`AddTurboStreamsBroadcasting()` registers the new Data Protection signer as the default `IChannelEncryption`. Apps that already call it get the fix by updating the package, with no code change. Rails' turbo-rails works the same way: it signs stream names with the app's permanent secret, so they survive a deploy.

After this change, a page rendered before a restart opens its socket on the restarted app with HTTP 101, the same as a freshly rendered page. A forged, edited, or foreign token still gets HTTP 400 and the same warning.

## User Stories

1. As a user with a page open during a deploy, I want that page's live channel to accept my next reconnect, so that I do not have to reload to see live updates.
2. As a user using a panel marked `data-turbo-permanent`, I want the panel's socket to connect after a Turbo navigation that follows a deploy, so that the panel keeps streaming.
3. As a user who presses Back after a deploy, I want the restored page's live channel to connect, so that a cached page is not silently dead.
4. As a user of an app that runs two instances, I want a page rendered by one instance to connect to the other, so that load balancing does not break live updates.
5. As an app developer, I want the fix to arrive by updating the package, so that I do not change my `Program.cs`.
6. As an app developer, I want the signer to use the Data Protection key ring my app already configures, so that I manage one set of keys, not two.
7. As an app developer whose host keeps Data Protection keys on disk or in a database, I want channel signatures to last as long as those keys, so that restarts stop breaking open pages.
8. As an app developer whose host keeps no Data Protection keys between restarts, I want the package to behave no worse than today, so that the upgrade never makes things worse.
9. As an app developer, I want to replace the default signer by registering my own `IChannelEncryption`, so that I keep control where I need it.
10. As an app developer, I want `IChannelEncryption` to keep its shape, so that my own implementations and test doubles still compile.
11. As an app developer, I want the websocket middleware to keep answering 400 for a token it cannot verify, so that tampering is still refused.
12. As an operator, I want the "invalid signature" warning to keep firing for a token that fails verification, so that a forged token or a lost key ring still shows in the logs.
13. As an operator, I want that warning to stop firing after a normal deploy, so that the warning means something again.
14. As an app developer, I want any channel name to work, including names with a period, pipe, space, parentheses, colon, or non-ASCII letters, so that I do not have to encode names myself.
15. As an app developer, I want the signed token to fit in one URL path segment without escaping, so that the browser cannot change it on the way to the server.
16. As an app developer, I want a token made for one purpose never to verify as a channel token, so that other Data Protection payloads cannot open a socket.
17. As an app developer, I want a channel token to verify after my Data Protection key ring rotates to a new key, so that a 90-day key rollover does not break long-open tabs.
18. As an app developer, I want channel tokens not to expire on a timer, so that a tab left open for days still connects.
19. As an app developer, I want signing to be safe when many requests run at once, so that busy pages do not need a lock.
20. As an app developer who uses `@Html.TurboBroadcastChannel(...)`, I want the HTML helper to use the same signer, so that both rendering paths produce tokens the middleware accepts.
21. As an app developer who still wants the old in-memory behaviour, I want `RsaChannelEncryption` to stay available as a public class, so that I can register it on purpose.
22. As a reader of the README, I want the broadcasting section to say that signatures depend on the app's Data Protection key ring, so that I know how to keep them valid across restarts and instances.

## Implementation Decisions

- Add a new public `IChannelEncryption` implementation backed by `IDataProtectionProvider`. It creates one `IDataProtector` with a fixed purpose string that belongs to this package's channel signing, for example `mvdmio.Hotwire.NET.Broadcasting.Channel`. No other code may use that purpose.
- `Encrypt` returns the protector's string output for the channel name. Data Protection's string form is Base64 URL without padding. It has no period, no `%`, and no character a browser escapes in a path. The channel name is encrypted inside the token, so it no longer appears in the socket URL.
- `Decrypt` unprotects the token and returns the original channel name. Any failure to verify becomes an `ArgumentException`. That covers a `CryptographicException` (wrong key, edited or foreign payload, other purpose) and a `FormatException` (input that is not Base64 URL). The middleware already maps `ArgumentException` to HTTP 400 and the "invalid signature" warning, and that mapping stays as it is.
- Tokens have no expiry. Do not use a time-limited protector.
- `AddTurboStreamsBroadcasting()` calls `AddDataProtection()`, which is safe to call more than once, so that `IDataProtectionProvider` always resolves. It registers the new signer as the default `IChannelEncryption` instead of `RsaChannelEncryption`. The host's own Data Protection setup (key storage location, key encryption, application name) applies unchanged.
- An app that registers its own `IChannelEncryption` after calling `AddTurboStreamsBroadcasting()` still wins, as it does today.
- The `IChannelEncryption` interface keeps its two methods and their names.
- `RsaChannelEncryption` stays public and unchanged. It is no longer the default. Its doc comment keeps saying that its key pair dies with the process.
- The websocket middleware, the `<turbo-broadcast-channel>` tag helper, and the HTML helper keep their behaviour. They already resolve `IChannelEncryption` from the container.
- This adds a default and keeps every public type, so the package version goes up by a minor step (1.7.2 → 1.8.0). The repo's AGENTS.md requires a version bump and a README update with every change.
- The README's Turbo Broadcasting section explains three things. Channel names are signed with Data Protection. Signatures survive restarts only when the app keeps its Data Protection keys between restarts. More than one instance needs a shared key ring and the same Data Protection application name.

## Testing Decisions

A good test proves what an app sees: a token issued before a restart is accepted after it, and a bad token is refused. Do not assert the exact token text or its length. Those are Data Protection's business.

- **Restart test (the main proof).** Build one service provider with `AddTurboStreamsBroadcasting()` and Data Protection keys stored in a temporary folder. Resolve `IChannelEncryption` and sign a channel name. Dispose that provider. Build a second provider over the same folder, which stands in for the restarted process. Its `IChannelEncryption` must return the original channel name for the first provider's token.
- **Foreign key ring test.** A token signed by a provider that uses a different key folder must make `Decrypt` throw `ArgumentException`.
- **Edited token test.** Change one character of a valid token. `Decrypt` must throw `ArgumentException`.
- **Garbage input test.** Input that is not Base64 URL, such as a string containing a period or a space, must make `Decrypt` throw `ArgumentException`, not `FormatException`.
- **Other purpose test.** A payload protected by the same provider under a different purpose must make `Decrypt` throw `ArgumentException`.
- **Channel name round trip.** Names such as `record-page:2:Requirement:iso-27001-2022|5.2`, `eu-gdpr-2016|Art. 5(1)(a)`, and a name with non-ASCII letters must come back unchanged.
- **Path-safe token.** Every token from the round-trip test is unchanged by URL path escaping (`Uri.EscapeDataString` returns the same string). This is the contract the middleware relies on, not a format detail.
- **Concurrency.** Keep the existing many-tasks test idea: 100 parallel sign-and-verify calls all round-trip.
- **Middleware refusal.** Run the websocket middleware with the real Data Protection signer and a path that carries a foreign-key-ring token. The response must be 400, and the broadcaster must get no connection.
- **Registration.** After `AddTurboStreamsBroadcasting()`, resolving `IChannelEncryption` gives the Data Protection signer. A registration the app adds afterwards replaces it.
- Keep the existing `RsaChannelEncryption` tests. That class still exists.
- Prior art: the existing tests for `RsaChannelEncryption` and the websocket middleware, including the middleware's test doubles for the broadcaster, the application lifetime, and the websocket feature. Use `DataProtectionProvider.Create` over a temporary folder, or `AddDataProtection().PersistKeysToFileSystem(...)` on a `ServiceCollection`, to stand in for a host's persisted key ring.

## Out of Scope

- **Reconnect after the socket closes.** Turbo's `<turbo-stream-source>` element (Turbo 8.0.13 as this package bundles it, and 8.0.23 in the mvdmio suite) opens one WebSocket when it is added to the page and never reconnects after that socket closes. A restart therefore still closes every open socket. The page gets live updates again only when the element is added to a page again, for example after a Turbo navigation. This Spec makes that later connect succeed. Reconnecting on its own is a separate change and is not filed.
- Changing the log level or wording of the "invalid signature" warning.
- Persisting or configuring Data Protection keys. That is the host app's job.
- Removing `RsaChannelEncryption`.
- Any change in the mvdmio suite. mvdmio-suite #262 bumps the package there after this ships, and it is blocked by this Issue.

## Further Notes

- Sentry COMPLIANCE-4 sample URL: `/turbo/ws/assistant-9805a57c-9b96-4870-96d9-6250c6f01054.<signature>`. A Guid channel has no period in it, so this is not the period-split fault behind COMPLIANCE-1 (2026-09-01). That earlier fault came from `RsaChannelEncryption.Decrypt` splitting the signed name on `.` and requiring exactly two parts. The suite worked around it by Base64-URL-encoding the record id in its channel names. With an opaque Data Protection token, a period in the channel name no longer matters. The suite's workaround stays harmless.
- Production traces for that event show the Assistant panel rendered at 17:57:44 UTC by release `945a390`. Compliance restarted at 18:00 and 18:16. At 19:40:47 the user made Turbo navigations with no new panel request, and the panel's socket got 400 twice.
- The suite stores its Data Protection keys on disk across deploys and protects them with a certificate. After the package bump, suite channel signatures will survive deploys with no suite-side setup.
- Pushing to `main` publishes the package to NuGet through the repo's publish workflow. NuGet skips a version it already has, so the version bump is what makes a release.

