
#r "../packages/FSharp.Data.2.3.3/lib/net40/FSharp.Data.dll"

open System
open FSharp.Data
open System.Collections.Generic

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

type Observation2 = {Id:int;ReadingTime:DateTime;Lat:float;Lon:float;Elev:float;Speed:float; LapNumber:int}

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




