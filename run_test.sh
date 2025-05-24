#!/bin/bash

# Start the command in the background and capture its PID
echo "Starting application..."
dotnet run --project Source/OLLAMACHAT.Web/OLLAMACHAT.Web.csproj &
PID=$!
echo "Application PID: $PID"

# Wait for 15 seconds
echo "Waiting for 15 seconds..."
sleep 15

# Check if the process is still running
if kill -0 $PID 2>/dev/null; then
  echo "Result: Command started successfully and is still running after 15 seconds."
  # Gracefully stop the command
  echo "Stopping application (PID: $PID)..."
  kill $PID # Sends SIGTERM
  # Wait a bit for graceful shutdown
  sleep 5 
  if kill -0 $PID 2>/dev/null; then
    echo "Application did not stop with SIGTERM, sending SIGKILL..."
    kill -9 $PID
  else
    echo "Application stopped gracefully."
  fi
else
  echo "Result: Command failed to start or exited early."
fi

# Wait for the process to terminate (cleanup)
echo "Waiting for process to terminate completely..."
wait $PID 2>/dev/null
echo "Test script finished."
