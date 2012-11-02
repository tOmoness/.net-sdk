# wp-api-client

This codebase contains the **Nokia Music Windows Phone API**.

##Overview
The **Nokia Music Windows Phone API** lets you easily integrate your Windows Phone applicaton with [Nokia Music on Nokia Lumia phones](http://www.nokia.com/global/apps/nokia/music/). The API offers two levels of integration; the simplest and quickest to get going is to use the high-level Launcher Tasks, with more advanced integration available to perform searches and get recommendations within your app.

##License
The **Nokia Music Windows Phone API** is released under the 3-clause license ("New BSD License" or "Modified BSD License") - see <https://raw.github.com/nokia-entertainment/wp-api-client/master/LICENSE.txt>.

##Usage

 - Install with [NuGet](http://nuget.org) - search for **nokiamusic** or install with [Package Manager](http://docs.nuget.org/docs/start-here/using-the-package-manager-console): <br/>
 ![Package Manager](http://api.ent.nokia.com/assets/nuget.png)
 - Download the latest binaries from the [Downloads](https://github.com/nokia-entertainment/wp-api-client/downloads) section

## Launcher Tasks
The [launcher APIs in Windows Phone](http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff769550.aspx) allow an application to integrate with common operating system tasks such as taking a picture, finding an address or a contact, making a call, or saving a ring tone with very little effort. 

With the high-level Launchers, you can perform the following using the new app-to-app APIs that Nokia Music supports on Windows Phone 8:

- Launch the Nokia Music app
- Search for music
- Show Artist details
- Show Gigs nearby
- Show available mixes
- Show product details
- Play a mix 
 
## API Methods
With the lower-level API access, you get more control and can perform the following use cases within your app:

- Search for content
- Get top artists
- Get charts
- Get new releases
- Get a list of genres
- Get albums and tracks by an artist
- Get artist recommendations
- Get a list of available mixes

## Documentation
The documentation for this component is at <http://nokia.ly/wpmusicapihelp>

## Dependencies

We're proud to build on the shoulders of the following giants...

 - [JSON.Net](http://json.codeplex.com) for JSON parsing
 - [NUnit](http://nunit.org) for unit testing
 - [OpenCover](https://github.com/sawilde/opencover/) and [ReportGenerator](http://reportgenerator.codeplex.com) for unit test coverage
 - [StyleCop](http://stylecop.codeplex.com) for keeping the code in order
 
The projects make use of [NuGet](http://nuget.org) to install these components at build time.
 
## Tools required to develop

 - Visual Studio 2010 for Windows Phone 7 development
 - Visual Studio 2012 for Windows Phone 8 development

## Contributing

If you want to contribute to the project, check out the [Issues](https://github.com/nokia-entertainment/wp-api-client/issues) tab.

You can:

 - Raise an issue
 - Suggest a feature for the application

Feeling like writing some code? Why not take the next step:

 - Fork the repository
 - Make the changes to the codebase
 - Send a pull request once you're happy with it

The team will then review the changes, discuss if anything needs to be addressed, and integrate your changes back into the application.

