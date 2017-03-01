using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Hexasoft
{
    public class BasicAuthentication : IHttpModule
    {
        static Regex requirePathRegex;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextBeginRequest;

            var regexRaw = ConfigurationManager.AppSettings["BasicAuthentication.RequiredOnPathRegex"];
            var ignoreCaseRaw = ConfigurationManager.AppSettings["BasicAuthentication.RequiredOnPathRegex.IgnoreCase"];

            if (!string.IsNullOrEmpty(regexRaw))
            {
                var options = RegexOptions.None;

                if (string.Equals(ignoreCaseRaw, "true", StringComparison.InvariantCultureIgnoreCase) || ignoreCaseRaw == "1")
                {
                    options |= RegexOptions.IgnoreCase;
                }

                requirePathRegex = new Regex(regexRaw, options);
            }
        }

        private void ContextBeginRequest(object sender, EventArgs e)
        {
            if (Required())
            {
                if (!ValidateCredentials())
                {
                    var httpApplication = (HttpApplication)sender;
                    httpApplication.Context.Response.Clear();
                    httpApplication.Context.Response.Status = "401 Unauthorized";
                    httpApplication.Context.Response.StatusCode = 401;
                    httpApplication.Context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"" + Request.Url.Host + "\"");
                    httpApplication.CompleteRequest();
                }
            }
        }

        private bool Required()
        {
            bool required = false;
            string requiredSetting = ConfigurationManager.AppSettings["BasicAuthentication.Required"];

            if (!string.IsNullOrWhiteSpace(requiredSetting))
            {
                requiredSetting = requiredSetting.Trim().ToLower();
                required = requiredSetting == "1" || requiredSetting == "true";
            }
            else if (requirePathRegex != null)
            {
                required = requirePathRegex.IsMatch(HttpContext.Current.Request.Url.AbsolutePath);
            }

            return required;
        }

        private bool ValidateCredentials()
        {
            string validUsername = ConfigurationManager.AppSettings["BasicAuthentication.Username"];

            if (string.IsNullOrEmpty(validUsername))
                return false;

            string validPassword = ConfigurationManager.AppSettings["BasicAuthentication.Password"];

            if (string.IsNullOrEmpty(validPassword))
                return false;

            string header = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(header))
                return false;

            header = header.Trim();
            if (header.IndexOf("Basic ", StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;

            string credentials = header.Substring(6);

            // Decode the Base64 encoded credentials
            byte[] credentialsBase64DecodedArray = Convert.FromBase64String(credentials);
            string decodedCredentials = Encoding.UTF8.GetString(credentialsBase64DecodedArray, 0, credentialsBase64DecodedArray.Length);

            // Get username and password
            int separatorPosition = decodedCredentials.IndexOf(':');

            if (separatorPosition <= 0)
                return false;

            string username = decodedCredentials.Substring(0, separatorPosition);
            string password = decodedCredentials.Substring(separatorPosition + 1);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return username.ToLower() == validUsername.ToLower() && password == validPassword;
        }

        private HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        public void Dispose()
        {
        }
    }
}