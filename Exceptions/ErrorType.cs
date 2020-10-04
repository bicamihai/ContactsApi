using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsApi.Exceptions
{
    public enum ErrorType
    {
        ClientError,
        ServerError,
        AuthenticationRequired,
        ResourceNotFound,
        ResourceAlreadyExists,
        AccessForbidden,
        NotAllowed,
        ValidationError,
        Unprocessable
    }
}
