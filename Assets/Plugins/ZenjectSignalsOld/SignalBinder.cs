using System;
using System.Collections.Generic;

namespace Zenject
{
    public class SignalBinder : ConditionCopyNonLazyBinder
    {
        readonly SignalSettingsOld _signalSettings;

        public SignalBinder(
            BindInfo bindInfo, SignalSettingsOld signalSettings)
            : base(bindInfo)
        {
            _signalSettings = signalSettings;
        }

        public ConditionCopyNonLazyBinder RequireHandler()
        {
            _signalSettings.RequiresHandler = true;
            return this;
        }
    }

    public class SignalBinderWithId : SignalBinder
    {
        public SignalBinderWithId(
            BindInfo bindInfo, SignalSettingsOld signalSettings)
            : base(bindInfo, signalSettings)
        {
        }

        public SignalBinder WithId(object identifier)
        {
            this.BindInfo.Identifier = identifier;
            return this;
        }
    }
}
