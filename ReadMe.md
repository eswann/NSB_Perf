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
* Make sure valid connection strings are present for sql, rabbit, and azure transport connection strings.
* To switch the transport to use, set the ServiceBusType to either rabbit, sql, azure, or msmq

## Fire up the solution.  
The following should be startup projects:
* Q2.Command.Harness
* Q2.Oao.Application.Command.Port
* Q2.Oao.Identity.Port
	