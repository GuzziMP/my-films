REM %1 = Solution Directory
REM %2 = $(ConfigurationName) Debug/Release
REM echo ${revision}
REM Select program path based on current machine environment
set progpath=%ProgramFiles%
if not "%ProgramFiles(x86)%".=="". set progpath=%ProgramFiles(x86)%


echo %1 >%1\solutionbuild.log
echo %2 >>%1\solutionbuild.log
xcopy %1\AMCupdater\AMCupdater\Merge\AMCUpdater.exe %1\Installer\Base.12 /r/y >>%1\solutionbuild.log
xcopy %1\Grabber_Interface\Merge\MyFilms_Grabber_Interface.exe %1\Installer\Base.12 /r/y >>%1\solutionbuild.log
xcopy %1\PluginConfigLoader\bin\%2\PluginConfigLoader.exe %1\Installer\Base.12 /r/y >>%1\solutionbuild.log
xcopy %1\PluginConfigLoader\bin\%2\PluginConfigLoader.exe.config %1\Installer\Base.12 /r/y >>%1\solutionbuild.log
xcopy %1\Grabber_Plugin\bin\%2\MyVideoGrabber.dll %1\Installer\plugins.12\Windows /r/y >>%1\solutionbuild.log
xcopy %1\MyFilms\Merge\MyFilms.dll %1\Installer\plugins.12\Windows /r/y >>%1\solutionbuild.log
xcopy %1\AMCupdater\AMCupdaterSetup\%2\*.* %1\Installer\Config\MyFilms\AMCUinstaller /r/y >>%1\solutionbuild.log

cd %1\Installer
rem dir >>%1\solutionbuild.log

call mpe-maker.bat  >>%1\solutionbuild.log
rem "C:\Programme\Team MediaPortal\MediaPortal\MpeMaker.exe" MyFilms-12.xmp2 /V=1.0.0.0 /B

cd %1\Installer\MPE-Packages
dir >>%1\solutionbuild.log

cd %1\Installer
del ChangeLog.xml >>%1\solutionbuild.log
echo Write Changelog to Installer directory >>%1\solutionbuild.log
rem svn log %1 -r HEAD:1468 --xml >>%1\Installer\ChangeLog-V5.2.0.xml
rem svn log %1 -r HEAD:2738 --xml >>%1\Installer\ChangeLog.xml
svn log %1 -r HEAD:3107 --xml >>%1\Installer\ChangeLog.xml
echo Solution Build MyFilms finished ! >>%1\solutionbuild.log
