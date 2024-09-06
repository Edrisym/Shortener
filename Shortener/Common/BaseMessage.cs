using Shortener.Primitives.Response;

namespace Shortener.Common;

public class BaseMessages
{
    public static Message OperationSuccessfulMessage => new("عملیات با موفقیت انجام شد.");
    public static Message OneOrSomeValuesAreInvalid => new("یک یا چند مورد از اطلاعات معتبر نمیباشد.");
    public static Message OperationFailedMessage => new("عملیات با خطا مواجه شد.");
    public static Message InvalidArgumentException => new("پارامتر وارد شده معتبر نمیباشد.");
}