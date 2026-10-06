# Coding Standards

Match the existing file before applying general preferences.

## C# Style

- Indent with 3 spaces; save `.cs` files as UTF-8 with BOM.
- Prefer file-scoped namespaces: `namespace Foo.Bar;`.
- Keep `using` directives explicit in the library; the test project enables implicit usings.
- Order `using` directives `System.*` first, then everything else alphabetically (`mvdmio.*` sorts before `Xunit`).
- Prefer `sealed` for small concrete leaf classes where the surrounding code does so.
- Put braces on their own lines; a single-statement `if`/`else`/loop body may omit braces (see `mvdmio.Hotwire.NET/ASP/Broadcasting/InMemoryTurboBroadcaster.cs`).

## Types And Nullability

- Avoid null-forgiving operators unless there is a strong reason.
- Use `var` for locals; write an explicit type only where the compiler needs a target type, such as a collection expression.

## Documentation And Public API

- Add or preserve `/// <summary>` comments for new public API in the package project.
- Use `<inheritdoc />` when implementing documented interface members.
- This matters because the library generates documentation files when built.
- `JetBrains.Annotations` is referenced and `[PublicAPI]` is used on public extension/action types; preserve that pattern when editing those surfaces.
- Do not change public API signatures casually.

## Error Handling

- Throw `InvalidOperationException` when required framework state is missing, for example when a referer or view engine is unavailable.
- Do not swallow exceptions inside core library logic.
- At middleware or app-boundary code, catch only expected operational exceptions, log them, and return an appropriate result.
- Use `TurboStreamsWebsocketMiddleware` as the local model for boundary error handling.

## Testing Conventions

- Test folders mirror production areas without the `ASP/` prefix (`ASP/TurboActions/` → `TurboActions/`).
- Name test classes after the target type or behavior plus `Tests`.
- Prefer descriptive test names; existing code contains both short names like `EncryptAndDecrypt` and behavior names like `IsTurboRequest_ShouldReturnTrue`.
- Match the style of the file you are editing, but prefer the more descriptive pattern in new test files.
- Use FluentAssertions for assertions.
- For multi-line HTML output assertions, prefer raw string literals for readability.
- Prefer simple stubs or real framework objects over heavy mocking; the repo already uses lightweight stubs under `mvdmio.Hotwire.NET.Tests/_Stubs/`.
- `_sut` is acceptable when a clear single subject exists.
