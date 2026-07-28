# NoClip

> Toggleable free-flight movement for Data Center.

[![Discord](https://img.shields.io/discord/1392073682133848075?style=for-the-badge&logo=discord&logoColor=white&label=Discord)](https://discord.gg/greg)
[![License](https://img.shields.io/badge/License-Apache%202.0-green?style=for-the-badge)](./LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.0-orange?style=for-the-badge)]()
[![GameVersion](https://img.shields.io/badge/Game%20Version-1.1.0-yellow?style=for-the-badge)]()
[![Unity](https://img.shields.io/badge/Unity-6000.4.12f1-black?style=for-the-badge&logo=unity&logoColor=white)]()

## Overview

Press the configured key to toggle noclip movement. Configuration is namespaced by the mod and persists through MelonLoader preferences.

## Installation

Copy `NoClip.dll` to `Data Center/Mods/`.

## Build from Source

```bash
dotnet build NoClip.csproj -c Release -p:Platform=x64
```

## Project Structure

`src/` contains the mod source, `references/` contains current Data Center 1.1.0 / Unity 6000.4.12f1 assemblies, and `docs/` contains maintenance notes.

## License

Apache License 2.0. See [LICENSE](./LICENSE).

## Support

Join [discord.gg/greg](https://discord.gg/greg).
