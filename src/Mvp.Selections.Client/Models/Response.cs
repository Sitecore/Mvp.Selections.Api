﻿using System.Net;

namespace Mvp.Selections.Client.Models
{
    public class Response<T>
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Unused;

        public string Message { get; set; } = string.Empty;

        public T? Object { get; set; }
    }
}
