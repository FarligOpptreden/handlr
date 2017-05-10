using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Web.Types;
using System.Collections.Generic;

namespace Handlr.Framework.Web
{
    public abstract partial class Handler : IController
    {
        public virtual User User()
        {
            return GetCookieObject<User>("H:U");
        }

        public virtual string GetUserId()
        {
            return User().ID.ToString();
        }

        public virtual bool IsAuthenticated()
        {
            return GetCookieObject<User>("H:U") != null;
        }

        public virtual bool PerformSignIn(string userId, string accessCode)
        {
            bool success = true;
            using (var db = Data.Core.Factory<Data.Http.Get>(Configuration.Accounts.Url))
            {
                string userJson = db.ExecuteReader("api/user", new Dictionary<string, object>()
                {
                    { "appId", Configuration.Application.Id },
                    { "userId", userId },
                    { "accessCode", accessCode }
                }) as string;
                if (!string.IsNullOrEmpty(userJson))
                {
                    User user = userJson.ToObject<User>();
                    if (!string.IsNullOrEmpty(user.Photo) && user.Photo[0] == '/')
                        user.Photo = Configuration.Accounts.Url + user.Photo;
                    if (user.Provider != null && !string.IsNullOrEmpty(user.Provider.Icon))
                        user.Provider.Icon = Configuration.Accounts.Url + user.Provider.Icon;
                    SetCookieObject("H:U", user);
                }
                else
                    success = false;
            }
            return success;
        }

        public virtual void PerformSignOut()
        {
            SetCookieObject<User>("H:U", null);
            if (!string.IsNullOrEmpty(Configuration.Accounts.Url) && AppId.HasValue)
                Redirect(Configuration.Accounts.Url + "/" + AppId.Value.ToString() + "/sign-out");
        }

        Config IController.Configuration()
        {
            return Configuration;
        }
    }
}
