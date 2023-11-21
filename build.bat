@ECHO off

set "modname=OinkyMod"
set "fromrelativepath=bin\Debug\netstandard2.1"
set "destination=C:\Program Files (x86)\Steam\steamapps\common\Lethal Company\BepInEx\plugins"

dotnet build

echo:
@echo Copying mod files...
copy "%~dp0\%fromrelativepath%\%modname%.dll" "%destination%"
@echo Done!

echo:
pause