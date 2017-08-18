
data.path <- "C:\\Git\\sloan4racing\\R Studio\\sloan_gpsdata.csv"
gps.data <- read.csv(data.path)
df <- data.frame(gps.data)
df.working <- df[,c(2,6)]
df.working[,2] = as.factor(df.working[,2])
head(df.working)

#id	ReadingTime	Lat	Lon	Elev	Speed	LapNumber	TrackLocation
#7/13/2017 10:52:51 PM 22.944

ts.gps <- ts(df.working,frequency=16)
ts.gps
plot(ts.gps, ylab = 'lap gps')

ts.gps.decompose <- decompose(ts.gps)
plot(ts.gps.decompose)

seasonality <- ts.gps.decompose$seasonal
plot(seasonality)
acf(seasonality)
