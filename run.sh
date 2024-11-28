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
) &

wait
