namespace App
open Fable.Core
open Fuse
open Fable.Import
open Fable.Import.Fetch

module cardsmenu =
    [<StringEnum>]
    type State = Menu | SelectedOrUnder | AboveSelected

    [<StringEnum>]
    type StatusBarStyle = | [<CompiledName("Light")>] Light | [<CompiledName("Dark")>]Dark

    type Item =
        {name : string
         color : string
         textColor : string
         index : int
         statusBarStyle : StatusBarStyle
         offset : float * float
         state : Observable.IObservable<State>
         isSelected : Observable.IObservable<bool>
         mutable selectMe : unit -> unit }

    let currentStatusbarStyle = Observable.createWith(Dark)
    let pageSelected = Observable.createWith(false)
    let pages = Observable.createTyped<Item>()

    let private selectMe current =
        fun () ->
            pageSelected.value <- not pageSelected.value

            if pageSelected.value = false then
                currentStatusbarStyle.value <- StatusBarStyle.Dark

            if current.isSelected.value then
                pages.forEach (fun page -> 
                                page.state.value <- Menu
                                page.isSelected.value <- false)

            else
                pages.forEach
                    (fun page ->
                        page.state.value <- (if page.index >= current.index then SelectedOrUnder else AboveSelected )
                        if page.index = current.index then
                            page.isSelected.value <- true
                            currentStatusbarStyle.value <- current.statusBarStyle
                        else page.isSelected.value <- false )

    let createItem(name, color, textColor, statusBarStyle, index, nPages) =
        let offsetSize = 60
        let offY = float ((offsetSize*(nPages - 1)) - offsetSize * (index - 1))
        let offX = (offY / 4.)
        let item = {name=name
                    color=color
                    textColor = textColor
                    index=index
                    statusBarStyle=statusBarStyle
                    offset=(offX,offY)
                    state = Observable.createWith(Menu)
                    isSelected=Observable.createWith false
                    selectMe = fun () -> ()}
        item.selectMe <- selectMe item
        item

    pages.add(createItem("PROJECTS","#FFF199","#B0545E",StatusBarStyle.Dark,0,4))
    pages.add(createItem("TEAM","#FFB37B","#B24B5D",StatusBarStyle.Dark,1,4))
    pages.add(createItem("ABOUT","#B17179","#FAD96E",StatusBarStyle.Light,2,4))
    pages.add(createItem("CONTACT","#4A304D","#D4725E",StatusBarStyle.Light,3,4))
