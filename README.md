<br>
<img src="https://i.ibb.co/Jm1YPCm/Untitled-1.png" alt="Electrify Logo">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![language](https://img.shields.io/badge/language-C%23-239120)
![OS](https://img.shields.io/badge/OS-windows%2C%20macOS-0078D4)
![GitHub last commit](https://img.shields.io/github/last-commit/fM4s0n/Electrify)
![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/RoyalScribblz/aa76ad223dfcd71c919610990ab72893/raw/electrify-code-coverage.json)

## Table of Contents

- [About](#-%F0%9F%8C%90-About)
- [How to Run](#-%F0%9F%8F%83%E2%80%8D%E2%99%82%EF%B8%8F-how-to-run)
- [Configuration](#-%E2%9A%99%EF%B8%8F-configuration)
- [Authors](#%F0%9F%91%A8%E2%80%8D%F0%9F%91%A9%E2%80%8D%F0%9F%91%A6-authors)

## 🌐 About
Electrify is a smart meter client/server system for creating meter readings and generating bills using the DLMS protocol and COSEM objects.

The frontend is developed using .NET Maui Blazor Hybrid and is contained in the `Electrify.SmartMeterUi` and `Electrify.AdminUi` projects.

The server is written using AspNetCore and exposes endpoints to gRPC methods using HTTP 2, or alternatively HTTP 1/2 JSON endpoints, enabled by JsonTranscoding.

All test projects `Electrify.AdminUi.UnitTests`, `Electrify.SmartMeterUi.UnitTests`, `Electrify.ComponentTests`, `Electrify.Dlms.UnitTests`, `Electrify.E2ETests`, `Electrify.Server.UnitTests` are written using xUnit and can be ran like any Unit Test.

## 🏃‍♂️ How to Run

### MacOS
```shell
./run.sh
```

### Windows
```batch
.\run.bat
```

## ⚙️ Configuration
This application is fully configurable using the [IOptions pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options), the settings can be adjusted in the `MauiProgram.cs` files in the Maui applications, or the `appsettings.json file` on the Server.

## 👨‍👩‍👦 Authors

- Freddie Mason - [fM4s0n](https://github.com/fM4s0n)
- James Lloyd - [RoyalScribblz](https://github.com/RoyalScribblz) & [james-llo](https://github.com/james-llo)
- Hamzah Sidat - [hs2213](https://github.com/hs2213)
- Josh Cartledge - [JoshCartledge3](https://github.com/JoshCartledge3)
