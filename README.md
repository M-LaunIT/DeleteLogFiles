# DeleteLogFiles

DeleteLogFiles ist ein kleiner Windows-Dienst, der alte Logdateien automatisch bereinigt. Das Tool prüft frei konfigurierbare Verzeichnisse in festen Intervallen und löscht Dateien erst dann, wenn Dateiendung und Aufbewahrungszeit zur Konfiguration passen.

Typische Einsatzbereiche sind IIS-Logs, Anwendungslogs, Exportverzeichnisse, temporäre Diagnoseprotokolle oder andere Ordner, in denen regelmäßig technische Logdateien entstehen. Ziel ist eine einfache, nachvollziehbare und sichere Speicherbereinigung auf Windows-Servern.

## Highlights

- Windows-Dienst auf Basis von .NET 8
- kleiner grafischer Konfigurationsdialog
- zentrale Konfiguration über `appsettings.json`
- Dry-Run-Modus für sichere Tests
- konfigurierbare Aufbewahrungsdauer
- Filterung nach erlaubten Dateiendungen
- optionale Bereinigung von Unterordnern
- eigenes Dienst-Log als Textdatei
- übersichtliche Laufstatistik mit geprüften, gelöschten und übersprungenen Dateien

## Projekte

| Projekt | Zweck |
| --- | --- |
| `DeleteLogFiles` | Der eigentliche Windows-Dienst. |
| `DeleteLogFiles.Configurator` | Kleiner WinForms-Dialog zur Bearbeitung der Konfiguration. |

## Schnellstart

Voraussetzungen:

- Windows
- .NET 8 SDK zum Bauen
- .NET 8 Runtime auf dem Zielsystem, wenn nicht self-contained veröffentlicht wird

Build:

```powershell
dotnet build .\DeleteLogFiles.sln -c Release
```

Konfigurationsdialog starten:

```powershell
dotnet run --project .\DeleteLogFiles.Configurator\DeleteLogFiles.Configurator.csproj
```

Eine ausführlichere Betriebsanleitung liegt unter `docs/BETRIEB-DE.md`.

Dienst veröffentlichen:

```powershell
dotnet publish .\DeleteLogFiles\DeleteLogFiles.csproj -c Release -r win-x64 --self-contained false -o .\publish
```

## Konfiguration

Die Konfiguration liegt in `DeleteLogFiles/appsettings.json`. Sie kann direkt im Editor oder über den Konfigurationsdialog bearbeitet werden.

```json
{
  "Cleanup": {
    "IntervalMinutes": 30,
    "DeleteAfterDays": 14,
    "IncludeSubdirectories": true,
    "DryRun": true,
    "Directories": [
      "C:\\inetpub\\logs\\LogFiles\\W3SVC1"
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

Wichtige Einstellungen:

| Einstellung | Beschreibung |
| --- | --- |
| `Cleanup:IntervalMinutes` | Abstand zwischen zwei Prüfläufen in Minuten. |
| `Cleanup:DeleteAfterDays` | Dateien werden erst gelöscht, wenn sie älter als diese Anzahl Tage sind. |
| `Cleanup:IncludeSubdirectories` | Unterordner werden ebenfalls geprüft. |
| `Cleanup:DryRun` | Testmodus: Dateien werden nur protokolliert, nicht gelöscht. |
| `Cleanup:Directories` | Zu prüfende Verzeichnisse. |
| `Cleanup:Extensions` | Erlaubte Dateiendungen, zum Beispiel `.log`. |
| `FileLogging:Enabled` | Aktiviert das zusätzliche Dienst-Log. |
| `FileLogging:Path` | Pfad zur Logdatei. Relative Pfade beziehen sich auf das Programmverzeichnis. |
| `FileLogging:MinimumLevel` | Mindeststufe für Logeinträge, zum Beispiel `Information`, `Warning` oder `Error`. |

## Konfigurationsdialog

Der Dialog in `DeleteLogFiles.Configurator` bietet eine einfache Oberfläche für die wichtigsten Einstellungen:

- Intervall und Aufbewahrungsdauer
- Dry-Run aktivieren oder deaktivieren
- Unterordner einbeziehen
- Dienst-Log aktivieren, Pfad setzen und Log-Level wählen
- Verzeichnisse hinzufügen oder entfernen
- Dateiendungen hinzufügen oder entfernen

Der Dialog enthält das Laun-IT-Logo und einen Link zur Projektseite.

## Dienst installieren

PowerShell als Administrator öffnen:

```powershell
sc.exe create DeleteLogFiles binPath= "C:\Pfad\zur\publish\DeleteLogFiles.exe" start= auto
sc.exe start DeleteLogFiles
```

Dienst stoppen und entfernen:

```powershell
sc.exe stop DeleteLogFiles
sc.exe delete DeleteLogFiles
```

## Sicherheit

DeleteLogFiles ist ein Löschdienst. Deshalb sollte jede neue Konfiguration zuerst mit `DryRun: true` getestet werden. Im Dry-Run protokolliert der Dienst, welche Dateien gelöscht würden, ohne tatsächlich Dateien zu entfernen.

Empfehlungen:

- Keine sehr breiten Pfade wie `C:\` konfigurieren.
- Nur notwendige Dateiendungen erlauben.
- Den Dienst mit einem Benutzerkonto betreiben, das nur die benötigten Rechte besitzt.
- Das Dienst-Log nach Konfigurationsänderungen prüfen.
- Vor dem Produktivbetrieb mindestens einen vollständigen Dry-Run ausführen.

## Migration von Version 0.1

Ältere Versionen nutzten `directories.txt`, `extensions.txt`, `App.config` und log4net. Die aktuelle Version verwendet nur noch `appsettings.json` und einen eingebauten kleinen File-Logger.

Für die Migration:

1. Bisherige Verzeichnisse aus `directories.txt` nach `Cleanup:Directories` übernehmen.
2. Bisherige Endungen aus `extensions.txt` nach `Cleanup:Extensions` übernehmen.
3. `DryRun` aktiviert lassen.
4. Dienst starten und Log prüfen.
5. Erst danach `DryRun` deaktivieren.
