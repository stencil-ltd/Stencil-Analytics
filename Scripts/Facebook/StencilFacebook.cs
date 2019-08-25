#if STENCIL_FACEBOOK
using Facebook.Unity;
using System;
using UI;
using UnityEngine;
using Util;

namespace Scripts.Facebook
{
    public class StencilFacebook : Controller<StencilFacebook>
    {
        public static event EventHandler OnFacebook;
        
        private void Start()
        {
            FB.Mobile.FetchDeferredAppLinkData();
            FB.Init(() =>
            {
                Debug.Log($"Facebook init: {FB.IsInitialized} [authed={FB.IsLoggedIn}]");
                FB.ActivateApp();
                OnFacebook?.Invoke();
            });
        }
    }
}

#endif