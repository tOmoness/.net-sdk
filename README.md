# wp-api-client

This codebase contains the **Nokia MixRadio C# SDK**.

##Overview
The **Nokia MixRadio C# SDK** (formally the Nokia Music SDK) lets you easily integrate your Windows Phone with [Nokia MixRadio on Nokia Lumia phones](http://nokia.ly/musicapp)
or your Windows 8 applicaton with [Nokia MixRadio on Windows 8](http://nokia.ly/musicappwin8).
The API offers two levels of integration; the simplest and quickest to get going is to use the high-level Launcher Tasks, with more advanced integration available to perform searches and get recommendations within your app.

##License
The **Nokia MixRadio C# SDK** is released under the 3-clause license ("New BSD License" or "Modified BSD License") - see <https://raw.github.com/mixradio/wp-api-client/master/LICENSE.txt>.

##Usage

 - Install the NokiaMusic package with [NuGet](https://nuget.org/packages/NokiaMusic) - or install with [Package Manager](http://docs.nuget.org/docs/start-here/using-the-package-manager-console): <br/>
 ![Package Manager](http://api.ent.nokia.com/assets/nuget.png)
 
##Releases

- 3.2.0 - Added BPM property to Product object and SearchBpmAsync to find tracks by Beats Per Minute. Fixed exception throwing in CountryResolver.CheckAvailabilityAsync when there is no connection.
- 3.1.5 - More web fallback improvements
- 3.1.4 - More web fallback improvements, added GetMixAsync to get details of a mix by id
- 3.1.3 - Added AppToAppPlayUri and WebPlayUri properties to Artist to enable playback of Artist mixes on the web, also enabling artist mixes on non-Nokia devices via the PlayMixTask. Merged in Win8 compiler directive change from https://github.com/mixradio/wp-api-client/pull/4
- 3.1.0 - Adding WebUri property to Artist, Product and Mix for web fallback on non-Nokia WP8 devices and linking. Other minor few bug fixes to sorting. Renamed test app to new brand.
- 3.0.0 - Added PCL project, added user data APIs and OAuth2 support for Wp8/Win8, added sorting for Search and GetArtistProducts, dropped support for Windows Phone 7, removed SearchGenre method (replaced with genreId param in Search method), updated to latest Json.Net
- 2.4.0 - Added support for .Net 4 projects, added SearchGenre method, fixed PlayMixTask for artists with & in the name, enabled ShowProductTask / Product.Show for Win8 now Nokia Music 1.2 supports product views.
- 2.3.0 - Fix to ensure GZip enabled only for Nokia Music domains rather than all.
- 2.2.0 - NuGet package fixes for Windows 8 and Windows Phone 7.
- 2.1.0 - Added various artists flag, release date and label to the product object, removed source-based GZip in favour of SharpGIS.GZipWebClient for Windows Phone
- 2.0.0 - Added support for Nokia Music on Windows 8. The *Nokia.Music.Phone* namespace has changed to *Nokia.Music*, the async *MusicClientAsync* methods have merged into *MusicClient* correcting the naming convention used, we've added sample clip and genre chart functionality.
- 1.1.0 - A tidy-up release. We have removed optional paging parameters that had incorrectly been included on the GetMixes and GetProduct methods and removed some unneeded properties from the Location type.
- 1.0.9 - Added Name to Artist Origin object, added AppToAppUri properties to Artist, Mix and Product
- 1.0.8 - Added GetProduct and GetSimilarProducts
- 1.0.7 - Added Gzip support
- 1.0.6 - Fixed MusicSearchTask bug where searching artists with spaces did not work, added LocationConverter for working with Maps control, added exclusiveTag for exclusive mixes
- 1.0.5 - Added RequestTimeout property to client, fix for location search querystring formatting
- 1.0.4 - Added Thumb50 property for Artist, Mix and Product types; Added location-based search
- 1.0.3 - Added Search Suggestions for artists and products
- 1.0.2 - Initial Release


## Launcher Tasks
The [launcher APIs in Windows Phone](http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff769550.aspx) allow an application to integrate with common operating system tasks such as taking a picture, finding an address or a contact, making a call, or saving a ring tone with very little effort. 

With the high-level Launchers, you can perform the following using the new app-to-app APIs that Nokia MixRadio supports on Windows Phone 8 and Windows 8:

- Launch the Nokia MixRadio app
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

## New User Data API Methods
New in v3.x, you can access user data (provided the user authorises your app!):

- Read the user's play history
- Read the user's top artists

## Documentation
The documentation for this component is at <http://nokia.ly/wpmusicapidoc>

## Dependencies

We're proud to build on the shoulders of the following giants...

 - [JSON.Net](http://json.codeplex.com) for JSON parsing
 - [NUnit](http://nunit.org) for unit testing
 - [OpenCover](https://github.com/sawilde/opencover/) and [ReportGenerator](http://reportgenerator.codeplex.com) for unit test coverage
 - [StyleCop](http://stylecop.codeplex.com) for keeping the code in order
 - [SharpGIS.GZipWebClient](https://github.com/dotMorten/SharpGIS.GZipWebClient) for GZIP compression on Windows Phone
 
The projects make use of [NuGet](http://nuget.org) to install these components at build time.
 
## Tools required to develop

 - Visual Studio 2012 for Windows Phone 8 / Windows 8 development

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

