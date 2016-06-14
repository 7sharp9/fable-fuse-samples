namespace App
open Fable.Core
open Fuse
open Fable.Import
open Fable.Import.Fetch

module http-json =
    let data = Observable.create()

    promise {
        let! req = GlobalFetch.fetch (Url "http://az664292.vo.msecnd.net/files/ZjPdBhWNdPRMI4qK-colors.json")
        let! json = req.json ()
        do (data.value <- json) } |> ignore