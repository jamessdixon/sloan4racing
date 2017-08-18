
#r "../packages/FSharp.Data.2.3.3/lib/net40/FSharp.Data.dll"

open System
open System.IO
open FSharp.Data
open System.Collections.Generic

[<Literal>]
let dataPath = @"..\Data\secondRun.csv"

type Context = CsvProvider<dataPath,HasHeaders=true,IgnoreErrors=true>
let data = Context.GetSample()
let rows = data.Rows
rows |> Seq.length

type Observation = {Id:int;ReadingTime:DateTime;Lat:float;Lon:float;Elev:float;
                    Speed:float;LapNumber:int;TrackLocation:int}

let obs = List<Observation>()
let mutable index = 0
let mutable lapNumber = 0
let mutable trackLocation = 0
let mutable priorLat = rows |> Seq.head |> fun o -> float o.Lat
let mutable priorLon = rows |> Seq.head |> fun o -> float o.Lon
for row in rows do
    obs.Add({Id=index; ReadingTime= row.DateTime;
            Lat=float row.Lat;Lon=float row.Lon;
            Elev=float row.Elev; Speed=float row.Speed;
            LapNumber=lapNumber; TrackLocation=trackLocation})
    if  (priorLat < 35.7005 && float row.Lat > 35.7005) then
        lapNumber <- lapNumber + 1
    if (priorLat < 35.7003 && float row.Lat > 35.7003) then
        trackLocation <- 1
    if (priorLon < -78.6756 && float row.Lon > -78.6756) then
        trackLocation <- 2
    if (priorLat > 35.7003 && float row.Lat < 35.7003) then
        trackLocation <- 3
    if (priorLat > 35.7007 && float row.Lat < 35.7007) then
        trackLocation <- 4
    if (priorLon > -78.6756 && float row.Lon < -78.6756) then
        trackLocation <- 5
    if (priorLat < 35.7007 && float row.Lat > 35.7007) then
        trackLocation <- 6
    priorLat <- float row.Lat
    priorLon <- float row.Lon
    index <- index + 1

obs |> Seq.length

let obs'=
    obs
    |> Seq.filter(fun o -> o.LapNumber > 1 && o.LapNumber < 11)

let header = "id,ReadingTime,Lat,Lon,Elev,Speed,LapNumber,TrackLocation"
let output =
    [|
        yield header
        for o in obs' ->
        sprintf "%i,%s,%f,%f,%f,%f,%i,%i" (o.Id) (o.ReadingTime.ToString()) (o.Lat) (o.Lon) (o.Elev) (o.Speed) (o.LapNumber) (o.TrackLocation)    
    |]

let desktop =
    Environment.SpecialFolder.Desktop
    |> Environment.GetFolderPath

let targetFolder = Path.Combine(desktop,"Sloan4Racing")
let fileName = "sloan_gpsdata.csv"
let filePath = Path.Combine(targetFolder,fileName)
File.WriteAllLines(filePath,output)





