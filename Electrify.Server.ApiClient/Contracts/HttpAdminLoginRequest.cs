using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrify.Server.ApiClient.Contracts;
public sealed record HttpAdminLoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
