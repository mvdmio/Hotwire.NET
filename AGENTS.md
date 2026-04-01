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

## C# Style

Match the existing file before applying general preferences.
Prefer file-scoped namespaces: `namespace Foo.Bar;`.
Keep `using` directives explicit; do not assume implicit usings in the library.
Order `using` directives as `System.*`, then framework/third-party, then local namespaces.
Use PascalCase for public types, methods, properties, and enum members.
Use `_camelCase` for private fields.
Keep interfaces prefixed with `I`.
Keep extension containers named `*Extensions`.
Prefer `sealed` for small concrete leaf classes where the surrounding code does so.
Prefer `static` classes for pure extension/helper containers.
Use braces on new lines.
Keep whitespace and indentation consistent with the file you are editing; there is no repo-wide formatter config.

## Types And Nullability

Honor nullable reference types everywhere.
Use `?` when absence is valid.
Avoid null-forgiving operators unless there is a strong reason.
Prefer concrete, specific types over weak abstractions like `object`.
Use `var` when the type is obvious from the right-hand side; otherwise prefer explicit types for clarity.
Do not change public API signatures casually.

## Documentation And Public API

Public library APIs frequently carry XML docs.
Add or preserve `/// <summary>` comments for new public API in the package project.
Use `<inheritdoc />` when implementing documented interface members.
This matters because the library generates documentation files when built.
`JetBrains.Annotations` is referenced and `[PublicAPI]` is used on public extension/action types; preserve that pattern when editing those surfaces.

## Naming And Organization

Keep folder structure aligned with feature areas such as `ASP/Extensions`, `ASP/MVC`, `ASP/RazorPages`, `ASP/TurboActions`, and `ASP/Broadcasting`.
Tests mirror production areas and should continue to do so.
Prefer descriptive method names over abbreviations.
For tests, `_sut` is acceptable when a clear single subject exists.

## Error Handling

Validate inputs early.
Throw `ArgumentException` for invalid caller input when that matches the existing API.
Throw `InvalidOperationException` when required framework state is missing, for example when a referer or view engine is unavailable.
Do not swallow exceptions inside core library logic.
At middleware or app-boundary code, catch only expected operational exceptions, log them, and return an appropriate result.
Use `TurboStreamsWebsocketMiddleware` as the local model for boundary error handling.

## Testing Conventions

The repo uses xUnit and FluentAssertions.
Use `[Fact]` for single scenarios and `[Theory]` with `[InlineData]` for variations.
Name test classes after the target type or behavior plus `Tests`.
Prefer descriptive test names; existing code contains both short names like `EncryptAndDecrypt` and behavior names like `IsTurboRequest_ShouldReturnTrue`.
Match the style of the file you are editing, but prefer the more descriptive pattern in new test files.
Use FluentAssertions for assertions.
For multi-line HTML output assertions, prefer raw string literals for readability.
Prefer simple stubs or real framework objects over heavy mocking; the repo already uses lightweight stubs under `mvdmio.Hotwire.NET.Tests/_Stubs/`.

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
