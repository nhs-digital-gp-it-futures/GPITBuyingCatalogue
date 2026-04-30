using System.Diagnostics;
using System.Text;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure
{
    public class TestServerFixture : IAsyncLifetime
    {
        private static readonly string ProjectPath = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", "..",   // ← segments to reach solution root
        "src",
        "NHSD.GPIT.BuyingCatalogue.WebApp",
        "NHSD.GPIT.BuyingCatalogue.WebApp.csproj"
            )
        );

        public string BaseUrl { get; } = "https://localhost:5001";

        private Process? _serverProcess;
        private readonly StringBuilder _log = new();

        public async Task InitializeAsync()
        {
            _serverProcess = StartServer();
            await WaitUntilReadyAsync(timeout: TimeSpan.FromSeconds(60));
        }

        public Task DisposeAsync()
        {
            Shutdown();
            return Task.CompletedTask;
        }

        private Process StartServer()
        {
            var info = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{ProjectPath}\" --launch-profile \"NHSD.GPIT.BuyingCatalogue.WebApp\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = new Process { StartInfo = info, EnableRaisingEvents = true };

            process.OutputDataReceived += (_, e) => Log(e.Data);
            process.ErrorDataReceived += (_, e) => Log(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }

        private async Task WaitUntilReadyAsync(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            using var client = BuildHttpClient();

            while (!cts.Token.IsCancellationRequested)
            {
                ThrowIfProcessDied();

                if (await IsResponsiveAsync(client))
                    return;

                await Task.Delay(2_000, cts.Token);
            }

            throw new TimeoutException(
                $"Server at {BaseUrl} was not ready within {timeout.TotalSeconds}s.{Environment.NewLine}{_log}");
        }

        private async Task<bool> IsResponsiveAsync(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(BaseUrl);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        private void ThrowIfProcessDied()
        {
            if (_serverProcess is { HasExited: true })
                throw new InvalidOperationException(
                    $"Server process exited unexpectedly (code {_serverProcess.ExitCode}).{Environment.NewLine}{_log}");
        }

        private void Shutdown()
        {
            if (_serverProcess is null || _serverProcess.HasExited)
                return;

            try
            {
                _serverProcess.Kill(entireProcessTree: true);
            }
            finally
            {
                _serverProcess.Dispose();
            }
        }

        private static HttpClient BuildHttpClient() =>
            new(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });

        private void Log(string? line)
        {
            if (!string.IsNullOrWhiteSpace(line))
                _log.AppendLine(line);
        }
    }
}
