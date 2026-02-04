; HUB Genius
; Instalação de aplicação 64bits como Serviço Windows ou Aplicação IIS

; === DEFINIÇÕES SOBRE A APLICAÇÃO ===
#define ApplicationName "HUB Genius"
#define NetVersion "net8.0"
#define ReleaseDirectory "bin\Release\"+NetVersion
#define PublishDirectory ReleaseDirectory+"\publish"
#define DestinationDirectory "Genius\HubGenius"
#define GroupName "Genius"
#define ExecFile "API.Genius.exe"
#define ConfigurationFile "appsettings.Production.json"
#define ConfigurationFileTemplate ConfigurationFile+".template"
#define FinalApplicationName "HUB Genius"
#define ServiceName"HUBGenius"
#define SiteName "HubGenius"


[Setup]
PrivilegesRequired=admin
AppName={#ApplicationName}
AppVersion=1.5
WizardStyle=modern dynamic
DefaultDirName={autopf}\{#DestinationDirectory}
DefaultGroupName={#GroupName}
UninstallDisplayIcon={app}\{#ExecFile}
Compression=lzma2
SetupIconFile=Setup.Ico
SolidCompression=yes
OutputDir=Setup
OutputBaseFilename=HubGeniusInstall
; "ArchitecturesAllowed=x64compatible" specifies that Setup cannot run
; on anything but x64 and Windows 11 on Arm.
ArchitecturesAllowed=x64compatible
; "ArchitecturesInstallIn64BitMode=x64compatible" requests that the
; install be done in "64-bit mode" on x64 or Windows 11 on Arm,
; meaning it should use the native 64-bit Program Files directory and
; the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
; Busca exe e dlls da pasta de publicaçao (publish)
Source: "{#PublishDirectory}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

; Busca template de configuração da pasta de release (build)
Source: "{#PublishDirectory}\{#ConfigurationFileTemplate}"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#FinalApplicationName}"; Filename: "{app}\{#ExecFile}"

[Code]

type
  TInstallMode = (Service, Desktop, IIS);
  
const
  INSTALL_MODE_SERVICE = 0;
  INSTALL_MODE_DESKTOP = 1;
  INSTALL_MODE_IIS = 2;
  
  CONFIG_FIELD_MANAGER_ADDRESS = 0;
  CONFIG_FIELD_MANAGER_PORT = 1;
  
  CONFIG_DEFAULT_MANAGER_ADDRESS = '127.0.0.1';
  CONFIG_DEFAULT_MANAGER_PORT = 9877;

  LPR_FIELD_URL = 0;
  LPR_FIELD_PORT = 1;
  
  LPR_DEFAULT_URL = 'http://127.0.0.1';
  LPR_DEFAULT_PORT = 5103;
  
  DB_FIELD_SERVER = 0;
  DB_FIELD_DATABASE = 1;
  DB_FIELD_USER = 2;
  DB_FIELD_PASSWORD = 3;
  
  DB_DEFAULT_SERVER = '.\';
  DB_DEFAULT_DATABASE = 'automacao';
  DB_DEFAULT_USER = 'sa';
  DB_DEFAULT_PASSWORD = '325014';
  
var
  ConfigFileExists: Boolean;

  InstallModePage: TInputOptionWizardPage;
  ConfigPage: TInputQueryWizardPage;
  DBConfigPage: TInputQueryWizardPage;
  LPRConfigPage: TInputQueryWizardPage;
  
// Altera um parâmetro em arquivos de configuração no formato XML
// Efetua a alteração sobre o primeiro parâmetro encontrado no arquivo
// Assume que o parâmetro está no formato:
//   <key_name>new_value</key_name>  
//
// Onde:
// key_name: Nome do parâmetro
// new_value: O valor a ser configurado no parâmetro
procedure ChangeKeyValuePairParameterXML(ConfigFile: string; Key: String; Value: String);
var
  Content: AnsiString;
  ContentString: String;
  KeyOpen: String;
  KeyClose: String;
begin
  if not FileExists(ConfigFile) then
  begin
    MsgBox('O arquivo de configuração ' + ConfigFile + ' não foi encontrado. Seus parâmetros não serão alterados.',
           mbInformation, MB_OK);
    Exit;
  end;
  
  if LoadStringFromFile(ConfigFile, Content) then
  begin
    ContentString := Content;
    KeyOpen := '<'+Key+'>';
    KeyClose := '</'+Key+'>';
    // Substitui o valor dentro da tag
    StringChangeEx(ContentString, KeyOpen + '.*' + KeyClose, 
                   KeyOpen + Value + KeyClose, True);
    SaveStringToFile(ConfigFile, ContentString, False);
  end;
end;

// Substitui um valor que está definido dentro da proóxima tag "<value>XXX</value>" da string
function ReplaceXmlValue(Content, NewValue: String): String;
var
  StartPos, EndPos: Integer;
begin
  StartPos := Pos('<value>', Content);
  EndPos := Pos('</value>', Content);

  if (StartPos > 0) and (EndPos > StartPos) then
  begin
    Delete(Content, StartPos, EndPos - StartPos + Length('</value>'));
    Insert('<value>' + NewValue + '</value>', Content, StartPos);
  end;

  Result := Content;
end;

// Altera um parâmetro em arquivos de configuração no formato XML
// Assume que o parâmetro está no formato:
//   <setting name="key_name" serializeAs="String">
//     <value>value</value>
//   </setting>
//
// Onde:
// key_name: Nome do parâmetro
// new_value: O valor a ser configurado no parâmetro
procedure ChangeKeyValuePairParameterXMLEx(ConfigFile: string; Key: String; Value: String);
var
  Content: AnsiString;
  ContentString: String;
  KeySearch: String;
  Before: String;
  After: String;
  i: Integer;
begin
  if not FileExists(ConfigFile) then
  begin
    MsgBox('O arquivo de configuração ' + ConfigFile + ' não foi encontrado. Seus parâmetros não serão alterados.',
           mbInformation, MB_OK);
    Exit;
  end;
  
  if LoadStringFromFile(ConfigFile, Content) then
  begin
    ContentString := Content;
    
    // Procura pela primeira ocorrência da tag "setting" do parâmetro
    KeySearch := '<setting name="'+Key+'" serializeAs="String">' + #13#10;
    i := Pos(KeySearch, ContentString);
    if i > 0 then
    begin
      // Reserva a parte da string até a tag de abertura de "setting" (inclusive) - Before
      // Separa o restante da string colocanfo a tag "value" no início - After
      Before := Copy(ContentString, 1, i + Length(KeySearch));
      After := Copy(ContentString, i + Length(KeySearch) + 1, Length(ContentString));
      
      // Troca o valor contido na tag "value" e monta novamente o conteúdo do arquivo
      After := ReplaceXmlValue(After, Value);
      ContentString := Before + After;
      
      SaveStringToFile(ConfigFile, ContentString, False);
    end;
  end;
end;

// Altera um parâmetro em arquivos de configuração no formato INI
// Assume que o parâmetro está no formato:
//   key_name=new_value
//
// Onde:
// key_name: Nome do parâmetro
// new_value: O valor a ser configurado no parâmetro
procedure ChangeKeyValuePairParameter(ConfigFile: string; Key: String; Value: String);
var
  Content: AnsiString;
  ContentString: String;
begin
  if not FileExists(ConfigFile) then
  begin
    MsgBox('O arquivo de configuração ' + ConfigFile + ' não foi encontrado. Um novo será criado.',
           mbInformation, MB_OK);
    // Cria o arquivo .config
    SaveStringToFile(ConfigFile, Key + '=' + Value + #13#10, False);
  end;
  
  if LoadStringFromFile(ConfigFile, Content) then
  begin
    ContentString := Content;
    
    if Pos(Key + '=', ContentString) >= 0 then
    begin
      // Substitui o valor dentro da tag
      StringChangeEx(ContentString, Key + '=.*' + #13#10, 
                     Key + '=' + Value + #13#10, True);
    end
    else
    begin
      ContentString := ContentString + Key + '=' + Value + #13#10;
    end;
    
    SaveStringToFile(ConfigFile, ContentString, False);
  end;
end;

// Altera um parâmetro não string em arquivos de configuração json
// Assume que o parâmetro está no formato:
//   "key_name": new_value,
//
// Onde:
// key_name: Nome do parâmetro
// new_value: O valor a ser configurado no parâmetro
procedure ChangeKeyValuePairParameterJson(ConfigFile: string; Key: String; Value: String; ValueDelimiter: String);
var
  Lines: TStringList;
  Line: String;
  Position: Integer;
  HasComma: Boolean;
  i: Integer;
begin
  if not FileExists(ConfigFile) then
  begin
    MsgBox('O arquivo de configuração ' + ConfigFile + ' não foi encontrado. Um novo será criado.',
           mbInformation, MB_OK);
    // Cria o arquivo .config
    SaveStringToFile(ConfigFile, '{' + #13#10 + 
                                 '  "' + Key + '": ' + ValueDelimiter + Value + ValueDelimiter + #13#10 + 
                                 '}', False);
  end;

  Lines := TStringList.Create;
  try
    Lines.LoadFromFile(ConfigFile);
    
    for i := 0 to Lines.Count - 1 do
    begin
      Line := Lines[i];
      
      HasComma := Line[Length(Line)] = ',';
      
      Position := Pos('"' + Key + '":', Line);
      if Position > 0 then
      begin
        Lines[i] := Copy(Line, 1, Position + Length(Key) + 1) + ': ' + ValueDelimiter + Value + ValueDelimiter;
        if HasComma then
          Lines[i] := Lines[i] + ',';
        Lines.SaveToFile(ConfigFile);
        Break;
      end;
    end;
  finally
    Lines.Free;
  end;
end;

// Altera um parâmetro não string em arquivos de configuração json
// Assume que o parâmetro está no formato:
//   "key_name": new_value,
//
// Onde:
// key_name: Nome do parâmetro
// new_value: O valor a ser configurado no parâmetro
procedure ChangeKeyValuePairNotStringParameterJson(ConfigFile: string; Key: String; Value: String);
begin
  ChangeKeyValuePairParameterJson(ConfigFile, Key, Value, '');
end;

// Altera um parâmetro string em arquivos de configuração json
// Assume que o parâmetro está no formato:
//   "key_name": "new_value",
//
// Onde:
// key_name: Nome do parâmetro
// new_value: O valor a ser configurado no parâmetro
procedure ChangeKeyValuePairStringParameterJson(ConfigFile: string; Key: String; Value: String);
begin
  ChangeKeyValuePairParameterJson(ConfigFile, Key, Value, '"');
end;

procedure VerifyConfigFileExists;
var
  ConfigFile: String;
begin
//  ConfigFile := ExpandConstant('{app}\{#ConfigurationFile}');
  ConfigFile := ExpandConstant('{autopf}\{#DestinationDirectory}\{#ConfigurationFile}');
  ConfigFileExists := FileExists(ConfigFile);
end;


function InitializeSetup: Boolean;
begin
  // Verifica se o arquivo de configuração já existe
  // Se ele já existir, o instalador não pede dados de configuraçao e não
  // altera as configurações existentes
  VerifyConfigFileExists;

{
  // *** CÓDIGO DE TESTE - IMPEDE QUE A INSTALAÇÃO PROSSIGA ***
  // Use este trecho para testar alguma lógica do script
  // Ao retornar False, a instlação é interrompida sem que a aplicaçao e 
  // demais arquivos sejam instalados

  // Colocar o código de teste aqui
  ChangeKeyValuePairNotStringParameterJson('C:\Temp\appsettings.json', 'GerenciadorPort', '12345');
  ChangeKeyValuePairStringParameterJson('C:\Temp\appsettings.json', 'AllowedOrigins', 'http://127.0.0.1:5050');


  // Retornar False para interromper a instalaçao
  Result := False;

  // **********************************************************
}
  Result := True;
end;

procedure CreateInstallModePage();
begin
  // Cria uma página para selecionar o modo de instalação
  InstallModePage := CreateInputOptionPage(
    wpWelcome,                                                  // página anterior
    'Modo de Instalação',                                       // título
    'Escolha como deseja instalar {#FinalApplicationName}',     // descrição
    'Selecione uma das opções abaixo:',                         // instrução
    True,                                                       // Exclusive (radio button)
    False);                                                     // Não mostrar como ListBox
    
  InstallModePage.Add('Instalar como Serviço Windows');
  InstallModePage.Add('Instalar como Aplicação Desktop');
  (**********************************************************************
   ** A instalação não está permitindo ainda IIS
  InstallModePage.Add('Instalar como Aplicação IIS');
  **********************************************************************)
end;

procedure CreateConfigPage();
begin
  // Cria uma página para coletar dados de configuração
  ConfigPage := CreateInputQueryPage(
    InstallModePage.ID,                                             // página anterior
    'Configuração',                                                 // título
    'Informe as configurações gerais de {#FinalApplicationName}',   // descrição
    'Informe os dados abaixo'                                       // instrução
  );

  // Adiciona um campo de texto
  ConfigPage.Add('Endereço do Gerenciador:', False);
  ConfigPage.Add('Porta do Gerenciador:', False);
  
  ConfigPage.Values[CONFIG_FIELD_MANAGER_ADDRESS] := CONFIG_DEFAULT_MANAGER_ADDRESS;
  ConfigPage.Values[CONFIG_FIELD_MANAGER_PORT] := IntToStr(CONFIG_DEFAULT_MANAGER_PORT);
end;

procedure CreateDBConfigPage();
begin
  // Cria uma página para coletar dados de configuração
  DBConfigPage := CreateInputQueryPage(
    ConfigPage.ID,       // página anterior
    'Configuração do banco de dados',           // título
    'Informe as configurações para acesso ao banco de dados',
    'Informe os dados abaixo' // instrução
  );

  // Adiciona um campo de texto
  DBConfigPage.Add('Servidor do Banco de Dados:', False);
  DBConfigPage.Add('Nome do Banco de Dados:', False);
  DBConfigPage.Add('Usuário:', False);
  DBConfigPage.Add('Senha:', True);
  
  DBConfigPage.Values[DB_FIELD_SERVER] := DB_DEFAULT_SERVER; //'.';
  DBConfigPage.Values[DB_FIELD_DATABASE] := DB_DEFAULT_DATABASE; //'automacao';
  DBConfigPage.Values[DB_FIELD_USER] := DB_DEFAULT_USER; //'sa';
  DBConfigPage.Values[DB_FIELD_PASSWORD] := DB_DEFAULT_PASSWORD; //'sa';
end;

procedure CreateLPRConfigPage();
begin
  // Cria uma página para coletar dados de configuração
  LPRConfigPage := CreateInputQueryPage(
    DBConfigPage.ID,
    'Configuração do LPR',
    'Informe as configurações para integraçao com as câmeras de LPR',
    'Os endereços das câmeras devem ser separados por vírgulas.' + #13#10 +
    'Exemplo: "http://192.168.0.10, http://192.168.0.20"'
  );

  // Adiciona um campo de texto
  LPRConfigPage.Add('Endereços Câmeras:', False);
  LPRConfigPage.Add('Porta HUB Genius:', False);
  
  LPRConfigPage.Values[LPR_FIELD_URL] := LPR_DEFAULT_URL;
  LPRConfigPage.Values[LPR_FIELD_PORT] := IntToStr(LPR_DEFAULT_PORT);
end;
  
procedure InitializeWizard;
begin
  CreateInstallModePage();
  if not ConfigFileExists then
  begin
    CreateConfigPage();
    CreateDBConfigPage();
    CreateLPRConfigPage();
  end;
end;

function GetInstallMode: TInstallMode;
begin
    if InstallModePage.SelectedValueIndex = INSTALL_MODE_SERVICE then
      Result := Service
    else if InstallModePage.SelectedValueIndex = INSTALL_MODE_IIS then
      Result := IIS
    else
      Result := Desktop;
end;

procedure ConfigureHUB(ConfigFile: String);
begin
  ChangeKeyValuePairStringParameterJson(ConfigFile, 'GerenciadorHost', ConfigPage.Values[CONFIG_FIELD_MANAGER_ADDRESS]);
  ChangeKeyValuePairNotStringParameterJson(ConfigFile, 'GerenciadorPort', ConfigPage.Values[CONFIG_FIELD_MANAGER_PORT]);
end;

procedure ConfigureDB(ConfigFile: String);
var
  Server: String;
  ConnectionString: String;
begin
  Server := DBConfigPage.Values[DB_FIELD_SERVER];
  StringChangeEx(Server, '\', '\\', True);
  ConnectionString := 'Server=' + Server + ';' +  // Necessário para o C# ler corretamente a "\"
                      'Database='+ DBConfigPage.Values[DB_FIELD_DATABASE] + ';' +
                      'User ID=' + DBConfigPage.Values[DB_FIELD_USER] + ';' +
                      'Password=' + DBConfigPage.Values[DB_FIELD_PASSWORD] + ';' +
                      'Trusted_Connection=false;' +
                      'TrustServerCertificate=true;';
  ChangeKeyValuePairStringParameterJson(ConfigFile, 'DefaultConnectionString', ConnectionString);
end;

function CreateConfigFileFromTemplate(ConfigFile: String): Boolean;
var
  ConfigFileTemplate: String;
begin
  Result := True;
  
  try
    ConfigFileTemplate := ExpandConstant('{app}\{#ConfigurationFileTemplate}');;
    if not FileExists(ConfigFileTemplate) then
    begin
      Result := False;
      MsgBox('Arquivo template de configuraçao não encontrado' + #13#10 + 
             ConfigFileTemplate + #13#10#13#10 + 
             'A configuração não será efetivada pelo instalador',
             mbError,
             MB_OK);
    end
    else
    begin
      if not CopyFile(ConfigFileTemplate, ConfigFile, False) then
      begin
        Result := False;
        MsgBox('Erro ao criar arquivo de configuração' + #13#10 + 
               ConfigFile + #13#10#13#10 + 
               'A configuração não será efetivada pelo instalador',
               mbError,
               MB_OK);
      end;
    end;
  except
    begin
      MsgBox('Erro ao criar arquivo de configuração' + #13#10 + 
             ConfigFile + #13#10#13#10 + 
             'A configuração não será efetivada pelo instalador',
             mbError,
             MB_OK);
      Result := False;
    end;
  end;
end;

procedure ConfigureLPR(ConfigFile: String);
var
  Port: String;
  UrlList: array of String;
  UrlListStr: String;
  i: Integer;
begin
  Port := LPRConfigPage.Values[LPR_FIELD_PORT];
  
  // Transforma: 'http://192.168.0.10, http://192.168.0.20' 
  //         em: '[ "http://192.168.0.10:5103", "http://192.168.0.20:5103" ]'
  UrlList := StringSplit(LPRConfigPage.Values[LPR_FIELD_URL], [','], stExcludeEmpty);
  UrlListStr := '';
  for i := 0 to Length(UrlList) - 1 do
  begin
    if i > 0 then
      UrlListStr := UrlListStr + ', ';
    UrlListStr := UrlListStr + '"' + Trim(UrlList[i]) + ':' + Port + '"';
  end;
  if Length(UrlList) > 1 then
    UrlListStr := '[' + UrlListStr + ']';
    
  ChangeKeyValuePairNotStringParameterJson(ConfigFile, 'AllowedOrigins', UrlListStr);
  ChangeKeyValuePairStringParameterJson(ConfigFile, 'Url', 'http://*:' + Port);
end;

procedure InstallAsService;
var
  ResultCode: Integer;
begin
  if Exec(
    'sc.exe',
    'create {#ServiceName} binPath= "' + ExpandConstant('{app}\{#ExecFile}') + '" start= auto',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode
  ) then
  begin
    MsgBox('O serviço "{#ServiceName}" foi criado com sucesso!', mbInformation, MB_OK);
  end
  else
  begin
    MsgBox('Falha ao criar o serviço "{#ServiceName}". Código de erro: ' + IntToStr(ResultCode), mbError, MB_OK);
  end;
end;

procedure InstallAsIIS;
var
  ResultCode: Integer;
begin
  if Exec(
    ExpandConstant('{sys}\inetsrv\appcmd.exe'),
    'add app /site.name:"Default Web Site" /path:/{#SiteName} /physicalPath:"' + ExpandConstant('{app}') + '"',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode
  ) then
  begin
    MsgBox('A aplicação "{#SiteName}" foi adicionada ao IIS com sucesso!', mbInformation, MB_OK);
  end
  else
  begin
    MsgBox('Falha ao adicionar a aplicação "{#SiteName}" ao IIS. Código de erro: ' + IntToStr(ResultCode), mbError, MB_OK);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ConfigFile: String;
//  ResultCode: Integer;
  InstallMode: TInstallMode;
begin
  if CurStep = ssPostInstall then
  begin
    InstallMode := GetInstallMode;
  
    // Só altera configurações se o arquivo de configuração ainda não existe
    if not ConfigFileExists then
    begin
      ConfigFile := ExpandConstant('{app}\{#ConfigurationFile}');
    
      
      // Só efetiva as configurações se for possível criar o arquivo de 
      // configuração a partir do arquivo de template
      if CreateConfigFileFromTemplate(ConfigFile) then
      begin
        ConfigureHUB(ConfigFile);
        ConfigureDB(ConfigFile);
        ConfigureLPR(ConfigFile);
      end;
    end;
    
    // Instalação como Serviço
    if InstallMode = Service then
      InstallAsService;

    // Instalação como Aplicação IIS
    if InstallMode = IIS then
      InstallAsIIS;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    // Tenta parar e remover o serviço
    if Exec('sc.exe', 'stop {#ServiceName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      MsgBox('O serviço "{#ServiceName}" foi parado com sucesso.', mbInformation, MB_OK)
    else
      MsgBox('Não foi possível parar o serviço "{#ServiceName}" (pode não existir).', mbInformation, MB_OK);

    if Exec('sc.exe', 'delete {#ServiceName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      MsgBox('O serviço "{#ServiceName}" foi removido com sucesso.', mbInformation, MB_OK)
    else
      MsgBox('Não foi possível remover o serviço "{#ServiceName}" (pode não existir).', mbInformation, MB_OK);

    // Tenta remover a aplicação IIS
    if Exec(ExpandConstant('{sys}\inetsrv\appcmd.exe'),
            'delete app /app.name:Default Web Site/{#SiteName}',
            '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      MsgBox('A aplicação "{#SiteName}" foi removida do IIS com sucesso.', mbInformation, MB_OK)
    else
      MsgBox('Não foi possível remover a aplicação "{#SiteName}" do IIS (pode não existir).', mbInformation, MB_OK);
  end;
end;
