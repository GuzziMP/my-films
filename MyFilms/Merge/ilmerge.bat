@echo off
IF EXIST MyFilms_UNMERGED.dll del MyFilms_UNMERGED.dll
ren MyFilms.dll MyFilms_UNMERGED.dll
ilmerge /out:MyFilms.dll MyFilms_UNMERGED.dll NLog.dll Grabber.dll TaskScheduler.dll /target:dll /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319 /wildcards
rem ilmerge /out:MyFilms.dll MyFilms_UNMERGED.dll NLog.dll Grabber.dll TaskScheduler.dll /target:dll /targetplatform:"v4,%PROGS%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /wildcards
rem ilmerge /out:MesFilms.dll MesFilms_UNMERGED.dll Cornerstone.MP.dll NLog.dll AMCupdater.exe TaskScheduler.dll
