@echo off
setlocal

::-------------------------------------------------------------------
:: Deploy Solution
echo Starting Docker Compose...
docker compose up -d --build
if %ERRORLEVEL% neq 0 (
    echo Error: Docker Compose failed with exit code %ERRORLEVEL%
    exit /b %ERRORLEVEL%
)
echo Deployment complete!

pause
::-------------------------------------------------------------------
:: Database maintenance
:: Note: If you're running this command directly in the terminal (and not from within a .bat file), replace %%i with %i.
FOR /F "tokens=*" %%i IN ('docker ps -q --filter "ancestor=myapp-webadmin:latest"') DO docker exec -i %%i bash < docker_db_maintenance.sh


::-------------------For macOS users----------------------------------
:: If you're using macOS, please run the following two commands:
::docker compose up -d --build
::docker exec -i $(docker ps -q --filter ancestor=myapp-webadmin:latest | head -n 1) bash < docker_db_maintenance.sh


echo To finish, press any key...
pause >nul
endlocal
