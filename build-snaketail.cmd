@echo off
msbuild SnakeTail\SnakeTail.csproj /p:Configuration=Release
msbuild SnakeTail.Wix\SnakeTail.Wix.wixproj /p:Configuration=Release;Platform=x86
msbuild SnakeTail.Wix\SnakeTail.Wix.wixproj /p:Configuration=Release;Platform=x64