# OpenSpritpreise
Copyright 2024 Jann Emken

### Description
A cross-platform app for listing nearby gas stations and fuel prices (limited to Germany).

Requires a personal API key from [Tankerkönig.de](http://tankerkoenig.de/). This can be obtained for free [here](https://creativecommons.tankerkoenig.de/) and must be provided once in the app settings.

The location can be optionally selected on a map.<br/>
In the Android and iOS versions, the location service can be used to determine the current position.

Both for map positioning and after using the location service, the search is done by coordinates instead of the displayed address.

### Supported Platforms

|Project Name|Description|
|-|-|
|OpenSpritpreise.Desktop|- Windows<br/>- Linux<br/>- MacOS|
|OpenSpritpreise.Android|Android devices|
|OpenSpritpreise.iOS|iPhone / iPad|
|OpenSpritpreise.Browser|Platform-independent WebAssembly app|

### Technical Details
Written in C# with .NET 8.0 and the GUI framework [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia).

The project template used is ```avalonia.xplat``` ([Link](https://github.com/AvaloniaUI/avalonia-dotnet-templates)). This allows executing the same program code (in this project: ```OpenSpritpreise/OpenSpritpreise.csproj```) on different platforms (see [Supported Platforms](https://github.com/q-g-j/OpenSpritpreise/edit/master/README.md#supported-platforms)). The browser version unfortunately still has some minor bugs: in mobile browsers, there may be issues with keyboard input.

The app is implemented using the MVVM pattern and utilizes the source code generators from [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/).

For navigation between views, a combination of Avalonia's standard [ViewLocator](https://docs.avaloniaui.net/docs/concepts/view-locator) and a navigation service was used.

Most class instances (including Views, ViewModels and services) are resolved using dependency injection. Microsoft's Dependency Injection Service is used for this purpose.

As a mapping service, I chose to use OpenStreetMap, as no API keys were required for integration. The integration is done through a control from the package [Mapsui.Avalonia](https://github.com/Mapsui/Mapsui).

The browser version initially encountered issues making WebAPI calls via C# code due to security limitations (["CORS"](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS?ref=rowanudell.com)-related) of certain browsers (at least Firefox). To bypass this restriction, I utilized JavaScript code to make those API requests. In .NET, this can be easily achieved by using the .NET [JS-Interop Service](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/import-export-interop?view=aspnetcore-8.0).
