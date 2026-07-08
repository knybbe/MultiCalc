# MultiCalc

A basic multiplatform calculator built with **Uno Platform**.

**Platforms**: macOS, Windows, Linux, Android, iOS, WebAssembly (browser).

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

## Getting Started (local)

### Prerequisites
- .NET 10 SDK
- For full mobile: appropriate workloads (`dotnet workload install android ios`)
- Uno Platform workloads are pulled via the SDK on restore/build

```bash
cd MultiCalc
dotnet restore
```

### Run / Build locally
```bash
# Desktop (recommended for quick dev on Mac)
dotnet build -f net10.0-desktop
dotnet run -f net10.0-desktop

# WebAssembly (serves in browser)
dotnet build -f net10.0-browserwasm
# Then serve the publish output, e.g.:
dotnet publish -f net10.0-browserwasm -c Release -o ../artifacts/wasm
# cd ../artifacts/wasm && npx http-server -p 8080   (or dotnet serve)
# Open http://localhost:8080
```

On CI or full dev machine use the complete target list in the csproj.

## Tests
```bash
cd ../MultiCalc.Tests   # from solution root or appropriate
dotnet test
```

All core arithmetic, edge cases, chaining, error states are covered.

## GitHub Actions
Pushes/PRs trigger:
- Unit tests (ubuntu)
- WASM publish artifact
- Linux desktop publish
- Windows publish
- Android publish
- macOS + iOS publish (macOS runner)

Artifacts are uploaded for inspection. For real store releases you will need to add signing secrets (certificates, provisioning profiles) — see notes in workflow.

## Development Notes
- UI implemented in `MainPage.xaml` + `MainPage.xaml.cs` using a Grid layout inspired by the official .NET MAUI calculator sample.
- All business logic lives in `CalculatorEngine.cs` (stateless from UI perspective) inside Core for testability and reuse.
- To target all 6 platforms restore the full `<TargetFrameworks>` list in `MultiCalc/MultiCalc/MultiCalc.csproj`.

## Name & License
MultiCalc — basic calculator example.

MIT-style (or as per your preference).

Enjoy calculating everywhere!
