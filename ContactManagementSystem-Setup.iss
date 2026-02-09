; Contact Management System - Inno Setup Installer Script
; This creates a professional Windows installer
; Download Inno Setup from: https://jrsoftware.org/isdl.php

[Setup]
AppName=Contact Management System
AppVersion=1.0.0
AppPublisher=Contact Management Team
AppPublisherURL=https://github.com/abraham9486937737/Contact-Management-System
AppSupportURL=https://github.com/abraham9486937737/Contact-Management-System/issues
DefaultDirName={autopf}\Contact Management System
DefaultGroupName=Contact Management System
AllowNoIcons=yes
OutputBaseFilename=ContactManagementSystem-Installer-1.0.0
OutputDir=.\Installers
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest
DisableDirPage=no
DisableProgramGroupPage=no
SetupIconFile=ContactManagementAPI\wwwroot\favicon.ico
WizardStyle=modern
MinVersion=10.0.17763
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Copy all published files to installation directory
Source: "ContactManagementAPI\bin\Release\net8.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Application shortcut in Start Menu
Name: "{group}\Contact Management System"; Filename: "{app}\ContactManagementAPI.exe"; WorkingDir: "{app}"; IconFileName: "{app}\ContactManagementAPI.exe"
Name: "{group}\Run Application"; Filename: "{app}\Run.bat"; WorkingDir: "{app}"
Name: "{group}\Uninstall"; Filename: "{uninstallexe}"

; Desktop shortcut
Name: "{autodesktop}\Contact Management System"; Filename: "{app}\ContactManagementAPI.exe"; WorkingDir: "{app}"; IconFileName: "{app}\ContactManagementAPI.exe"

[Run]
; Run application after installation
Filename: "{app}\ContactManagementAPI.exe"; Description: "Launch Contact Management System"; Flags: nowait postinstall skipifsilent WorkingDir: "{app}"

[UninstallDelete]
; Clean up application data on uninstall
Type: filesandordirs; Name: "{app}\*"
Type: dirifempty; Name: "{app}"

[Code]
// Check if .NET 8 is installed for framework-dependent version
function IsDotNetInstalled: Boolean;
var
  ResultCode: Integer;
begin
  if not ShellExec('open', ExpandConstant('{cmd}'), '/c dotnet --version', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Result := False;
  end else
  begin
    Result := True;
  end;
end;

[Messages]
BeveledLabel=Contact Management System v1.0.0
SetupWindowTitle=Contact Management System - Installation Wizard
WizardSmallImageFile=compiler:WizModernSmallImage.bmp

[InstallDelete]
; Remove old versions
Type: filesandordirs; Name: "{app}"
