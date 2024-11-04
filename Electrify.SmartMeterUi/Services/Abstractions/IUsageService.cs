using Electrify.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrify.SmartMeterUi.Services.Abstractions;
internal interface IUsageService
{
    UsageInstance GetCurrentUsage();

    void Start();

    void Stop();
}
