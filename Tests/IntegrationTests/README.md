# QuestPDF Integration Tests

These projects verify the NuGet package consumption path. They intentionally reference QuestPDF through a package restored from `../../artifacts/nupkg`; no integration project may reference the QuestPDF source project.

Modern .NET tests must always follow this sequence:

1. `dotnet publish`
2. execute the published output
3. validate the generated PDF or HTTP response

This catches package layout, RID asset selection, publish output, native library loading, single-file, self-contained, and AOT issues that are invisible when tests use `ProjectReference`, `dotnet build`, or `dotnet run`.

Local example after packing QuestPDF into `artifacts/nupkg`:

```bash
zx Tests/IntegrationTests/scripts/run-published-test.mjs console net10.0 osx-arm64 0.0.0-local
```

Named zx/minimist flags are also supported for local runs, for example `--app`, `--framework`, `--runtime`, and `--package-version`.

The .NET Framework project is Windows-only and uses `dotnet build`, because .NET Framework does not have the same publish model. It still validates the package restore output and native DLL copy behavior.

```bash
zx Tests/IntegrationTests/scripts/run-netframework-test.mjs 0.0.0-local net481 x64
```
