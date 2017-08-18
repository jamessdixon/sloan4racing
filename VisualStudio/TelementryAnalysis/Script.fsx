
#r "../packages/FSharp.Data.2.3.3/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Charting.0.90.14/lib/net40/FSharp.Charting.dll"
#r "System.Windows.Forms.DataVisualization"

open System
open FSharp.Data
open FSharp.Charting
open System.Collections.Generic
open System.Windows.Forms.DataVisualization

[<Literal>]
let dataPath = @"..\Data\secondRun.csv"

type Context = CsvProvider<dataPath,HasHeaders=true,IgnoreErrors=true>
let data = Context.GetSample()
let rows = data.Rows
rows |> Seq.length

rows |> Seq.head
type observation = {Id:int;ReadingTime:DateTime;Lat:float;Lon:float;Elev:float;Speed:float}
let obs = 
    rows 
    |> Seq.mapi(fun idx r -> {Id=idx;
                                ReadingTime=r.DateTime;
                                Lat=float r.Lat;
                                Lon=float r.Lon;
                                Elev=float r.Elev;
                                Speed=float r.Speed})
obs |> Seq.length

//obs 
//|> Seq.map(fun r -> r.Lat, r.Lon)
//|> Chart.Line
//|> Chart.WithYAxis(Min= -78.676, Max= -78.675)
//|> Chart.Show
//
//obs 
//|> Seq.map(fun r -> (r.Lat * -1.0), r.Lon)
//|> Chart.Line
//|> Chart.WithYAxis(Min= -78.676, Max= -78.675)
//|> Chart.Show
//
obs 
|> Seq.map(fun r -> (r.Lat * -1.0), r.Lon)
|> Chart.Point
|> Chart.WithYAxis(Min= -78.676, Max= -78.675)
|> Chart.Show

type Observation2 = {Id:int;ReadingTime:DateTime;Lat:float;Lon:float;Elev:float;Speed:float; LapNumber:int}

let obs2 = List<Observation2>()
let mutable lapNumber = 0
for ob in obs do
    obs2.Add({Id=ob.Id;ReadingTime=ob.ReadingTime;Lat=ob.Lat;Lon=ob.Lon;Elev=ob.Elev;Speed=ob.Speed;LapNumber=lapNumber})
    if  (ob.Lon < -78.6758 && ob.Lat < 35.7007 && ob.Lat > 35.7005) then
        lapNumber <- lapNumber + 1
obs2 |> Seq.length

obs2
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.length)
|> Seq.iter(fun (ln,lgth) -> printfn "%i:%i" ln lgth)


let obs3 = List<Observation2>()
let mutable lapNumber2 = 0
let mutable priorLat = obs |> Seq.head |> fun o -> o.Lat
for ob in obs do
    obs3.Add({Id=ob.Id;ReadingTime=ob.ReadingTime;Lat=ob.Lat;Lon=ob.Lon;Elev=ob.Elev;Speed=ob.Speed;LapNumber=lapNumber2})
    if  (priorLat < 35.7005 && ob.Lat > 35.7005) then
        lapNumber2 <- lapNumber2 + 1
    priorLat <- ob.Lat

obs3 |> Seq.length

obs3
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.length)
|> Seq.iter(fun (ln,lgth) -> printfn "%i:%i" ln lgth)

let racingObs =
    obs3
    |> Seq.filter(fun o -> o.LapNumber > 0 && o.LapNumber < 12)

racingObs |> Seq.length

racingObs
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.length)
|> Seq.iter(fun (ln,lgth) -> printfn "%i:%i" ln lgth)

//Average Speed
racingObs
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun ob -> ob.Speed))
|> Seq.iter(fun (ln,s) -> printfn "%i:%f" ln s)


//Check time = clock is every second so time diff is number of seconds
let totalTimeForObservations (obs:Observation2 seq) =
    let firstValue = obs |> Seq.head |> fun o -> o.ReadingTime
    let lastValue = obs |> Seq.last |> fun o -> o.ReadingTime
    (lastValue - firstValue).TotalSeconds
    
racingObs
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln, totalTimeForObservations obs)
|> Seq.iter(fun (ln,s) -> printfn "%i:%f" ln s)

//Fastest Lap
racingObs
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun ob -> ob.Speed))
|> Seq.sortByDescending(fun (ln,s) -> s)
|> Seq.iter(fun (ln,s) -> printfn "%i:%f" ln s)

//
type Observation3 = {Id:int;Lat:float;Lon:float;Elev:float;
                    Speed:float; LapNumber:int; TrackLocation:int}

let obs4 = List<Observation3>()
let mutable lapNumber3 = 0
let mutable trackLocation = 0
let mutable priorLat2 = obs |> Seq.head |> fun o -> o.Lat
let mutable priorLon = obs |> Seq.head |> fun o -> o.Lon
for ob in obs do
    obs4.Add({Id=ob.Id;Lat=ob.Lat;Lon=ob.Lon;Elev=ob.Elev;
                Speed=ob.Speed;LapNumber=lapNumber3; TrackLocation=trackLocation})
    if  (priorLat2 < 35.7005 && ob.Lat > 35.7005) then
        lapNumber3 <- lapNumber3 + 1
    if (priorLat2 < 35.7003 && ob.Lat > 35.7003) then
        trackLocation <- 1
    if (priorLon < -78.6756 && ob.Lon > -78.6756) then
        trackLocation <- 2
    if (priorLat2 > 35.7003 && ob.Lat < 35.7003) then
        trackLocation <- 3
    if (priorLat2 > 35.7007 && ob.Lat < 35.7007) then
        trackLocation <- 4
    if (priorLon > -78.6756 && ob.Lon < -78.6756) then
        trackLocation <- 5
    if (priorLat2 < 35.7007 && ob.Lat > 35.7007) then
        trackLocation <- 6
    priorLat2 <- ob.Lat
    priorLon <- ob.Lon

obs4 |> Seq.length

obs4
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun o -> o.Speed))
|> Chart.Bar
|> Chart.Show

obs4
|> Seq.filter(fun o -> o.LapNumber > 1 && o.LapNumber < 11)
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun o -> o.Speed))
|> Chart.Bar
|> Chart.WithYAxis(Min= 15.0, Max= 25.0)
|> Chart.Show

obs4
|> Seq.filter(fun o -> o.LapNumber > 1 && o.LapNumber < 11)
|> Seq.groupBy(fun o -> o.TrackLocation)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun o -> o.Speed))
|> Chart.Bar
|> Chart.WithYAxis(Min= 20.0, Max= 25.0)
|> Chart.Show

racingObs
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun ob -> ob.Speed))
|> Seq.sortBy(fun (ln,s) -> s)
|> Seq.iter(fun (ln,s) -> printfn "%i:%f" ln s)

let getTrackLocationSpeed lapNumber =
    obs4
    |> Seq.filter(fun o -> o.LapNumber = lapNumber)
    |> Seq.groupBy(fun o -> o.TrackLocation)
    |> Seq.map(fun (ln,obs) -> ln,obs |> Seq.averageBy(fun o -> o.Speed))
    |> Seq.sortBy(fun (ts,sp) -> ts) 

Seq.zip (getTrackLocationSpeed 3) (getTrackLocationSpeed 5)
|> Seq.map(fun (t,f) -> fst t, snd t, snd f)
|> Seq.map(fun (ts,t,f) -> ts,f-t)
|> Chart.Bar
|> Chart.Show

let slow = 
    [3;4;8] 
    |> Seq.collect(fun ln -> getTrackLocationSpeed ln)
    |> Seq.groupBy(fun x -> fst x)
    |> Seq.map(fun (tl, obs) -> tl,obs |> Seq.averageBy(fun o -> snd o))

let fast =
    [5;7;10]
    |> Seq.collect(fun ln -> getTrackLocationSpeed ln)
    |> Seq.groupBy(fun x -> fst x)
    |> Seq.map(fun (tl, obs) -> tl,obs |> Seq.averageBy(fun o -> snd o))

Seq.zip slow fast
|> Seq.map(fun (t,f) -> fst t, snd t, snd f)
|> Seq.map(fun (ts,t,f) -> ts,f-t)
|> Chart.Bar
|> Chart.Show


//Map
#r "../packages/Microsoft.Maps.MapControl.WPF.1.0.0.3/lib/net40-Client/Microsoft.Maps.MapControl.WPF.dll"
#r "PresentationFramework.dll"
#r "PresentationCore.dll"
#r "WindowsBase.dll"
#r "System.Xaml.dll"

open System.IO
open System.Text
open System.Xaml
open System.Windows
open System.Windows.Data
open System.Windows.Media
open System.Windows.Shapes
open System.Windows.Markup
open System.Windows.Controls
open Microsoft.Maps.MapControl.WPF

let key = "AsA7FfvBJiHj6hzCc1Nb7ipihTsxfO09DvZWHDItqtNFUFBDQU3QE4PtgZlCZXuF"
let credentials = new ApplicationIdCredentialsProvider(key)

let window = new System.Windows.Window()
window.Height <- 800.0
window.Width <- 800.0
let grid = System.Windows.Controls.Grid()
window.Content <- grid

let map = new Microsoft.Maps.MapControl.WPF.Map()
map.Mode <- Microsoft.Maps.MapControl.WPF.AerialMode()

map.CredentialsProvider <- credentials
map.ZoomLevel <- 19.0
map.Center <- new Location(35.7004574, -78.6760326) 

let createPin lap lat lon =
    let brush =
        match lap with
        | 3 | 4 | 8 -> new SolidColorBrush(Colors.Red)
        | 5 | 7 | 10 -> new SolidColorBrush(Colors.Green)
        | _ -> new SolidColorBrush(Colors.Black)
     
    let pin = new Pushpin()
    let factory = new FrameworkElementFactory(typeof<Ellipse>)
    factory.SetValue(Ellipse.FillProperty,brush) |> ignore
    factory.SetValue(Ellipse.WidthProperty, 4.0) |> ignore
    factory.SetValue(Ellipse.HeightProperty, 4.0) |> ignore
    let template = new ControlTemplate();
    template.VisualTree <- factory
    pin.Template <- template

    let location = new Location()
    location.Latitude <- lat
    location.Longitude <- lon
    pin.Location <- location
    pin

obs4
|> Seq.filter(fun o -> o.LapNumber > 1 && o.LapNumber < 11)
|> Seq.map(fun o -> createPin o.LapNumber o.Lat o.Lon)
|> Seq.iter(fun pp -> map.Children.Add(pp) |> ignore)

grid.Children.Add(map)
window.Show()


