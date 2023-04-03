﻿# mvdmio.Hotwire.NET
This package makes it possible to use [Hotwire Turbo](https://turbo.hotwire.dev) and [Hotwire Stimulus](https://stimulus.hotwire.dev) in your .NET project without any external dependency.

## Usage
After installation, the NuGet package will automatically build the tailwind input file on every build. It will minify the output file when in Release mode.

### Install the NuGet package
```bash
dotnet add package mvdmio.Hotwire.NET
```

### Configuration
You can override the following properties in your project file (.csproj):
```xml
<PropertyGroup>
	<!-- The path to the turbo.js file in the NuGet package -->
	<HotwireTurboJsPath>lib\turbo.js</HotwireTurboJsPath>

	<!-- The path to the stimulus.js file in the NuGet package -->
	<HotwireStimulusJsPath>lib\stimulus.js</HotwireStimulusJsPath>

	<!-- True if the turbo.js file and stimulus.js files should be copied to your project on build. False otherwise.
	<HotwireCopyDefaultFiles>true</HotwireCopyDefaultFiles>

	<!-- The base path that the build will copy the turbo.js and stimulus.js files to.
	<HotwireCopyBaseDirectory>$(MSBuildProjectDirectory)\wwwroot\js\lib</HotwireCopyBaseDirectory>
</PropertyGroup>
```

# Contact
For issues with the package, please create a new issue on GitHub. Pull requests are also welcome.

For anything else, please contact me at [michiel@mvdm.io](mailto:michiel@mvdm.io).