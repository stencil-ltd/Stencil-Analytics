using System;
using Scripts.RemoteConfig;
using State.Active;

namespace RemoteConfig
{ 
    public class RemoteGate : ActiveGate
    {
        public string key;
        public bool invert = false;
        public bool defaultFieldValue = false;
        public bool enableInDebug = true;
        public bool listen = true;

        public static bool IsVisible(string key, bool invert = false, bool defaultFieldValue = false, bool enableInDebug = true)
        {
            if (enableInDebug && StencilRemote.IsDeveloper()) return true;
#if !STENCIL_FIREBASE
                return null;
#else
            var retval = StencilRemote.BoolValue(key, defaultFieldValue);
            if (invert) retval = !retval;
            return retval;
#endif
        }

        public override void Register(ActiveManager manager)
        {
            base.Register(manager);
            StencilRemote.OnRemoteConfig += _OnRemote;
        }

        public override void Unregister()
        {
            base.Unregister();
            StencilRemote.OnRemoteConfig -= _OnRemote;
        }

        private void _OnRemote(object sender, EventArgs e)
        {
            if (listen)
                RequestCheck();
        }

        public override bool? Check()
        {
            return IsVisible(key, invert, defaultFieldValue, enableInDebug);
        }
    }
}