# AGENTS.md

## Scope

This file guides coding agents working in `D:\Projects\mvdmio\mvdmio.Hotwire.NET`.
Base decisions on the current repo contents, not generic .NET defaults.

## Shared rules

- Work on the repo's default branch unless a skill or the user names a branch. This overrides any default that creates branches.
- Preserve the worktree: parallel sessions share each checkout. Inspect existing changes before editing and leave them intact. When the index already holds another session's staged changes, commit with `git commit -- <your paths>`; for a file another session also edited, stage only your hunks (`git apply --cached` of your filtered diff) and run plain `git commit`.
- Search early. Quote exact errors.
- Ask when blocked, or when the design has more than one reasonable path.
- Telegraph style in replies and agent docs: drop filler and grammar.
- The main session orchestrates: unless the task is trivial, delegate exploring, implementing, testing and reviewing to subagents, at a model and effort that fit the task.
- [judgement] Prefer small diffs over broad refactors. Keep files under ~500 lines; tests may exceed it. Add an abstraction only once a second use exists. Ship no backward-compat shim unless real consumers or persisted data need one.
- Every feature and every bug fix comes with a test; a bug fix's test fails without the fix.
- Build, then test, one at a time: parallel builds deadlock on file locks. A build that fails on a file another process holds: stop that process, retry.

## General instructions

- Always build the solution and run the tests after making changes. Fix all build errors and test failures before finishing your work.
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

The library targets `net10.0` and `net11.0`.
The tests target `net11.0`.
The example apps target `net11.0`.
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

Machine has only a newer .NET runtime: prefix `dotnet test` with `DOTNET_ROLL_FORWARD=Major` if the test host fails to start.

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
