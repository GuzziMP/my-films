set progpath=%ProgramFiles%
if not "%ProgramFiles(x86)%".=="". set progpath=%ProgramFiles(x86)%
"%progpath%\Team MediaPortal\MediaPortal\MpeMaker.exe" MyFilms-12.xmp2 /V=6.1.2.1462 /B
