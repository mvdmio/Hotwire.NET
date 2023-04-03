# mvdmio.Tailwind.NET
This package makes it possible to use [TailwindCSS](https://tailwindcss.com) in your .NET project without any external dependency.

It uses the [Tailwind Standalone CLI](https://tailwindcss.com/blog/standalone-cli) to make this possible.

## Usage
After installation, the NuGet package will automatically build the tailwind input file on every build. It will minify the output file when in Release mode.

### Install the NuGet package
```
dotnet add package mvdmio.Tailwind.NET
```

This does the following:
1. Creates a `<root>/Tailwind` folder with the following files:
	1. `tailwind.config.js` with some default configuration.
	2. `tailwind.input.css` with the basics to get tailwind running.
2. Adds build targets to your `.csproj` that automatically build the TailwindCSS input file

### Tailwind Functionality
More information about Tailwind Functions & Directives: [https://tailwindcss.com/docs/functions-and-directives](https://tailwindcss.com/docs/functions-and-directives).

#### Layers
Tailwind recommends using layers like so:
```css
# /tailwind/tailwind.input.css
@import 'tailwindcss/base';
@import 'tailwindcss/components';
@import 'tailwindcss/utilities';

@layer base {
  # Your base module additions
}

@layer components {
  # Your components module additions
}

@layer utilities {
  # Your utilities module additions
}
```

#### Import
You can use `@import` to import different files like so:
```
# /tailwind/tailwind.input.css
@import 'tailwindcss/base';
@import 'tailwindcss/components';
@import 'tailwindcss/utilities';

@import './some-other-file.css'
```

```
# /tailwind/some-other-file.css

.select2-dropdown {
  @apply rounded-b-lg shadow-md;
}
.select2-search {
  @apply border border-gray-300 rounded;
}
.select2-results__group {
  @apply text-lg font-bold text-gray-900;
}
/* ... */
```

## Customization

### Customizing file locations
You can change the input and output file paths by adding the following properties in your `.csproj`:
```
<PropertyGroup>
  <TailwindConfigFile>path/to/your/tailwind.config.js</TailwindConfigFile>
  <TailwindInputFile>path/to/your/input/file.css</TailwindInputFile>
  <TailwindOutputFile>path/to/your/output/file.css</TailwindOutputFile>
</PropertyGroup>
```

The defaults for these properties are:
```
<PropertyGroup>
  <TailwindConfigFile>tailwind/tailwind.config.js</TailwindConfigFile>
  <TailwindInputFile>tailwind/tailwind.input.css</TailwindInputFile>
  <TailwindOutputFile>wwwroot/tailwind.output.css</TailwindOutputFile>
</PropertyGroup>
```

### Disable default files copy action
The library copies default files (`tailwind/tailwind.config.js` and `tailwind/tailwind.input.css`) to your project. To disable this, add the following property:
```
<PropertyGroup>
  <TailwindCopyDefaultFiles>false</TailwindCopyDefaultFiles>
</PropertyGroup>
```

### Tailwind Watch
Tailwind Watch does not work (currently) with this NuGet package. All builds must be manually triggered by running the build in Visual Studio or using the `dotnet build` command.

If you want to use Tailwind watch, install Tailwind using NPM and use the following command:
```
npx tailwind -c tailwind/tailwind.config.js -i tailwind/tailwind.input.css -o wwwroot/tailwind.output.css --watch
```

# Contact
For issues with the package, please create a new issue on GitHub. Pull requests are also welcome.

For anything else, please contact me at [michiel@mvdm.io](mailto:michiel@mvdm.io).