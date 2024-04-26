module Server

open Saturn
open Giraffe
open Elmish.Bridge

open Chess.Server

let router =
    choose [
        RemotingApi.endpoint
        WebSocketApi.endpoint
    ]

let app = application {
    use_router router
    memory_cache
    use_static "public"
    use_gzip
    app_config Giraffe.useWebSockets
}

[<EntryPoint>]
let main _ =
    run app
    0