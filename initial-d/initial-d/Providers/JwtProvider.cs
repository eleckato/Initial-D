using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace initial_d.Providers
{
    public class JwtProvider
    {
        private static string _tokenUri;

        //default constructor
        public JwtProvider() { }

        public static JwtProvider Create(string tokenUri)
        {
            _tokenUri = tokenUri;
            return new JwtProvider();
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            string methodName = "JwtProvider.GetTokenAsync";

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_tokenUri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("email", username),
                    new KeyValuePair<string, string>("hash", password),
                });

                    var response = await client.PostAsync("/user-auth", content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        var res = response.Headers.GetValues("error-code").First();
                        return res;
                    }
                    else
                    {
                        // Return null if unauthenticated
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"ERROR {methodName}: {e.Message}");
                    return null;
                }
            }
        }

        public JObject DecodePayload(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            string methodName = "JwtProvider.DecodePayload";

            try
            {
                var parts = token.Split('.');
                var payload = parts[1];

                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
                return JObject.Parse(payloadJson);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"ERROR {methodName}: {e.Message}");
                return null;
            }
        }

        public ClaimsIdentity CreateIdentity(bool isAuthenticated, string userName, dynamic payload)
        {
            if (string.IsNullOrEmpty(userName) || payload == null)
                return null;

            string methodName = "JwtProvider.CreateIdentity";

            try
            {
                // Decode the payload from token in order to create a claim
                string userId = payload.userId;
                string role = payload.Usertype;

                // Define the claim
                var jwtIdentity = new ClaimsIdentity(
                    new JwtIdentity(isAuthenticated, userName, DefaultAuthenticationTypes.ApplicationCookie)
                );

                // Add Claims NameIdentifier and Role
                jwtIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
                jwtIdentity.AddClaim(new Claim(ClaimTypes.Role, role));

                return jwtIdentity;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"ERROR {methodName}: {e.Message}");
                return null;
            }
        }

        private byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }

    public class JwtIdentity : IIdentity
    {
        private bool _isAuthenticated;
        private string _name;
        private string _authenticationType;

        public JwtIdentity() { }
        public JwtIdentity(bool isAuthenticated, string name, string authenticationType)
        {
            _isAuthenticated = isAuthenticated;
            _name = name;
            _authenticationType = authenticationType;
        }
        public string AuthenticationType
        {
            get
            {
                return _authenticationType;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}