# WebSharper Houdini API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Houdini API](https://developer.mozilla.org/en-US/docs/Web/Houdini), enabling developers to extend CSS and the rendering engine with custom worklets in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Houdini API.

2. **Sample Project**:
   - Demonstrates how to use the Houdini API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/HoudiniAPI/)

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.Houdini
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/Houdini.git
   cd Houdini
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.Houdini/WebSharper.Houdini.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.Houdini.Sample
   dotnet build
   dotnet run
   ```

## Example Usage

Below is an example of how to use the Houdini API in a WebSharper project:

```fsharp
namespace WebSharper.Houdini.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Houdini

[<JavaScript>]
module Client =

    // The templates are loaded from the DOM, so you can edit index.html
    // and refresh your browser without recompiling, unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let statusMessage = Var.Create "Click the button to register a custom Houdini paint worklet."

    let registerPaintWorklet () =
        promise {
            try
                let window = As<Window>(JS.Window)
                let cssPaintWorklet = window.CSS?paintWorklet

                // Register the worklet
                do! cssPaintWorklet.AddModule("houdini-paint.js") |> Promise.AsAsync

                statusMessage.Value <- "✅ Houdini Paint Worklet Registered!"
            with ex ->
                statusMessage.Value <- $"❌ Error: {ex.Message}"
        }

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate.Main()
            .RegisterHoudini(fun _ ->
                async {
                    do! registerPaintWorklet() |> Promise.AsAsync
                }
                |> Async.StartImmediate
            )
            .Status(statusMessage.V)
            .Doc()
        |> Doc.RunById "main"
```

## Important Considerations

- **Limited Browser Support**: Houdini is still an experimental technology and is not fully supported in all browsers. Check [MDN Houdini API](https://developer.mozilla.org/en-US/docs/Web/Houdini) for compatibility details.
- **Security Restrictions**: Some Houdini worklets require HTTPS to function correctly.
- **Performance Considerations**: Worklets run on separate threads, which can improve performance but may introduce complexity.
