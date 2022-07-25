using System.Runtime.InteropServices;
using Application.Abtractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassLibrary1.Enviroment;

public class DeploymentEnvironment : IDeploymentEnvironment
{
    private readonly ILogger<DeploymentEnvironment> _logger; 
    private readonly IWebHostEnvironment _hostingEnv;

    public DeploymentEnvironment(ILogger<DeploymentEnvironment> logger, IWebHostEnvironment hostingEnv)
    {
        _logger = logger;
        _hostingEnv = hostingEnv;
    }

    public string OS { get; } = $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}";
   
    public string MachineName => System.Environment.MachineName;
    public string RuntimeFramework =>
        $"{RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture}";
    public string EnvironmentName => _hostingEnv.EnvironmentName;

    public string CommitSha => "ThisAssembly.Git.Commit";

    public string Branch => "ThisAssembly.Git.Branch";
    public string Tag => "ThisAssembly.Git.Tag";
}