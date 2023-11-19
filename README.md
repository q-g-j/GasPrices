# OpenSpritpreise
Copyright 2023 Jann Emken

### Beschreibung
Eine kleine Work-in-progress Cross-Platform-App zum Auflisten von Tankstellen und Spritpreisen in der Nähe (auf Deutschland beschränkt).

Benötigt einen persönlichen API-Schlüssel von [Tankerkönig.de](http://tankerkoenig.de/). Dieser kann [hier](https://creativecommons.tankerkoenig.de/) kostenlos bezogen werden und muss anschließend einmalig in den App-Einstellungen angegeben werden.

Die Position kann wahlweise auf einer Karte ausgewählt werden. Tipp: in Android mit einem Doppel-Tap.<br/>
In der Android-Version kann der Standort-Dienst zum Ermitteln der aktuellen Position genutzt werden.

Sowohl bei der Kartenposition als auch nach der Nutzung des Standortdienstes erfolgt die Suche nach Koordinaten statt nach der angezeigten Adresse.

#### Unterstützte Betriebssysteme:
- Windows
- Linux (noch nicht getestet)
- MacOS (noch nicht getestet)
- Android
- ~~iOS~~ (entfernt, da ich es nicht testen kann)

#### Technische Details
Das Programm ist in C# mit dem AvaloniaUI Framework geschrieben und nutzt durchgehend das MVVM Pattern.<br/>
Zur Umsetzung des IoC (Inversion of Control) im Projekt wurde der Dependency-Injection-Service aus **Microsoft.Extensions.DependencyInjection** gewählt.<br/>
Die ViewModels machen Gebrauch vom Source-Code-Generator aus dem **CommunityToolkit.Mvvm** um die Definitionen der Properties für die XAML-Bindings zu vereinfachen.<br/>
Die Navigation zwischen den Views wird ermöglicht durch eine Kombination aus einem [ViewLocator](https://docs.avaloniaui.net/docs/next/concepts/view-locator) und einem NavigationService (nach diesem Vorbild: [NavigationMVVM](https://github.com/SingletonSean/wpf-tutorials/tree/master/NavigationMVVM) vom Youtuber SingletonSean).

