using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Mvp.Selections.Api.Configuration
{
    public class OpenApiConfigurationOptions : IOpenApiConfigurationOptions
    {
        public OpenApiInfo Info { get; set; } = new ()
        {
            Version = "4.10.0",
            Title = "Sitecore MVP Selections API",
            Description = "Supporting API for the Sitecore MVP Selection process.",
            Contact = new OpenApiContact
            {
                Name = "Ivan Lieckens",
                Email = "ivan.lieckens@sitecore.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        };

        public List<OpenApiServer> Servers { get; set; } = new ();

        public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;

        public bool IncludeRequestingHostName { get; set; }

        public bool ForceHttp { get; set; }

        public bool ForceHttps { get; set; }
    }
}
