## Prerequisites
* Solution has been created in VS 2015, not sure if it will work in 2013
* The included Nuget.config has the locations of the necessary Nuget dependencies.
* Solution is using .Net VNext: Must have dnx 1.0.0-Beta7 installed (this was created with x64 version).

## Setup
* Azure - Make sure the azure storage emulator is installed on your local machine.
* Rabbit - Make sure you have a local or accessible RabbitMQ installation.
* Sql - Make sure you have a local or accessible SQL Server installation.
* Msmq - Make sure msmq is installed.

## To switch between transports
* Open solution in VS 2015. 
* Open the solution explorer tree in VS 2015
* Inside of the "Solution Items" folder, open the Config.Bus.json
* Make sure valid connection strings are present for sql, rabbit, azure, and msmq transport connection strings.
* To switch the transport to use, set the ServiceBusType to either rabbit, sql, azure, or msmq

## Fire up the solution.  
The following should be startup projects:
* Q2.Command.Harness (Console app to kick off commands)
* Q2.Oao.Application.Command.Port (Processes Application commands)
* Q2.Oao.Identity.Port (Processes User/Identity commands)

## Scenario
* Client (Q2.Command.Harness) makes request/response with port.  
* Port starts saga with two commands (StartApplicationSaga).
* Commands are processed in parallel.
	* StartApplication (Handled by Q2.Oao.Application.Command.Port)
	* CreateUser (Handled by Q2.Oao.Identity.Port)
	* Logic of each handler has been removed and replaced with a Task.Delay of 100 ms.
* Saga returns response to caller.  

## Observed locally for full request/response cycle:
Rabbit : < 200ms
MSMQ : 300-350 ms
SQL : 1500 - 3500 ms
Azure Queues : 4500 - 6000 ms
	