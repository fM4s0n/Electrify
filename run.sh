dotnet build -c Release

(
  cd Electrify.Server || exit
  dotnet run --Release --no-build
) &

(
  cd Electrify.AdminUi || exit
  dotnet run --Release --no-build
) &

(
  dotnet publish Electrify.SmartMeterUi -f net8.0-maccatalyst -c Release -p:CreatePackage=false --no-build
  cd Electrify.SmartMeterUi/bin/release/net8.0-maccatalyst || exit
  sleep 20
  open -n ./Electrify.SmartMeterUi.app --args 4b34de2e-c340-4aec-84bf-636e7a388410
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args e774ebc0-ea2c-4882-900d-bca4c37e535b
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 6842a005-1533-4abf-9abe-9a02b4ab304d
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 23a487cf-9cb0-48a5-9a36-99db82192799
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args ac1516a8-1c1d-417e-bcf9-95d308a65c47
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args e3f6fd12-fe66-4281-8709-1844703249ee
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 3c7c1e8e-4ccd-4b27-ae27-636d53376c59
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 83a3f16d-e0af-4e6e-a98a-62468625a0c3
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 7d5758fb-15b0-4645-b682-8fdd64bdf6fe
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 99a1d8fa-ea52-4447-a447-98b0e632cefe
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 7fe1e7f6-b679-40a8-8bd8-d1c21c58baf4
  sleep 5
  open -n ./Electrify.SmartMeterUi.app --args 3812f147-f064-4dd5-9584-f9397cddbd56
) &

wait
