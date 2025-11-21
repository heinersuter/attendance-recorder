# Attendance Recorder

This application records the time a user is logged in to a computer.
If the session is locked, time recording is stopped.

## Time Recording

A LifeSign is written every 30 seconds.
All LifeSigns within the same minute are combined to an active interval.

A user can override the intervals by defining additional active intervals.
Or inactive intervals.

## Build the app

### Prerequisites

* .Net SDK as defined in [Directory.Build.props](Directory.Build.props)
* Nude.js in a current version

## Run the app

1. Configure the file location in `appsettings.json` of ConsoleApp.
2. Build the client with `npm run build` (output is `wwwroot` of ConsoleApp).
3. Publish project ConsoleApp.
4. Run the ConsoleApp from publish folder (shell script can be used).

## Application Parts

### ConsoleApp

Main entry point of the application.
Hosts the static web pages and the WebApi.

### LifeSign

Every 30 seconds (configurable), a timestamp is written to a file.
Session lock and session unlock events are detected.

### WebApi

API to display information in the frontend. 
And to store overrides (merges).

### FileSystemStorage

Reading and writing files.

### attendance-recorder-react

Frontend to display attendance information.