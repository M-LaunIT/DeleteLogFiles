# Homepage-Text: Logdateien automatisch löschen mit DeleteLogFiles

## SEO-Titel

```text
Logdateien automatisch löschen mit DeleteLogFiles | Windows-Dienst
```

## Meta Description

```text
DeleteLogFiles löscht alte Logdateien automatisch nach definierbaren Regeln. Ideal für Windows-Server, IIS-Logs und regelmäßige Speicherbereinigung.
```

## Beitrag

# Logdateien automatisch löschen mit DeleteLogFiles

Auf Windows-Servern sammeln sich mit der Zeit viele Logdateien an: IIS-Logs, Anwendungslogs, Exportdateien oder temporäre Diagnoseprotokolle. Einzelne Dateien sind oft klein, aber über Wochen und Monate können daraus schnell viele Gigabyte werden. Wird nicht regelmäßig aufgeräumt, belegen alte Logs unnötig Speicherplatz und erschweren die Wartung.

DeleteLogFiles ist ein kleiner Windows-Dienst, der diese Aufgabe automatisiert. Der Dienst prüft definierte Verzeichnisse in festen Intervallen und entfernt Dateien nur dann, wenn sie zur konfigurierten Dateiendung passen und älter als die eingestellte Aufbewahrungszeit sind.

## Wofür eignet sich DeleteLogFiles?

DeleteLogFiles ist besonders praktisch für Systeme, auf denen regelmäßig technische Protokolldateien entstehen, zum Beispiel:

- IIS-Webserver
- Anwendungssysteme mit eigenen Logordnern
- Terminalserver
- Export- und Übergabeverzeichnisse
- temporäre Diagnose- oder Trace-Dateien

Die Konfiguration bleibt bewusst einfach: Verzeichnisse eintragen, Dateiendungen festlegen, Aufbewahrungsdauer wählen und den Dienst laufen lassen.

## Sicherer Start mit Dry-Run

Da DeleteLogFiles Dateien löschen kann, startet die neue Version standardmäßig im Dry-Run-Modus. In diesem Modus wird nur protokolliert, welche Dateien gelöscht würden. Erst wenn die Ausgabe geprüft wurde, kann die tatsächliche Löschung aktiviert werden.

Das reduziert das Risiko bei produktiven Servern deutlich und macht nachvollziehbar, welche Dateien von der Regel erfasst werden.

## Neuer Konfigurationsdialog

Neben der manuellen Bearbeitung der `appsettings.json` gibt es jetzt einen kleinen grafischen Konfigurationsdialog. Darüber lassen sich die wichtigsten Einstellungen bequem setzen:

- Prüfintervall
- Aufbewahrungsdauer
- Dry-Run-Modus
- Unterordner einbeziehen
- Verzeichnisse
- Dateiendungen
- Dienst-Log und Log-Level

## Eigenes Dienst-Log

Der Dienst schreibt zusätzlich ein kleines Text-Log. Darin stehen Start und Ende eines Prüflaufs, gefundene Dateien, gelöschte Dateien, übersprungene Einträge und die berechnete Speicherersparnis. So lässt sich auch später nachvollziehen, was der Dienst getan hat.

## Moderne technische Basis

DeleteLogFiles wurde auf .NET 8 aktualisiert und als moderner Worker Service umgesetzt. Die alte Konfiguration über mehrere Textdateien wurde durch eine zentrale `appsettings.json` ersetzt. Das macht Installation, Betrieb und spätere Anpassungen übersichtlicher.

## Quellcode

Der Quellcode steht auf GitHub zur Verfügung:

```text
https://github.com/M-LaunIT/DeleteLogFiles
```

Wenn Sie eine kleine Automatisierung für Ihre Serverumgebung benötigen oder DeleteLogFiles an Ihre Anforderungen anpassen lassen möchten, sprechen Sie mich gerne an.
