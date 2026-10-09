# Coding Standards

Match the existing file before applying general preferences.

## C# Style

Prefer file-scoped namespaces: `namespace Foo.Bar;`.
Keep `using` directives explicit; do not assume implicit usings in the library.
Order `using` directives as `System.*`, then framework/third-party, then local namespaces.
Test files put `using Xunit;` last, after local namespaces; match that in tests.
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
Prefer simple stubs or real framework objects over heavy mocking.
Lightweight stubs live under `mvdmio.Hotwire.NET.Tests/_Stubs/`; helpers that own a real resource (a temp folder, a key ring) live under `mvdmio.Hotwire.NET.Tests/_Helpers/`.
