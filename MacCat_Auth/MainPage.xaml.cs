using System.Diagnostics;
using Security;

namespace MacCat_Auth;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
#if MACCATALYST
        RunAsRoot("echo hello world!");
#endif
    }

#if MACCATALYST
    bool RunAsRoot(string cmd)
    {
        try
        {
            var args = new[] { "", null };
            var parameters = new AuthorizationParameters
            {
                Prompt = "",
                PathToSystemPrivilegeTool = ""
            };

            var flags = AuthorizationFlags.ExtendRights |
                AuthorizationFlags.InteractionAllowed |
                AuthorizationFlags.PreAuthorize;

            using var auth = Authorization.Create(parameters, null, flags);
            int result = auth.ExecuteWithPrivileges(
                cmd,
                AuthorizationFlags.Defaults,
                args);
            if (result == 0) return true;
            if (Enum.TryParse(result.ToString(), out AuthorizationStatus authStatus))
            {
                if (authStatus == AuthorizationStatus.Canceled)
                {
                    return false;
                }
                else if (authStatus == AuthorizationStatus.ToolExecuteFailure)
                {
                    // Reaches here. -60031
                    // https://developer.apple.com/documentation/security/1540004-authorization_services_result_co/errauthorizationtoolexecutefailure
                    throw new InvalidOperationException($"Could not get authorization. {authStatus}");
                }
                else
                {
                    throw new InvalidOperationException($"Could not get authorization. {authStatus}");
                }
            }
            return false;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }

#endif
}


