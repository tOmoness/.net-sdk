# .net-sdk

This codebase contains the **MixRadio .Net SDK**.

[![Build status](https://ci.appveyor.com/api/projects/status/cfdh2bc1cursyab9)](https://ci.appveyor.com/project/srgb/wp-api-client)

#Overview
The **MixRadio .Net SDK** (formally the Nokia Music SDK) lets you easily integrate your Windows Phone app with [MixRadio on Lumia phones](http://nokia.ly/musicapp), your Windows 8 app with [MixRadio on Windows 8](http://nokia.ly/musicappwin8) or get data into any [Portable class libraries (PCL)](http://msdn.microsoft.com/en-us/library/vstudio/gg597391(v=vs.110).aspx) environment - for example using [Xamarin](http://www.xamarin.com/) to target the [Nokia X device family](http://www.nokia.com/global/products/nokia-x/).

The SDK lets you perform searches, get charts and recommendations and user data such as play history within your app. You can link through to the MixRadio apps to give your users a full listening experience.

##Installation
You need two things to start using the SDK:

1. **The SDK itself**
 - Install the NokiaMusic package with [NuGet](https://nuget.org/packages/NokiaMusic) or the [Package Manager Visual Studio console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console): <br/>
 ![Package Manager](http://dev.mixrad.io/assets/nuget-package-install.png)
 - or download the latest source from [GitHub](http://nokia.ly/wpmusicapi).
2. **Credentials** Visit our [API Registration page](https://account.mixrad.io/developer) and request your credentials.

##Quick Start
Impatient to get going? Head over to the [Quick Start guide](http://developer.nokia.com/resources/library/Lumia/nokia-mixradio-api/quick-start.html).

##Source License
The **MixRadio C# SDK** source code is released under the 3-clause license ("New BSD License" or "Modified BSD License") - see <https://raw.github.com/mixradio/wp-api-client/master/LICENSE.txt>.

## Terms
Usage of the SDK is subject to the following terms: <http://dev.mixrad.io/terms.html>

##Releases

- 3.6.1 - Patch to Windows Phone 8.0 AuthenticateUserAsync to take a oauthRedirectUri to make sure the OAuth2 flow ends at the correct point.
- 3.6.0 - Added full user authentication methods for PCL clients:GetAuthenticationUri / GetAuthenticationTokenAsync / RefreshAuthenticationTokenAsync. Going forward, these will be the main methods for authentication working cross-platform with AuthenticateUserAsync / CompleteAuthenticateUserAsync / DeleteAuthenticationTokenAsync now marked as obsolete.
- 3.5.0 - Added user methods to PCL library to allow usage from Xamarin projects.
- 3.4.0 - Added user authentication methods to Windows Phone 8.1 library, converted Win8 test app to a Universal app and added Windows Phone 8.1 test app
- 3.3.0 - Added GetUserRecentMixes method, replaced SharpGIS.GZipWebClient with PCL HttpClient, added CancellationToken support, moved all app-to-app comms to new mixradio protocol for Windows Phone, added new PlayMeTask to launch PlayMe feature of MixRadio, added MusicBrainzId property for Artists, added Windows Phone 8.1 project, upgraded Windows 8 project to 8.1, added GetAllMixesAsync and GetMixAsync methods
- 3.2.0 - Added BPM property to Product object and SearchBpmAsync to find tracks by Beats Per Minute
- 3.1.5 - More web fallback improvements
- 3.1.4 - More web fallback improvements, added GetMixAsync to get details of a mix by id
- 3.1.3 - Added AppToAppPlayUri and WebPlayUri properties to Artist to enable playback of Artist mixes on the web, also enabling artist mixes on non-Lumia devices via the PlayMixTask. Merged in Win8 compiler directive change from https://github.com/mixradio/wp-api-client/pull/4
- 3.1.0 - Adding WebUri property to Artist, Product and Mix for web fallback on non-Lumia WP8 devices and linking. Other minor few bug fixes to sorting. Renamed test app to new brand.
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

With the high-level Launchers, you can perform the following using the new app-to-app APIs that MixRadio supports on Windows Phone 8 and Windows 8:

 - Launch the MixRadio app
 - Search for music
 - Show Artist details
 - Show Gigs nearby
 - Show available mixes
 - Show product details
 - Play a mix 

## Metadata Methods
Examples of the data you can get in your app:

 - Search for content
 - Get top artists
 - Get charts
 - Get new releases
 - Get a list of genres
 - Get albums and tracks by an artist
 - Get artist recommendations
 - Get a list of available mixes

## User Data API Methods
With version 3 and above, you can access user data (provided the user authorises your app!):

- Read the user's play history
- Read the user's top artists
- Read the user's recent mixes

## Documentation
The documentation for this component is at <http://dev.mixrad.io/doc/netsdk>

## Dependencies
We're proud to build on the shoulders of the following giants...

 - [JSON.Net](http://json.codeplex.com) for JSON parsing
 - [NUnit](http://nunit.org) for unit testing
 - [OpenCover](https://github.com/sawilde/opencover/) and [ReportGenerator](http://reportgenerator.codeplex.com) for unit test coverage
 - [StyleCop](http://stylecop.codeplex.com) for keeping the code in order
 
The projects make use of [NuGet](http://nuget.org) to install these components at build time.
 
## Tools required to develop
 - Visual Studio 2013 for Windows Phone 8 / Windows 8 development

## Contributing
If you want to contribute to the project, check out the [Issues](https://github.com/mixradio/.net-sdk/issues) tab.

You can:

 - Raise an issue
 - Suggest a feature for the application

Feeling like writing some code? Why not take the next step:

 - Fork the repository
 - Make the changes to the codebase
 - Send a pull request once you're happy with it

The team will then review the changes, discuss if anything needs to be addressed, and integrate your changes back into the application.

##Feedback
If you have a suggestion, you can tell us about it on our [MixRadio API UserVoice](https://mixradio.uservoice.com/forums/233741-api)

