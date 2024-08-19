# SpotlightImageGrabber

A commandline application to scan the Spotlight Images directory and copy them to another folder in 'My Documents'. I am using the images 
for the 'Photos' screensaver.
The project was written using Visual Studio 2022 in C# .Net 8. The needed Nuget packages are available from the Nuget package manager in 
Visual Studio.  Visual Studio should auto update packages but you may need to refresh them manually using the package manager.

## Details ##
The package looks in the directory
C:\Users\<YourUsername>\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets
and copies them to 
C:\Users\<YourUsername>\Pictures\Spotlight

The filenames are not changed other than to add the appropriate file extension for the image type. So far all images are jpegs, but the application
will recognize and manage .gif, .png and .bmp images as well.

The application will keep logs and a record of the current assets in the following directory:
C:\Users\<YourUsername>\AppData\Local\SpotlightImageGrabber

I am using the files copied there in the Windows 'Photos' screensaver.

I copied the application to a folder in the Program Files directory:
C:\Program Files\SpotlightImageGrabber

and I have a Task Scheduler task setup to run it daily as long as I'm logged in. If you wish to use the application for a similar purpose you will 
need to do these steps manually as the application will not install itself or configure scheduled tasks.

### Requirements ###
- Spotlight must be active as your lockscreen background.
- Visual Studio 2022 (for .Net 8)
- Nuget Package SkiaSharp
- Nuget Package Newtonsoft.Json
