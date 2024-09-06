using Shortener.Primitives.Response;

namespace Shortener.Common;

public abstract class BaseErrors
{
    public static Error InternalServerError => new("ApplicationFailure", "An Internal Server Error Has Occured");
    public static Error IdentityConflictError => new("IdentityConflict", "Identity values do not match eachother");
}