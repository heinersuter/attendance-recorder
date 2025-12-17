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
* Node.js in a current version

## Build and run the app

1. Configure the file location in `appsettings.json` of ConsoleApp.
2. Build the client with `npm run build` (output is `wwwroot` of ConsoleApp).
3. Publish project ConsoleApp.
4. Run the ConsoleApp from publish folder.

### Start on login on Mac

To start the application on login, create a file `com.attendance.recorder.plist` in `~/Library/LaunchAgents/` with the following content:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    <dict>
        <key>Label</key>
        <string>com.attendance.recorder</string>
        <key>ProgramArguments</key>
        <array>
            <string>/usr/local/share/dotnet/dotnet</string>
            <string>AttendanceRecorder.ConsoleApp.dll</string>
        </array>
        <key>WorkingDirectory</key>
        <string>{Path to publish directory}/bin/Release/net9.0/publish</string>
        <key>RunAtLoad</key>
        <true/>
        <key>KeepAlive</key>
        <true/>
        <key>StandardErrorPath</key>
        <string>/tmp/com.attendance.recorder.err</string>
    </dict>
</plist>
```

Start service: `launchctl load ~/Library/LaunchAgents/com.attendance.recorder.plist`  
Stop service: `launchctl unload ~/Library/LaunchAgents/com.attendance.recorder.plist`

To check the location of the dotnet command, use `which dotnet`.

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