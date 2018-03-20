using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace mvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api","My Api")
            };
        }

        //
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "Mvc Client",
                    ClientUri = "http://localhost:5001",
                    LogoUri = "https://chocolatey.org/content/packageimages/visualstudio2017-workload-netweb.1.1.1.png",
                    AllowRememberConsent = true,



                    AllowedGrantTypes = GrantTypes.Implicit,//隐式模式
                    ClientSecrets = {new Secret("secret".Sha256())},
                    RequireConsent = true,//用户同意授权机制，暂时不做，直接跳转 也可以在可信任端使用这种设置
                    RedirectUris = {"http://localhost:5001/signin-oidc"},//正式环境应当写在数据库中，改地址为处理认证逻辑的固定地址
                    PostLogoutRedirectUris = {"http://localhost:5001/signout-callback-oidc"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email
                    }
                },

            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "10000",
                    Username = "WRF",
                    Password = "123456",
                    Claims = new List<Claim>{
                        new Claim("name","WRF"),
                        new Claim("website","video"),
                        new Claim("sth","sth")
                    }
                }
            };
        }
    }
}
