using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using JhpDataSystem.store;
using JhpDataSystem.model;

namespace JhpDataSystem.Security
{
    public class UserAuthenticator
    {
        static KindName authenticationStore = new KindName("appusers");
        internal UserSession Authenticate(string name, int passCode)
        {
            //var keys = LocalEntityStore.Instance.GetKeys(authenticationStore);
            var asLower = name.ToLowerInvariant();
            var bolg = LoadCredentials().Where(t => t.UserId == name).FirstOrDefault();
            if (bolg == null)
                return null;

            var knownBolg = JhpSecurity.Encrypt(name + Constants.MOTHER_OFBOLG + passCode);
            if (knownBolg == bolg.SpawnOfAzog)
            {
                //you are an orc :)
                return new UserSession() { AuthorisationToken = Guid.NewGuid().ToString("N"), Id = bolg.Id, User = bolg };
            }

            return null;
        }

        List<AppUser> LoadCredentials()
        {
            var allCreds = new TableStore(Constants.KIND_APPUSERS).Get(null);
            if (allCreds == null)
                return new List<AppUser>();

            return (from credString in allCreds
                     select DbSaveableEntity.fromJson<AppUser>(credString)).ToList();
        }
    }
}