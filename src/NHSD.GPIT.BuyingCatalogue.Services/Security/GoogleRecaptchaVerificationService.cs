using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Security;

public class GoogleRecaptchaVerificationService : IRecaptchaVerificationService
{
    private readonly HttpClient httpClient;
    private readonly IOptions<RecaptchaSettings> settings;
    private readonly ILogger<GoogleRecaptchaVerificationService> logger;

    public GoogleRecaptchaVerificationService(
        HttpClient httpClient,
        IOptions<RecaptchaSettings> settings,
        ILogger<GoogleRecaptchaVerificationService> logger)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Validate(string recaptchaResponse)
    {
        if (string.IsNullOrWhiteSpace(recaptchaResponse))
            return false;

        var parameters = new Dictionary<string, string>
        {
            { "secret", settings.Value.SecretKey }, { "response", recaptchaResponse },
        };

        using var content = new FormUrlEncodedContent(parameters);
        var response = await httpClient.PostAsync(new Uri("siteverify", UriKind.Relative), content);

        if (!response.IsSuccessStatusCode) await HandleInvalidResponse(response);

        var responseContent = await response.Content.ReadAsStreamAsync();
        var deserializedResponse = await JsonSerializer.DeserializeAsync<GoogleRecaptchaResponse>(responseContent);

        return deserializedResponse.Success;
    }

    private async Task HandleInvalidResponse(HttpResponseMessage message)
    {
        var messageContent = await message.Content.ReadAsStringAsync();

        logger.LogError(
            "Received {StatusCode} from Google reCAPTCHA. Body: {Content}",
            message.StatusCode,
            messageContent);

        message.EnsureSuccessStatusCode();
    }

    [ExcludeFromCodeCoverage]
    internal class GoogleRecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error_codes")]
        public string[] ErrorCodes { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTimeOffset ChallengeTimestamp { get; set; }
    }
}
