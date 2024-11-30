@echo off

dotnet build -c Release

start "Electrify.Server" cmd /c "cd Electrify.Server && dotnet run --Release --no-build"

start "Electrify.AdminUi" cmd /c "cd Electrify.AdminUi && dotnet run --Release --no-build"

(
    cd Electrify.SmartMeterUi
    dotnet build -f net8.0-windows10.0.19041.0 -c Release -p:PublishReadyToRun=true -p:WindowsPackageType=None
    cd bin\release\net8.0-windows10.0.19041.0\win10-x64
    timeout /t 30 /nobreak

    start "" Electrify.SmartMeterUi.exe 4b34de2e-c340-4aec-84bf-636e7a388410
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe e774ebc0-ea2c-4882-900d-bca4c37e535b
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 6842a005-1533-4abf-9abe-9a02b4ab304d
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 23a487cf-9cb0-48a5-9a36-99db82192799
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe ac1516a8-1c1d-417e-bcf9-95d308a65c47
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe e3f6fd12-fe66-4281-8709-1844703249ee
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 3c7c1e8e-4ccd-4b27-ae27-636d53376c59
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 83a3f16d-e0af-4e6e-a98a-62468625a0c3
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 7d5758fb-15b0-4645-b682-8fdd64bdf6fe
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 99a1d8fa-ea52-4447-a447-98b0e632cefe
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 7fe1e7f6-b679-40a8-8bd8-d1c21c58baf4
      timeout /t 3 /nobreak
    start "" Electrify.SmartMeterUi.exe 3812f147-f064-4dd5-9584-f9397cddbd56
)

pause
