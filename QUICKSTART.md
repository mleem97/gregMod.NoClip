# Quickstart — gregMod.NoClip

> Press the configured key to toggle noclip movement. The mod uses Data Center's new Input System. Configuration is namespaced by the mod and persists through Mel

Repo: [https://github.com/mleem97/gregMod.NoClip](https://github.com/mleem97/gregMod.NoClip) · Version: `0.1.0` · Lizenz: Apache-2.0.

## 1. Klonen

```bash
git clone git@github.com:mleem97/gregMod.NoClip.git
cd gregMod.NoClip
```

## 2. Bauen / Starten

Je nach Tech-Stack **einen** Weg wählen:

```bash
# .NET
dotnet build -c Release
dotnet run --project src/

# Node / pnpm
pnpm install
pnpm build
pnpm start

# Python
python -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt
python -m <modul>
```

## 3. Testen

```bash
dotnet test            # .NET
pnpm test              # Node
pytest                 # Python
```

Details stehen in [README.md](README.md) und [docs/INDEX.md](docs/INDEX.md).
Bei Problemen: Issue anlegen ([Issues](https://github.com/mleem97/gregMod.NoClip/issues)) oder [CONTRIBUTING.md](CONTRIBUTING.md) lesen.
