# AGENTS.md

## Scope

This file guides coding agents working in `D:\Projects\mvdmio\mvdmio.Hotwire.NET`.
Base decisions on the current repo contents, not generic .NET defaults.

## General instructions

- Ask questions if you need clarification.
- Search early; quote exact errors; prefer newer sources.
- Style: telegraph. Drop filler/grammar. Min tokens (global AGENTS + replies).
- Keep files shorter than ~500 LOC; split/refactor as needed. Does not apply to test files.
- Always add tests when adding functionality.
- Always create or modify tests when fixing a bug.
- Always build the solution and run the tests after making changes. Fix all build errors and test failures before finishing your work.
- If the build fails because some process is running and locking the file, kill the process.
- Always update the README.md file so that it reflects the latest state of the project.
- Always bump the version number in the `mvdmio.Hotwire.NET` .csproj file when making changes that affect the projects. Follow semantic versioning principles (MAJOR.MINOR.PATCH).

## Rule Files

No `.cursorrules` file exists.
No `.cursor/rules/` directory exists.
No `.github/copilot-instructions.md` exists.
This file is therefore the primary agent instruction source for the repo.

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

```powershell
dotnet restore .\mvdmio.Hotwire.NET.sln
```

Build solution:

```powershell
dotnet build .\mvdmio.Hotwire.NET.sln
```

Build library only:

```powershell
dotnet build .\mvdmio.Hotwire.NET\mvdmio.Hotwire.NET.csproj
```

Build tests only:

```powershell
dotnet build .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj
```

Notes:
Building the solution also builds both example apps.
Example app builds trigger Tailwind work through `mvdmio.Tailwind.NET`.
Building the library creates a `.nupkg` in `mvdmio.Hotwire.NET/bin/...`.

## Test Commands

Run all tests:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj
```

Run one test class:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj --filter "FullyQualifiedName~mvdmio.Hotwire.NET.Tests.Extensions.RequestExtensions.IsTurboRequestTests"
```

Run one test method:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj --filter "FullyQualifiedName~mvdmio.Hotwire.NET.Tests.TurboActions.ReplaceTurboActionTests.WithRequiredParameters"
```

Verified single-method filter example from this repo:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj --filter "FullyQualifiedName~mvdmio.Hotwire.NET.Tests.Extensions.RequestExtensions.IsTurboRequestTests.IsTurboRequest_ShouldReturnTrue"
```

Use `--no-build` after a successful build:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj --no-build
```

Collect coverage if needed:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj --collect:"XPlat Code Coverage"
```

Important xUnit note: filtering a `[Theory]` method by method name runs every inline-data row for that method.

Machine has only a newer .NET runtime (no net8.0 runtime): prefix `dotnet test` with `DOTNET_ROLL_FORWARD=Major` so the net8.0 test host starts.

## Lint / Format

No dedicated lint command is checked in.
Use `dotnet build` for compile-time validation and `dotnet test` for behavioral validation.
No repo-defined `dotnet format` configuration exists.
Do not introduce new linting or formatting tooling unless the user asks.
If you use a formatter locally, keep the diff minimal and match surrounding style.

## npm / JavaScript

There are no npm scripts in this repo.
The `package.json` under `mvdmio.Hotwire.NET/` is only for vendored Hotwire assets used during packaging.
Run npm only when updating those assets, and run it in `mvdmio.Hotwire.NET/`:

```powershell
npm install
```

Normal .NET build and test workflows do not require manual npm commands.

## Coding Standards

Read `.agents/refs/coding-standards.md` before writing or reviewing code or tests: C# style, nullability, XML docs, naming, error handling, test conventions.

## Agent Guidance

Keep changes narrowly scoped.
Do not add repo infrastructure such as `.editorconfig`, analyzer packages, or formatter config unless requested.
Do not change target frameworks, packaging behavior, or MSBuild targets unless the task requires it.
If you touch example apps, remember that solution builds may regenerate Tailwind output.
If you touch public package APIs, update docs and tests together.
If you update Hotwire JS package versions, inspect both `package.json` and the library packaging inputs.
Run the smallest relevant validation command before finishing.

Recommended validation defaults:

```powershell
dotnet test .\mvdmio.Hotwire.NET.Tests\mvdmio.Hotwire.NET.Tests.csproj
```

For packaging or integration changes:

```powershell
dotnet build .\mvdmio.Hotwire.NET.sln
```
