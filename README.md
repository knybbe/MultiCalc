# MultiCalc

A basic multiplatform calculator built with **Uno Platform**.

**Platforms**: macOS, Windows, Linux, Android, iOS, WebAssembly (browser).

**🌐 Live Demo (WebAssembly)**: [https://knybbe.github.io/MultiCalc/](https://knybbe.github.io/MultiCalc/)

## Features (basic operations)
- Addition, subtraction, multiplication, division
- Decimal point, percent, sign toggle (+/-)
- Backspace, clear (C)
- Chained operations and equals
- Graceful error display (e.g. divide by zero)

## Project Structure
- `MultiCalc/` — Main Uno Platform single-project app (XAML UI + integration)
- `MultiCalc.Core/` — Pure .NET class library containing `CalculatorEngine` (no UI)
- `MultiCalc.Tests/` — xUnit tests for the engine (run on any .NET)
- `.github/workflows/ci.yml` — GitHub Actions for tests + cross-platform builds
- `.github/workflows/deploy-wasm.yml` — Deploys the WebAssembly build to GitHub Pages on main pushes

## Getting Started (local)

### Prerequisites
- .NET 10 SDK
- For full mobile: appropriate workloads (`dotnet workload install android ios`)
- Uno Platform workloads are pulled via the SDK on restore/build

```bash
cd MultiCalc
# Note: plain `dotnet restore` or build will require workloads for *all* declared platforms in the app.
# Use targeted -p: override (see below) for dev machines with partial workloads.
```

### Run / Build locally
```bash
# Desktop (recommended for quick dev on Mac, Linux, Windows)
dotnet build -f net10.0-desktop -p:TargetFrameworks=net10.0-desktop
dotnet run -f net10.0-desktop -p:TargetFrameworks=net10.0-desktop

# WebAssembly (serves in browser; requires `dotnet workload install wasm-tools`)
dotnet build -f net10.0-browserwasm -p:TargetFrameworks=net10.0-browserwasm
# Then serve the publish output, e.g.:
dotnet publish -f net10.0-browserwasm -p:TargetFrameworks=net10.0-browserwasm -c Release -o ../artifacts/wasm
# cd ../artifacts/wasm && npx http-server -p 8080   (or dotnet serve)
# Open http://localhost:8080
```

Core is declared as net10.0 (using Uno.Sdk to support TFM overrides in CI); per-platform builds of Core happen via -p:TargetFrameworks overrides in CI for proper Android etc. assets. For local tests just `dotnet test` (in Tests dir) works with no extra workloads.

## Tests
Tests target plain net10.0 and can run anywhere .NET 10 is installed (no mobile workloads needed).

```bash
cd MultiCalc.Tests
dotnet test
```

All core arithmetic, edge cases, chaining, error states are covered. (19 tests)

## GitHub Actions
Pushes/PRs trigger:
- Unit tests (ubuntu)
- WASM publish artifact
- Linux desktop publish
- Windows publish
- Android publish
- macOS + iOS publish (macOS runner)

On push to `main`, the WASM build is automatically deployed to GitHub Pages.

Artifacts are uploaded for inspection. For real store releases you will need to add signing secrets (certificates, provisioning profiles) — see notes in workflow.

## Development Notes
- UI implemented in `MainPage.xaml` + `MainPage.xaml.cs` using a Grid layout inspired by the official .NET MAUI calculator sample.
- All business logic lives in `CalculatorEngine.cs` (stateless from UI perspective) inside Core for testability and reuse.
- To target all 6 platforms restore the full `<TargetFrameworks>` list in `MultiCalc/MultiCalc/MultiCalc.csproj`.

## Name & License
MultiCalc — basic calculator example.

MIT-style (or as per your preference).

Enjoy calculating everywhere!
