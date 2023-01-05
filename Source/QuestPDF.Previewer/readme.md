Install nuget locally (in directory where nupkg file is located)

```
dotnet tool install --global --add-source . QuestPDF.Previewer --global
```

Run on default port

```
questpdf-previewer
```

Run on custom port

```
questpdf-previewer 12500
```

Remove nuget locally 

```
dotnet tool uninstall QuestPDF.Previewer --global
```