# AGENTS.md

## General instructions

- Style: telegraph. Drop filler/grammar. Min tokens (agent docs + replies).
- Coding standards: read `CODING_STANDARDS.md` before writing or reviewing code.
- Keep files shorter than ~500 LOC; split/refactor as needed. Does not apply to test files.
- Always add tests when adding functionality.
- Always create or modify tests when fixing a bug.
- Always build the solution and run the tests after making changes. Fix all build errors and test failures before finishing your work.
- If the build fails because some process is running and locking the file, kill the process.
- Always update `Readme.md` so that it reflects the latest state of the project; it is packed as the NuGet package page.
- Always bump `<Version>` in `mvdmio.Hotwire.NET/mvdmio.Hotwire.NET.csproj` when a change affects the published package. Follow semantic versioning (MAJOR.MINOR.PATCH). Every push to `main` runs `nuget push -SkipDuplicate`, so a merge without a bump silently does not publish.

## Repository Structure

Solution: `mvdmio.Hotwire.NET.sln`
Library: `mvdmio.Hotwire.NET/mvdmio.Hotwire.NET.csproj`
Tests: `mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj`
Example MVC app: `Example.ASP.NetCore.MVC/`
Example Razor Pages app: `Example.ASP.NetCore.RazorPages/`
Packaging assets: `mvdmio.Hotwire.NET/build/`
Hotwire JS dependency manifest: `mvdmio.Hotwire.NET/package.json`

## Project Facts

The library targets `net8.0` and `net9.0`.
The tests target `net8.0`.
The example apps target `net8.0`.
The library has `Nullable` enabled, `ImplicitUsings` disabled, and `LangVersion` set to `latest`.
The library builds a NuGet package on every build via `GeneratePackageOnBuild=True`.
There is no repo-level `.editorconfig`, `Directory.Build.props`, `Directory.Build.targets`, StyleCop config, or ESLint config.

## Build Commands

Run from repo root unless noted.

Restore solution:

```sh
dotnet restore ./mvdmio.Hotwire.NET.sln
```

Build solution:

```sh
dotnet build ./mvdmio.Hotwire.NET.sln
```

Build library only:

```sh
dotnet build ./mvdmio.Hotwire.NET/mvdmio.Hotwire.NET.csproj
```

Build tests only:

```sh
dotnet build ./mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj
```

Notes:
Building the solution also builds both example apps.
Example app builds run Tailwind through `mvdmio.Tailwind.NET` and may regenerate the committed `tailwind.output.css`.

## Test Commands

Run all tests:

```sh
dotnet test ./mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj
```

Run one test class:

```sh
dotnet test ./mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj --filter "FullyQualifiedName~mvdmio.Hotwire.NET.Tests.Extensions.RequestExtensions.IsTurboRequestTests"
```

Run one test method:

```sh
dotnet test ./mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj --filter "FullyQualifiedName~mvdmio.Hotwire.NET.Tests.TurboActions.ReplaceTurboActionTests.WithRequiredParameters"
```

Use `--no-build` after a successful build:

```sh
dotnet test ./mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj --no-build
```

Collect coverage if needed:

```sh
dotnet test ./mvdmio.Hotwire.NET.Tests/mvdmio.Hotwire.NET.Tests.csproj --collect:"XPlat Code Coverage"
```

Important xUnit note: filtering a `[Theory]` method by method name runs every inline-data row for that method.

## Lint / Format

No dedicated lint command is checked in.
Add linting, formatting, or analyzer tooling (`.editorconfig`, analyzer packages, formatter config) only when the user asks.
If you use a formatter locally, keep the diff minimal and match surrounding style.

## npm / JavaScript

There are no npm scripts in this repo.
The `package.json` under `mvdmio.Hotwire.NET/` is only for vendored Hotwire assets used during packaging.
Run npm only when updating those assets, and run it in `mvdmio.Hotwire.NET/`:

```sh
npm install
```

Normal .NET build and test workflows do not require manual npm commands.
If you update Hotwire JS package versions, inspect both `package.json` and the library packaging inputs: `node_modules/` is committed, and the csproj packs `stimulus.js` and `turbo.js` from it.
