@echo off
echo ====================================
echo Force Oracle XE Removal Script
echo ====================================

:: Stop Oracle Services
echo Stopping Oracle Services...
net stop OracleServiceXE
net stop OracleXETNSListener

:: Delete Oracle Services
echo Deleting Oracle Services...
sc delete OracleServiceXE
sc delete OracleXETNSListener

:: Delete Environment Variables
echo Cleaning up Environment Variables...
setx ORACLE_HOME ""
setx ORACLE_SID ""
setx TNS_ADMIN ""
:: Remove Oracle from PATH
for /f "tokens=1,* delims==" %%a in ('"set path"') do (
    echo %%b | find /i "oracle" >nul
    if not errorlevel 1 (
        set NEWPATH=%%b
        echo Updated PATH: !NEWPATH!
        setx PATH "!NEWPATH!"
    )
)

:: Delete Oracle Directories
echo Deleting Oracle folders...
rd /s /q "C:\oraclexe"
rd /s /q "C:\app"
rd /s /q "C:\ProgramData\Oracle"
rd /s /q "C:\Program Files\Oracle"
rd /s /q "%USERPROFILE%\AppData\Local\Oracle"
rd /s /q "%USERPROFILE%\AppData\Roaming\Oracle"

echo ====================================
echo Oracle XE Removal Complete.
echo Please reboot your system.
echo ====================================
pause
