# AGENTS.md — Notes for AI agents (gregMod.NoClip)

Repo: https://github.com/mleem97/gregMod.NoClip · License: Apache-2.0 · Version: see `VERSION` (1.0.2).

**Experimental mod** (`.experimental-mods/`, excluded from central
`build.sh` build/deploy). MelonMod for Data Center (`NoClipMod`). Free-camera
/ noclip movement.

## Duties

1. **Read first:** `README.md`, `QUICKSTART.md`, `docs/INDEX.md` — only then make changes.
2. **Do not commit secrets** (keys, tokens, `.env`). Use keys only via environment variables.
3. **Preserve history:** no `push --force`, no history rewrite without instruction.
4. **Verify changes:** before reporting done, build and test whatever the repo
   provides (`QUICKSTART.md`, `scripts/`, `tests/` — `dotnet build gregMod.NoClip.csproj -c Release`).
5. **Keep docs in sync:** for new features update `README.md` + `docs/` + `CHANGELOG.md` (Unreleased).
6. **Conventions:** Conventional Commits (`feat:`, `fix:`, `docs:`, `chore:` …), one logical change per commit.
7. **When unsure:** stop and ask instead of guessing — especially for deletes, migrations, CI.

## Build and references

- Target: `net6.0`, x64. Game: Data Center.
- `references/` holds symlinks into the Steam install. Never commit
  `references/*.dll`, `bin/`, or `obj/`.
- Not covered by `ModRepositories/build.sh`; build directly in this folder.

## Hard rules

- Always snapshot the camera/player transform on enable and restore on
  disable, mod unload, and scene change — never trap the player in noclip.
- Movement must not touch UI-map bindings or disable all input actions
  (same discipline as gregMod.KeyChaos scope rules).
- **Never** touch gregCore types outside a soft-probe/JIT-split bridge.
- Security: `SECURITY.md` applies.

## Layout

- `src/NoClipMod.cs` — MelonMod entry, movement, toggle.
- `scripts/`, `tests/`, `examples/`, `docs/` — tooling and docs.
