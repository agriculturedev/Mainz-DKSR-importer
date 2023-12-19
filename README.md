# General structure

The project is divided into 3 main parts:

## - Importer
The program that runs indefinitely,
periodically fetching data from DKSR, converting this to FrostApi format and adding or updating the Frost data.

the importers should implement the **abstract** `Importer` class.
each importer also passes 2 strings to the `Importer` class, one for the DataType and one for the DataStreamName.
currently these would be "Tree" & "HealthState" and "ParkingLot" & "Occupancy" respectively.
each importer will then start a timer defining how often it should run.

they first get the DKSR data and for each they will use the `GetFrostThingData()` method to see if a thing already exists.
if it does not, they will create it and then call the `Update()` method of the Abstract class to handle all the frost data updates.

## - FrostApi
communication with a Frost Server

this contains response models for the FrostApi endpoints as well as models used for creating new Frost Data.
each new Thing that will be importer should implement IThing so that it has the correct properties for the FrostApi.
and a mapper should be added in the mapper class in the Importer project. this mapper should map all the properties that you want to add to the thing

## - DKSRDomain
Response models from DKSR to use for mapping to FrostApi models


# DataFlow

when running the application, multiple importers will start simultaneously. all executing mostly the same code.
dataflow is as follows:

Importer -> import() -> get dksr data -> map to frost model -> add/update frost data

adding or updating frost data is done in a particular order
1. check if the thing already exists in frost
2. if not, add it
3. update the thing with new data
4. get datastream or create one if it does not exist
5. create Location if it does not exist
6. add observation to the datastream
