@echo off
msbuild SnakeTail\SnakeTail.csproj /p:Configuration=Release;Platform=AnyCPU /t:Clean;Rebuild
msbuild SnakeTail.Wix\SnakeTail.Wix.wixproj /p:Configuration=Release;Platform=AnyCPU;TargetPlatform=x86
msbuild SnakeTail.Wix\SnakeTail.Wix.wixproj /p:Configuration=Release;Platform=AnyCPU;TargetPlatform=x64