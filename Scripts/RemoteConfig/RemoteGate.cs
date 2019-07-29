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
            if (enableInDebug && StencilRemote.IsDeveloper()) return true;
#if EXCLUDE_FIREBASE
                return null;
#else
            var retval = StencilRemote.BoolValue(key, defaultFieldValue);
            if (invert) retval = !retval;
            return retval;
#endif
        }
    }
}