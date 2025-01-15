# mvdmio.Hotwire.NET

This package makes it possible to use [Hotwire Turbo](https://turbo.hotwire.dev)
and [Hotwire Stimulus](https://stimulus.hotwire.dev) in your .NET project without any external dependency.

## Usage

After installation the NuGet package will automatically add the Turbo and Stimulus JS files to your project in
`wwwroot/js/lib`. See 'Configuration' below to change the default directory.

### Install the NuGet package

```
dotnet add package mvdmio.Hotwire.NET
```

### Add the Turbo and Stimulus scripts to your HTML

```
<script type="module" src="~/js/lib/turbo.js" asp-append-version="true"></script>
<script type="module "src="~/js/lib/stimulus.js" asp-append-version="true"></script>
```

Do not forget `type="module"` here, or you will get an error similar to
`Uncaught SyntaxError: export declarations may only appear at top level of a module`.

### Configuration

You can override the following properties in your project file (.csproj):

```
<PropertyGroup>
  /* True if the turbo.js file and stimulus.js files should be copied to your project on build. False otherwise. */
  <HotwireCopyDefaultFiles>true</HotwireCopyDefaultFiles>

  /* The base path that the build will copy the turbo.js and stimulus.js files to. */
  <HotwireCopyBaseDirectory>$(MSBuildProjectDirectory)\wwwroot\js\lib</HotwireCopyBaseDirectory>
</PropertyGroup>
```

## Turbo Streams
Turbo streams are supported for ASP.NET Core MVC and ASP.NET Core Razor Pages.

You can return the `TurboStreamActionResult` by using the `TurboStream(...)` extension methods on your `Controller` or `PageModel`.
The following actions are supported:
- Append and AppendMultiple
- Prepend and PrependMultiple
- Replace and ReplaceMultiple
- Update and UpdateMultiple
- Remove and RemoveMultiple
- Before and BeforeMultiple
- After and AfterMultiple
- Refresh

### Example
#### Using 'ExtendedController'
```csharp
public class MyController : ExtendedController
{
    public async Task<IActionResult> Index()
    {
        if (Request.IsTurboRequest())
        {
            var renderedPartial = await RenderView("_Content", DateTime.Now);
            return TurboStream(new ReplaceTurboAction("content", renderedPartial));
        }
        
        return Page();
    }
}
```

#### Using Controller extension methods
```csharp
public class MyController : Controller
{
    public async Task<IActionResult> Index()
    {
        if (Request.IsTurboRequest())
        {
            // 'this' keyword required here; otherwise the compiler can't call extension methods.
            var renderedPartial = await this.RenderView("_Content", DateTime.Now);
            return this.TurboStream(new ReplaceTurboAction("content", renderedPartial));
        }

        return Page();
    }
}
```

### Documentation
How to use: https://turbo.hotwired.dev/handbook/streams

Reference: https://turbo.hotwired.dev/reference/streams

## Turbo Broadcasting
Turbo broadcasting lets you send Turbo Streams to clients from the server. This is useful when you want to update clients
when a certain event occurs.

To start using broadcasting, make sure the following is added to your Program.cs:

```csharp
builder.Services.AddTurboBroadcasting();

app.UseTurboBroadcasting();
```

This makes the ITurboBroadcaster interface available in your application. You can inject this interface in your services or controllers.
The ITurboBroadcaster can be used to send Turbo Streams to clients.

To create a broadcasting channel, you can use the following code in your HTML:
```html
<turbo-broadcast-channel name="channel-name" />
```

# Contact

For issues with the package, please create a new issue on GitHub. Pull requests are also welcome.

For anything else, please contact me at [michiel@mvdm.io](mailto:michiel@mvdm.io).