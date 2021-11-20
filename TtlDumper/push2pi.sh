#!/bin/bash
if dotnet publish ; then
    # Empty remote folder
    ssh root@raspberrypi.local. "cd ttldumper &&  rm -Rf *"
    # Copy locally published files over to raspbery pi
    scp -r ./bin/Debug/net5.0/publish/* root@raspberrypi.local.:/root/ttldumper
fi