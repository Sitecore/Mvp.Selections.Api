using System.Text.Json;
using Microsoft.Extensions.Options;
using Mvp.Selections.Client.Configuration;
using Mvp.Selections.Client.Interfaces;
using Mvp.Selections.Client.Models;
using Mvp.Selections.Client.Tests.Serialization;
using Mvp.Selections.Domain;
using Xunit;
using File = System.IO.File;

namespace Mvp.Selections.Client.Tests;

public class ApplicationJsonDownload
{
    private const string SelectionGuid = "";

    private const string Token =
        "";

    private static readonly Uri _BaseAddress = new("http://localhost:7071");

    private static readonly HttpClient _HttpClient = new() { BaseAddress = _BaseAddress, Timeout = TimeSpan.FromSeconds(3600) };

#pragma warning disable xUnit1004
    [Fact(Skip = "Manual Test")]
#pragma warning restore xUnit1004
    public async Task Download()
    {
        MvpSelectionsApiClient client = new(_HttpClient, new OptionsMock(), new TokenMock());
        Response<IList<Application>> applications;
        int page = 1;
        do
        {
            applications = await client.GetApplicationsAsync(null, null, new Guid(SelectionGuid), null, ApplicationStatus.Submitted, page, 10);
            if (!Directory.Exists(SelectionGuid))
            {
                Directory.CreateDirectory(SelectionGuid);
            }

            foreach (Application application in applications.Result ?? [])
            {
                Response<Application> fullApplication = await client.GetApplicationAsync(application.Id);
                FileStream stream = File.Create($"{SelectionGuid}\\{application.Applicant.Name}-{application.Id}.json");
                await JsonSerializer.SerializeAsync(stream, fullApplication.Result, SerializationSettings.GetOptions());
                await stream.FlushAsync();
                stream.Close();
            }

            page++;
        }
        while (applications.Result?.Count > 0);
    }

    private class TokenMock : ITokenProvider
    {
        public Task<string> GetTokenAsync()
        {
            return Task.FromResult(Token);
        }
    }

    private class OptionsMock : IOptions<MvpSelectionsApiClientOptions>
    {
        public MvpSelectionsApiClientOptions Value => new() { BaseAddress = _BaseAddress };
    }
}