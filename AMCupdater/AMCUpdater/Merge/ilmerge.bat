@echo off
IF EXIST AMCUpdater_UNMERGED.exe del AMCUpdater_UNMERGED.exe
ren AMCUpdater.exe AMCUpdater_UNMERGED.exe
rem ilmerge /out:AMCUpdater.exe AMCUpdater_UNMERGED.exe Grabber.dll XionControls.dll Nlog.dll MyFilms.dll /allowDup:Grabber.MyFilmsIMDB
ilmerge /out:AMCUpdater.exe AMCUpdater_UNMERGED.exe Grabber.dll XionControls.dll Nlog.dll /target:dll /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319 /wildcards
rem ilmerge /out:AMCUpdater.exe AMCUpdater_UNMERGED.exe Grabber.dll XionControls.dll Nlog.dll /target:dll /targetplatform:"v4,%PROGS%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /wildcards
