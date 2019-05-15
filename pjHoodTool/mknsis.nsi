XPStyle on
SetCompressor /SOLID LZMA

Var wasInUse

Name "pj Hood Tool"
!include outfile.txt

InstallDirRegKey HKCU Software\Ambertation\SimPe\Settings Path
LicenseData "LICENSE.txt"

Page license
Page directory
Page instfiles

Section
  SetOutPath $INSTDIR
  Call CheckExists
  IfErrors 0 +2
  Abort "Cannot continue to install with SimPe running."
  File /r "Plugins"
SectionEnd

Function .onVerifyInstDir
  IfFileExists $INSTDIR\SimPe.exe +2
    Abort
FunctionEnd

Function .onInstSuccess
  StrCmp $wasInUse 1 WasInUse
  Return
WasInUse:
  Exec "$INSTDIR\SimPe.exe"
FunctionEnd



Function CheckExists
  StrCpy $wasInUse 0

  IfFileExists "$INSTDIR\Plugins\pjHoodTool.plugin.dll" Exists
  Return
Exists:
  FileOpen $0 "$INSTDIR\Plugins\pjHoodTool.plugin.dll" a
  IfErrors InUse
  FileClose $0
  Return
InUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL \
    "SimPe is running.$\r$\nPlease close it and retry.$\r$\nIt will be restarted after the install." \
    IDRETRY Exists
  SetErrors
FunctionEnd