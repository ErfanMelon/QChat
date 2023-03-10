using CSharpFunctionalExtensions;
using MediatR;

namespace QChat.Application.Behaviors;

public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            TResponse response;
            if (typeof(TResponse) == typeof(Result))
                response = (TResponse)Convert.ChangeType(Result.Failure(ex.Message), typeof(TResponse));

            else if (typeof(TResponse).Name == typeof(Result<>).Name)
            {
                response = (TResponse)Convert.ChangeType(new MyType(ex.Message), typeof(TResponse));
            }
            else
                response = default!;

            return response;
        }
    }
}
class MyType : IConvertible
{
    public string Error { get; set; }
    public MyType(string error)
    {
        Error = error;
    }
    public TypeCode GetTypeCode()
    {
        return TypeCode.Object;
    }
    public object ToType(Type conversionType, IFormatProvider? provider)
    {
        var generic = conversionType.GenericTypeArguments[0];
        var result = typeof(Result)
            .GetMethod("Failure", 1, new Type[] { Type.GetType("System.String")! })?
            .MakeGenericMethod(generic)
            .Invoke(null, new object[] { Error });

        return result!;
    }

    public bool ToBoolean(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public byte ToByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public char ToChar(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public DateTime ToDateTime(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public decimal ToDecimal(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public double ToDouble(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public short ToInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public int ToInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public long ToInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public sbyte ToSByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public float ToSingle(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public string ToString(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ushort ToUInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public uint ToUInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ulong ToUInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
}