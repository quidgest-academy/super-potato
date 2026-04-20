using CommandLine;

namespace AdminCLI;

[Verb("user-setup", HelpText = "Setups the roles defined in this app in the configured providers")]
public class SetupUserProvidersOptions
{
}


partial class AdminCLI
{
    private static int SetupUserProviders(SetupUserProvidersOptions opts)
    {
        dBUserManagement.SetupProviders();
        return 0;
    }
}