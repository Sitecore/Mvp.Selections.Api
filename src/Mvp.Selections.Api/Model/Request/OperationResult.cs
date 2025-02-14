﻿using System.Net;

namespace Mvp.Selections.Api.Model.Request;

public class OperationResult<T>
    where T : class
{
    public T? Result { get; set; }

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotImplemented;

    public IList<string> Messages { get; } = [];
}