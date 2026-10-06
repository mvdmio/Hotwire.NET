# AGENTS.md

- Style: telegraph. Drop filler/grammar. Min tokens (agent docs + replies).
- Read `CODING_STANDARDS.md` before writing or reviewing code, and its section before these tasks:
  - **Hotwire JS** upgrade → § Hotwire JS Assets
  - **Packaging** edits (csproj, `build/`) → § Packaging
  - **Example app** changes → § Example Apps
  - **Test subset** runs → § Testing

## Done When

- **Solution build** green; it also compiles the RazorPages example against the library. The example apps' Tailwind step may rewrite the committed `tailwind.output.css`: keep that diff only alongside example view changes, else `git restore` it. Error "being used by another process" → kill the locking process, rebuild.
- **Tests** green; CI runs none, so this run is the only test gate. Abort "You must install or update .NET to run this application … version '8.0.0'" (tests target `net8.0`) → rerun as `DOTNET_ROLL_FORWARD=Major dotnet test …`.
- New behaviour and bug fixes carry tests → `CODING_STANDARDS.md` § Testing.
- `Readme.md` current when public API or usage changed → `CODING_STANDARDS.md` § Publishing.
- `<Version>` bumped when the package contents changed → `CODING_STANDARDS.md` § Publishing.
