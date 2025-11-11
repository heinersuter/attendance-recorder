# Attendance Recorder

This application records the time a user is logged in to a computer.
If the session is locked, time recording is stopped.

## Time Recording

A LifeSign is written every 30 seconds.
All LifeSigns within the same minute are combined to an active interval.

A user can override the intervals by defining additional active intervals.
Or inactive intervals.

## Application Parts

### LifeSign

Every 30 seconds (configurable), a timestamp is written to a file.
Session lock and session unlock events are detected.

### WebApi

API to display information in the frontend. 
And to store overrides (merges).

### FileSystemStorage

Reading and writing files.

### BlazorUi

Frontend to display attendance information.  
Main starting point of the application.