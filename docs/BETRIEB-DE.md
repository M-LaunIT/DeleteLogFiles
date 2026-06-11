# DeleteLogFiles betreiben

Diese Anleitung beschreibt den typischen Ablauf von Build, Konfiguration, Installation und sicherem Produktivbetrieb.

## 1. Build erstellen

```powershell
dotnet build .\DeleteLogFiles.sln -c Release
```

Für ein Zielverzeichnis veröffentlichen:

```powershell
dotnet publish .\DeleteLogFiles\DeleteLogFiles.csproj -c Release -r win-x64 --self-contained false -o .\publish
```

## 2. Konfiguration vorbereiten

Die zentrale Datei ist `appsettings.json`.

Wichtig für den ersten Start:

- `DryRun` auf `true` lassen
- nur konkrete Logverzeichnisse eintragen
- Dateiendungen eng begrenzen, zum Beispiel `.log`
- Dienst-Log aktiviert lassen

Beispiel:

```json
{
  "Cleanup": {
    "IntervalMinutes": 30,
    "DeleteAfterDays": 14,
    "IncludeSubdirectories": true,
    "DryRun": true,
    "Directories": [
      "C:\\inetpub\\logs\\LogFiles"
    ],
    "Extensions": [
      ".log"
    ]
  },
  "FileLogging": {
    "Enabled": true,
    "Path": "Logs\\DeleteLogFiles.log",
    "MinimumLevel": "Information"
  }
}
```

## 3. Konfigurationsdialog nutzen

Der Dialog kann direkt aus dem Quellcode gestartet werden:

```powershell
dotnet run --project .\DeleteLogFiles.Configurator\DeleteLogFiles.Configurator.csproj
```

Im Dialog können Intervall, Aufbewahrungsdauer, Dry-Run, Unterordner, Dienst-Log, Verzeichnisse und Dateiendungen bearbeitet werden.

## 4. Dienst installieren

PowerShell als Administrator öffnen:

```powershell
sc.exe create DeleteLogFiles binPath= "C:\Pfad\zur\publish\DeleteLogFiles.exe" start= auto
sc.exe start DeleteLogFiles
```

Der Dienst schreibt standardmäßig ein Log nach:

```text
Logs\DeleteLogFiles.log
```

Relative Logpfade beziehen sich auf das Verzeichnis der Dienstdatei.

## 5. Dry-Run prüfen

Nach dem ersten Start das Log öffnen und prüfen:

- Werden die richtigen Verzeichnisse geprüft?
- Passen die Dateiendungen?
- Werden nur Dateien gefunden, die wirklich gelöscht werden dürfen?
- Ist die berechnete Anzahl der Tage korrekt?

Erst nach dieser Prüfung sollte `DryRun` auf `false` gesetzt werden.

## 6. Produktivbetrieb

Empfehlungen für den laufenden Betrieb:

- Konfiguration nach jeder Änderung mit Dry-Run testen.
- Dienstkonto mit möglichst wenigen Rechten verwenden.
- Logdatei regelmäßig prüfen oder in bestehendes Monitoring aufnehmen.
- Aufbewahrungsdauer nicht zu knapp wählen, wenn Logs für Analyse oder Nachweise benötigt werden.

## 7. Deinstallation

```powershell
sc.exe stop DeleteLogFiles
sc.exe delete DeleteLogFiles
```
