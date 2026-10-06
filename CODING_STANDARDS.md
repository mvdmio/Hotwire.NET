# Coding Standards

Match the existing file before applying general preferences. Style lives here and in the surrounding code; add `.editorconfig`, analyzers, or formatter config only when the user asks.

## C# Style

- Indent with 3 spaces; save `.cs` files as UTF-8 with BOM. With no `.editorconfig`, `dotnet format` and IDE defaults reindent to 4 spaces: reformat only the lines you change.
- File-scoped namespaces: `namespace Foo.Bar;`.
- Order `using` directives `System.*` first, then everything else alphabetically (`mvdmio.*` sorts before `Xunit`).
- Seal concrete classes not designed for inheritance.
- Put braces on their own lines; a single-statement `if`/`else`/loop body may omit braces (see `mvdmio.Hotwire.NET/ASP/Broadcasting/InMemoryTurboBroadcaster.cs`).
- Use `var` for locals; write an explicit type only where the compiler needs a target type, such as a collection expression.
- Settle nullability with guards, pattern matching, or `?`. Reserve `!` for values a preceding check or API contract guarantees non-null where the compiler cannot see it, e.g. after `.Where(x => x is not null)`.
- Keep production files under ~500 lines; split by responsibility past that. Test files are exempt.

## Public API

- Give every public library member a `/// <summary>`; implementations of documented interface members use `<inheritdoc />`. A gap shows up as build warning CS1591.
- Keep `[PublicAPI]` (JetBrains.Annotations) where it stands; add it to new public action and extension types.
- Grow the API additively: new members or overloads. Change or remove a public signature only when the task requires it; that is a breaking change (MAJOR, § Publishing).

## Error Handling

- Throw `InvalidOperationException` when required framework state is missing, e.g. no referer or no view engine.
- Core library logic lets exceptions propagate to the caller. Boundary code (middleware, websocket I/O) catches only the expected operational exceptions, logs them, and returns a fitting result or cleans up; `TurboStreamsWebsocketMiddleware` is the model.

## Testing

- Every new behaviour gets a test; every bug fix gets a test that fails without the fix.
- Test folders and namespaces mirror production areas minus the `ASP/` prefix (`ASP/TurboActions/` → `mvdmio.Hotwire.NET.Tests.TurboActions`).
- Name test classes after the target type or behaviour plus `Tests`; in new test files, name methods `Subject_ShouldOutcome` (`IsTurboRequest_ShouldReturnTrue`).
- Assert with FluentAssertions; write multi-line HTML expectations as raw string literals.
- Fake dependencies with real framework objects or the stubs in `mvdmio.Hotwire.NET.Tests/_Stubs/`.
- Subset runs: `--filter "FullyQualifiedName~mvdmio.Hotwire.NET.Tests.<Area>.<Class>[.<Method>]"`. Filtering a `[Theory]` by method name runs every one of its `[InlineData]` rows.

## Publishing

- Every push to `main` builds the library and runs `nuget push … -SkipDuplicate`. A version already on nuget.org is skipped without error, so a merge without a `<Version>` bump in `mvdmio.Hotwire.NET/mvdmio.Hotwire.NET.csproj` silently publishes nothing.
- Bump `<Version>` when the package contents change: library code, `build/`, vendored Hotwire JS. Changes confined to tests, example apps, or agent docs need none. A `Readme.md`-only change reaches nuget.org with the next version.
- Semantic versioning: MAJOR for breaking public API (§ Public API, § Packaging), MINOR for new API, PATCH for fixes.
- `Readme.md` is the nuget.org package page (`PackageReadmeFile`): write it for consumers (install, usage, API examples) and update it whenever public API or usage changes. Contributor and agent material belongs in `AGENTS.md` and this file.

## Hotwire JS Assets

- Turbo and Stimulus are vendored: `mvdmio.Hotwire.NET/package.json` pins them and `mvdmio.Hotwire.NET/node_modules/` is committed, so builds need no npm.
- Upgrade by editing the versions in `mvdmio.Hotwire.NET/package.json` and running `npm install` in that folder; commit `package.json`, `package-lock.json`, and the `node_modules/` diff.
- The csproj packs `node_modules/@hotwired/turbo/dist/turbo.es2017-esm.js` as `lib/turbo.js` and `node_modules/@hotwired/stimulus/dist/stimulus.js` as `lib/stimulus.js`; after an upgrade, confirm both source paths still exist. `build/mvdmio.Hotwire.NET.targets` copies the pair into the consumer's `wwwroot/js/lib` before compile.
- The example apps keep their own copies (§ Example Apps).

## Packaging

- CI publishes the `.nupkg` that `dotnet build` produces through `GeneratePackageOnBuild`; keep that property on.
- `build/*.props|targets` ship in the package and run only in projects that reference the package, so neither example app exercises local edits to them. Verify such edits against the `.nupkg` from `mvdmio.Hotwire.NET/bin/<Configuration>/`, consumed by a scratch project through a local package source.
- `HotwireCopyDefaultFiles` and `HotwireCopyBaseDirectory` are public API, documented in `Readme.md`; renaming one or changing its meaning is a breaking change.

## Example Apps

- `Example.ASP.NetCore.RazorPages` references the library project: exercise local library changes here.
- `Example.ASP.NetCore.MVC` references a pinned nuget.org version of the package; local library changes reach it only after a release and a version bump in its csproj.
- Both commit their own `wwwroot/js/lib/turbo.js` and `stimulus.js`, and builds never refresh them from the vendored assets: the RazorPages project reference skips the package's build targets, and the MVC build copies them from its pinned package. Copy upgraded assets into RazorPages by hand when it should run them.
