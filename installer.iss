[Setup]
AppId={{A7A022F2-41E1-4EED-AC5A-3B10FA77EC6B}}
AppName=League RPC
AppVersion=1.0.0
AppPublisher=YourName
DefaultDirName={autopf}\LeagueRPC
DefaultGroupName=League RPC
UninstallDisplayIcon={app}\LeagueRPC.exe
SetupIconFile=icon.ico
OutputDir=installer_output
OutputBaseFilename=LeagueRPC-Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "startupicon"; Description: "Start League RPC automatically when Windows starts"; GroupDescription: "Startup:"; Flags: unchecked

[Files]
Source: "bin\Release\net10.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\League RPC"; Filename: "{app}\LeagueRPC.exe"
Name: "{group}\Uninstall League RPC"; Filename: "{uninstallexe}"
Name: "{autodesktop}\League RPC"; Filename: "{app}\LeagueRPC.exe"; Tasks: desktopicon
Name: "{userstartup}\League RPC"; Filename: "{app}\LeagueRPC.exe"; Tasks: startupicon

[Run]
Filename: "{app}\LeagueRPC.exe"; Description: "Launch League RPC"; Flags: nowait postinstall skipifsilent