namespace WebSharper.Houdini.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Houdini

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let statusMessage = Var.Create "Click the button to register a custom Houdini paint worklet."

    let registerPaintWorklet () =
        promise {
            try
                let window = As<Window>(JS.Window)
                let cssPaintWorklet = window?CSS?paintWorklet |> As<Worklet>

                // Register the worklet
                do! cssPaintWorklet.AddModule("paint-worklet.js")
                
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
