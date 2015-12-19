@echo off
IF EXIST MyFilms_Grabber_Interface_UNMERGED.exe del MyFilms_Grabber_Interface_UNMERGED.exe
ren MyFilms_Grabber_Interface.exe MyFilms_Grabber_Interface_UNMERGED.exe
ilmerge /out:MyFilms_Grabber_Interface.exe MyFilms_Grabber_Interface_UNMERGED.exe NLog.dll Grabber.dll  /target:dll /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319 /wildcards
rem ilmerge /out:MyFilms_Grabber_Interface.exe MyFilms_Grabber_Interface_UNMERGED.exe NLog.dll Grabber.dll  /target:dll /targetplatform:"v4,%PROGS%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /wildcards
