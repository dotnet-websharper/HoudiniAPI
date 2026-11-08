namespace WebSharper.Houdini.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Houdini

[<JavaScript>]
module Client =
    // The template is loaded from the DOM, so editing index.html and refreshing
    // the browser is enough; a rebuild is only needed when holes change.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let status = Var.Create "Click the button to register the paint worklet."
    let registered = Var.Create "No"

    let registerPaintWorklet () =
        promise {
            try
                let paintWorklet = JS.Window?CSS?paintWorklet |> As<Worklet>
                do! paintWorklet.AddModule("paint-worklet.js") |> Promise.AsAsync
                registered := "Yes"
                status := "Houdini paint worklet registered."
            with ex ->
                status := $"Error: {ex.Message}"
        }

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate()
            .registerWorklet(fun _ -> registerPaintWorklet () |> ignore)
            .status(status.V)
            .registered(registered.V)
            .Bind()
