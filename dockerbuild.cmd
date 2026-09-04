docker build --platform linux/amd64 -t envoyreader2:v1 .
if %ERRORLEVEL% NEQ 0 (
    exit /b %ERRORLEVEL%
)

docker save -o ..\EnvoyReader2.tar envoyreader2:v1
if %ERRORLEVEL% NEQ 0 (
    exit /b %ERRORLEVEL%
)
