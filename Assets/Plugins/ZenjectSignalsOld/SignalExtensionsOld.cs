using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public static class SignalExtensionsOld
    {
        public static SignalBinderWithId DeclareSignalOld<T>(this DiContainer container)
            where T : ISignalBase
        {
            var info = new BindInfo();
            var signalSettings = new SignalSettingsOld();
            container.Bind<T>(info).AsCached().WithArguments(signalSettings, info);
            return new SignalBinderWithId(info, signalSettings);
        }

        public static SignalBinderWithId DeclareSignalOld(this DiContainer container, Type type)
        {
            var info = new BindInfo();
            var signalSettings = new SignalSettingsOld();
            container.Bind(type).AsCached().WithArguments(signalSettings, info);
            return new SignalBinderWithId(info, signalSettings);
        }

        public static SignalHandlerBinderWithId BindSignalOld<TSignal>(this DiContainer container)
            where TSignal : ISignal
        {
            var binder = container.StartBinding();
            return new SignalHandlerBinderWithId(
                container, typeof(TSignal), binder);
        }

        public static SignalHandlerBinderWithId<TParam1> BindSignalOld<TParam1, TSignal>(this DiContainer container)
            where TSignal : ISignal<TParam1>
        {
            var binder = container.StartBinding();
            return new SignalHandlerBinderWithId<TParam1>(
                container, typeof(TSignal), binder);
        }

        public static SignalHandlerBinderWithId<TParam1, TParam2> BindSignalOld<TParam1, TParam2, TSignal>(this DiContainer container)
            where TSignal : ISignal<TParam1, TParam2>
        {
            var binder = container.StartBinding();
            return new SignalHandlerBinderWithId<TParam1, TParam2>(
                container, typeof(TSignal), binder);
        }

        public static SignalHandlerBinderWithId<TParam1, TParam2, TParam3> BindSignalOld<TParam1, TParam2, TParam3, TSignal>(this DiContainer container)
            where TSignal : ISignal<TParam1, TParam2, TParam3>
        {
            var binder = container.StartBinding();
            return new SignalHandlerBinderWithId<TParam1, TParam2, TParam3>(
                container, typeof(TSignal), binder);
        }

        public static SignalHandlerBinderWithId<TParam1, TParam2, TParam3, TParam4> BindSignalOld<TParam1, TParam2, TParam3, TParam4, TSignal>(this DiContainer container)
            where TSignal : ISignal<TParam1, TParam2, TParam3, TParam4>
        {
            var binder = container.StartBinding();
            return new SignalHandlerBinderWithId<TParam1, TParam2, TParam3, TParam4>(
                container, typeof(TSignal), binder);
        }
    }
}
