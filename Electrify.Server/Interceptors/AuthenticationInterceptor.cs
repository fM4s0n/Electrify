using System.Reflection;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Electrify.Server.Interceptors;

/// <summary>
/// Uses reflection to determine if a request has a "Token" property, if it does
/// the AdminService is used to check its validity.
/// </summary>
/// <param name="adminService"></param>
public sealed class AuthenticationInterceptor(IAdminService adminService) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var property = typeof(TRequest).GetProperty("Token", BindingFlags.Public | BindingFlags.Instance);

        var value = property?.GetValue(request);

        if (property is null || value is not string tokenProperty)
        {
            return await continuation(request, context);
        }

        if (!Guid.TryParse(tokenProperty, out var token))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Token must be in GUID format"));
        }

        if (!await adminService.ValidateToken(token))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Token is invalid"));
        }

        return await continuation(request, context);
    }
}