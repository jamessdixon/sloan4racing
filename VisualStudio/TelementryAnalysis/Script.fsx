
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

let chartData = obs |> Seq.map(fun r -> r.Lat, r.Lon)
let chart = Chart.Line(chartData).WithYAxis(Min= -78.676, Max= -78.675)
chart.ShowChart()

let chartData' = obs |> Seq.map(fun r -> (r.Lat * -1.0), r.Lon)
let chart' = Chart.Line(chartData').WithYAxis(Min= -78.676, Max= -78.675)
chart'.ShowChart()

let chartData'' = obs |> Seq.map(fun r -> (r.Lat * -1.0), r.Lon)
let chart'' = Chart.Point(chartData'').WithYAxis(Min= -78.676, Max= -78.675)
chart''.ShowChart()

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
    if  (priorLat < 35.7005 && ob.Lat > 35.7005) then
        lapNumber3 <- lapNumber3 + 1
    if (priorLat < 35.7003 && ob.Lat > 35.7003) then
        trackLocation <- 1
    if (priorLon < -78.6756 && ob.Lon > -78.6756) then
        trackLocation <- 2
    if (priorLat > 35.7003 && ob.Lat < 35.7003) then
        trackLocation <- 3
    if (priorLat > 35.7007 && ob.Lat < 35.7007) then
        trackLocation <- 4
    if (priorLon > -78.6756 && ob.Lon < -78.6756) then
        trackLocation <- 5
    if (priorLat < 35.7007 && ob.Lat > 35.7007) then
        trackLocation <- 6
    priorLat <- ob.Lat
    priorLon <- ob.Lon

obs4 |> Seq.length

obs4
|> Seq.filter(fun o -> o.LapNumber > 0 && o.LapNumber < 12)
|> Seq.groupBy(fun o -> o.LapNumber)
|> Seq.map(fun (lp,obs) -> lp,obs |> Seq.groupBy(fun o -> o.TrackLocation))
|> Seq.map(fun (lp,obs) -> lp,obs |> Seq.map(fun (ts,obs2) -> ts, obs2))
|> Seq.map(fun (lp,obs) -> lp,obs |> Seq.map(fun (ts,obs2) -> ts, obs2 |> Seq.averageBy(fun o -> o.Speed))) 
|> Seq.map(fun (lp,obs) -> lp,obs |> Seq.sortBy(fun (x,y) -> x))
|> Seq.collect(fun (lp,obs) -> obs |> Seq.map(fun (x,y) -> lp,x,y))
|> Seq.iter(fun (lp,tl,s) -> printfn "%i,%i,%f" lp tl s)

//Chart Redux
let chartData5 =
    obs4
    |> Seq.filter(fun o -> o.LapNumber > 0 && o.LapNumber < 12)
    |> Seq.groupBy(fun o -> o.LapNumber)
    |> Seq.map(fun (lp,obs) -> lp,obs |> Seq.groupBy(fun o -> o.TrackLocation))
    |> Seq.map(fun (lp,obs) -> lp,obs |> Seq.map(fun (ts,obs2) -> ts, obs2))
    |> Seq.map(fun (lp,obs) -> lp,obs |> Seq.map(fun (ts,obs2) -> ts, obs2 |> Seq.averageBy(fun o -> o.Speed))) 
    |> Seq.map(fun (lp,obs) -> lp,obs |> Seq.sortBy(fun (x,y) -> x))
    |> Seq.collect(fun (lp,obs) -> obs |> Seq.map(fun (x,y) -> lp,x,y))
    |> Seq.map(fun (lp,ts,s) -> ts,s)

let chart5 = Chart.Bar(chartData5)
chart.ShowChart()




