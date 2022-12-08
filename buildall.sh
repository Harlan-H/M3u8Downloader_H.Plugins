#!/bin/bash

targetDir=$1
if [ ! -d $targetDir ]; then
    echo "${targetDir} not found!!"
    exit
fi

plugins=$(ls | grep -i "M3u8downloader_H.*.plugin$")
for d in $plugins; do 
    dotnet publish $d/ -o $d/bin/Publish -c Release
    zip -j $targetDir/$d.zip $d/bin/Publish/$d.dll
done
