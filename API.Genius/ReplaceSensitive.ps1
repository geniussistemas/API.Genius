param(
    [string]$filePath
)

# Lê todo o conteúdo do arquivo
$content = Get-Content $filePath -Raw

# Substitui a senha (qualquer valor após "Password=" até o próximo delimitador ;)
$content = $content -replace '(?i)(Password=)[^;"<]*', '${1}{{PASSWORD}}'

# Substitui o usuário (qualquer valor após "User ID=" até o próximo delimitador ;)
$content = $content -replace '(?i)(User ID=)[^;"<]*', '${1}{{USER}}'

# Salva de volta
Set-Content -Path $filePath -Value $content -Encoding UTF8