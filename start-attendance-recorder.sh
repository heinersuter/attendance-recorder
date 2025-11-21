#!/bin/zsh
# Shell script to start AttendanceRecorder.ConsoleApp from bin/Debug/net9.0

APP_DIR="$(dirname "$0")/AttendanceRecorder.ConsoleApp/bin/Release/net9.0/publish"
DLL="$APP_DIR/AttendanceRecorder.ConsoleApp.dll"

if [ ! -f "$DLL" ]; then
  echo "AttendanceRecorder.ConsoleApp.dll not found in $APP_DIR"
  exit 1
fi

cd "$APP_DIR" || exit 1

echo "Starting AttendanceRecorder.ConsoleApp..."
dotnet AttendanceRecorder.ConsoleApp.dll
