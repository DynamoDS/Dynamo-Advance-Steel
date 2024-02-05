using Autodesk.AutoCAD.Internal;
using Greg;
using Greg.AuthProviders;
using RestSharp;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Dynamo.Applications.AdvanceSteel
{
  internal class SteelAuthProvider : IOAuth2AuthProvider
  {
    private readonly SynchronizationContext SyncContext;

    public event Func<object, bool> RequestLogin;
    public event Action<LoginState> LoginStateChanged;

    public SteelAuthProvider()
    {
      SyncContext = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher);
    }

    public LoginState LoginState
    {
      get
      {
        bool result = false;
        SyncContext.Send((_) => result = WSUtils.IsLoggedIn(), null);
        return result ? LoginState.LoggedIn : LoginState.LoggedOut;
      }
    }

    public string Username
    {
      get
      {
        var result = string.Empty;
        SyncContext.Send((_) => result = WSUtils.GetLoginUserName(), null);
        return result;
      }
    }

    public bool Login()
    {
      bool result = false;

      SyncContext.Send((_) => result = WSUtils.Login(), null);
      LoginStateChanged?.Invoke(result ? LoginState.LoggedIn : LoginState.LoggedOut);

      return result;
    }

    public void Logout()
    {
      SyncContext.Send((_) => PInvoke.AcConnectWebServicesLogout(), null);
      LoginStateChanged?.Invoke(LoginState.LoggedOut);
    }

    public void SignRequest(ref RestRequest m, RestClient client)
    {
      if (LoginState == LoginState.LoggedOut && !Login())
      {
        throw new Exception("You must be logged into AutoCAD to use the package manager.");
      }

      m.AddHeader("Authorization", $"Bearer {WSUtils.GetO2tk("data:create data:write")}");
    }

    private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };
    private static readonly string[] UriRfc3968EscapedHex = new[] { "%21", "%2A", "%27", "%28", "%29" };

    private static string UrlEncodeRelaxed(string value)
    {
      // Start with RFC 2396 escaping by calling the .NET method to do the work.
      // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
      // If it does, the escaping we do that follows it will be a no-op since the
      // characters we search for to replace can't possibly exist in the string.
      var escaped = new StringBuilder(Uri.EscapeDataString(value));

      // Upgrade the escaping to RFC 3986, if necessary.
      for (var i = 0; i < UriRfc3986CharsToEscape.Length; i++)
      {
        var t = UriRfc3986CharsToEscape[i];
        escaped.Replace(t, UriRfc3968EscapedHex[i]);
      }

      // Return the fully-RFC3986-escaped string.
      return escaped.ToString();
    }
  }
  internal static class PInvoke
  {
    [DllImport("AcConnectWebServices.arx", EntryPoint = "AcConnectWebServicesLogout")]
    public static extern bool AcConnectWebServicesLogout();
  }
}