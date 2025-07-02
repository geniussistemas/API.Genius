using System;
using API.Genius.Lib.Util;

namespace API.Genius.Core.Models;

public class RequestToServer<DataClass> : RequestResponseServer<DataClass>
    where DataClass : class, new()
{
    public RequestToServer()
    {
        senderName = APIGeniusConst.APIGeniusName;
    }
}
