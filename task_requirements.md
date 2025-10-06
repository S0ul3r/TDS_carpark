# .NET Take Home Task Instructions

Project Overview:
-----------------
- Your task is to create a simple car park management API. Please read this document fully and carefully to ensure you don’t miss any requirements.
- You should create and structure the project in the way you feel most appropriate.
- You have the freedom to come up with your own solution entirely, our only ask is that it must cater for the requirements and define the endpoints you will find in the table below.
- Whilst this is a take home task, and we hope you use it as an opportunity to showcase your abilities, we also understand that your time is valuable. Please don’t feel compelled to over engineer the solution or spend many hours on it, we suggest 2-3 hours.

Requirements:
--------------
- Your API should be able to handle the following scenarios:
- Allocating vehicles to the first available space
- Determine the number of available and full spaces
- Determine the parking charge on vehicle exit
- De-Allocate a space on vehicle exit
- Vehicles will be charged per minute they are parked
- The parking charges are:
    - Small Car - £0.10/minute
    - Medium Car - £0.20/minute
    - Large Car £0.40/minute
- Every 5 minutes an additional charge of £1 will be added


Technical Requirements:
-----------------------
- Should use either an in-memory database, MSSQL or Postgres. Whichever option you choose, you should provide the relevant scripts (or instructions) to scaffold and seed the database.
- Basic error handling should be implemented
- Some Unit tests should be included - we do not expect everything to be covered by a test for this take home assessment.
- The following are the API endpoints to be defined:


| Type | Route | Query Params | Body | Return | Notes |
| ---- | ----- | ------------ | ---- | ------ | ----- |
| POST | /parking | | {VehicleReg: string, VehicleType: string} | {VehicleReg: string, SpaceNumber: int, TimeIn: DateTime} | Parks a given vehicle in the first available space and returns the vehicle and its space number |
| GET | /parking | | | {AvailableSpaces: int, OccupiedSpaces: int} | Gets available and occupied number of spaces |
| POST | /parking/exit | | {VehicleReg: string} | {VehicleReg: string, VehicleCharge: double TimeIn: DateTime, TimeOut: DateTime} | Should free up this vehicles space and return its final charge from its parking time until now |

 
**NOTE: The endpoint names and types must match, as well as any named parameters/body variables and return variables. Blank cells indicate no value.**

Non Technical Requirements:
---------------------------
- You should provide a README with your solution it should:
- Include information about how to setup and run your solution locally
- Detail any assumptions you made
- You could include any questions you would’ve asked had you needed to

