# Coding Standards

## Done When

Completion criterion for every code task: every item passes.

- **Build**: `dotnet build mvdmio.Hotwire.NET.sln` green with zero warnings. File "being used by another process" → kill that process, rebuild.
- **Build churn**: the build rewrites both examples' `Tailwind/tailwind.config.js` and regenerates RazorPages' committed `wwwroot/css/tailwind.output.css`. Keep those diffs only when the task changes Tailwind config or example views; else `git restore` them.
- **Tests** green. Abort "You must install or update .NET … version '8.0.0'" → rerun as `DOTNET_ROLL_FORWARD=Major dotnet test …`.
- **Test coverage**: each new behaviour has a test; each bug fix has a test that fails without the fix.
- **Readme**: `Readme.md` updated when public API or usage changed. It is the nuget.org package page: write it for consumers (install, usage, API examples).
- **Version**: `<Version>` in `mvdmio.Hotwire.NET/mvdmio.Hotwire.NET.csproj` bumped (semver) when package contents changed: library code, `build/`, vendored JS. Changes confined to tests, example apps, or agent docs need none; a `Readme.md`-only edit ships with the next version. Every push to `main` runs `nuget push … -SkipDuplicate`, which skips an already-published version without error: an unbumped merge silently publishes nothing.
- **Rules**: every rule below holds for the diff.

## C# Style

- **Indent** 3 spaces; `.cs` files UTF-8 with BOM. Format by hand, touching only lines the task changes: with no `.editorconfig`, `dotnet format` and IDE reformat reindent whole files to 4 spaces.
- **Usings**: `System.*` first, then the rest alphabetically, ignoring case (`mvdmio.*` before `Xunit`).
- **Seal** concrete classes not designed for inheritance.
- **`var`** for every local; explicit type only where the compiler needs a target type (collection expressions).
- **Nullability**: settle with guards, pattern matching, or `?`; `!` only where a preceding check or API contract guarantees non-null out of the compiler's sight (after `.Where(x => x is not null)`).
- **File size**: production files under ~500 lines; split by responsibility. Test files exempt.

## Public API

- **XML docs**: `/// <summary>` on every public member; `<inheritdoc />` on implementations of documented interface members.
- **`[PublicAPI]`** (JetBrains.Annotations) on new public action and extension types.
- **Additive**: grow through new members or overloads. Changing or removing a public signature, or renaming or redefining the Readme-documented MSBuild properties `HotwireCopyDefaultFiles` / `HotwireCopyBaseDirectory`, is breaking (MAJOR): only when the task requires it.

## Error Handling

- **Core** library code lets exceptions propagate to the caller.
- **Boundary** code (middleware, websocket I/O) catches the typed operational exceptions it expects, logs them, then returns a result or cleans up; a catch-all only as the outermost log guard. Model: `TurboStreamsWebsocketMiddleware`.

## Tests

- **Layout** mirrors production minus `ASP/` (`ASP/TurboActions/` → `mvdmio.Hotwire.NET.Tests.TurboActions`).
- **Names**: methods in new test files read `Subject_ShouldOutcome` (`IsTurboRequest_ShouldReturnTrue`).
- **Assertions**: FluentAssertions; multi-line HTML expectations as raw string literals.
- **Fakes**: real framework objects or the stubs in `mvdmio.Hotwire.NET.Tests/_Stubs/`.
- **`[Theory]`** filtered by method name runs every `[InlineData]` row: expect one result per row.

## Packaging

- **`GeneratePackageOnBuild`** stays on: CI pushes the `.nupkg` that `dotnet build` produces and has no pack step.
- **`build/*.props|targets`** edits reach neither example (§ Example Apps). Verify them against the `.nupkg` from `mvdmio.Hotwire.NET/bin/<Configuration>/`, consumed by a scratch project through a local package source.
- **Vendored JS**: `mvdmio.Hotwire.NET/node_modules/` is committed; the csproj packs `turbo.js` and `stimulus.js` from it. Upgrade with `npm install` in `mvdmio.Hotwire.NET/`; commit the `node_modules/` diff with `package.json` and `package-lock.json`.

## Example Apps

- **RazorPages** references the library project: demo local library changes here. A project reference skips the package's `build/` targets.
- **MVC** pins a nuget.org release of the package: it sees library and `build/` changes only after a publish and a bump of that reference.
- **JS copies**: each example commits its own `wwwroot/js/lib/turbo.js` and `stimulus.js`; builds leave them stale after a vendored upgrade (RazorPages skips the targets; MVC copies from its pinned release). Copy upgraded files into RazorPages by hand when it should run them.
