using System;
using ModestTree;

namespace Zenject
{
    public static class SignalExtensions
    {
        public static DeclareSignalRequireHandlerAsyncCopyBinder DeclareSignal<TSignal>(this DiContainer container)
        {
            var signalBindInfo = new SignalDeclarationBindInfo(typeof(TSignal));
            signalBindInfo.RunAsync = container.Settings.Signals.DefaultSyncMode == SignalDefaultSyncModes.Asynchronous;
            signalBindInfo.MissingHandlerResponse = container.Settings.Signals.MissingHandlerDefaultResponse;
            var bindInfo = new BindInfo();
            container.Bind<SignalDeclarationBindInfo>(bindInfo)
                .FromInstance(signalBindInfo).WhenInjectedInto<SignalBus>();
            return new DeclareSignalRequireHandlerAsyncCopyBinder(signalBindInfo, bindInfo);
        }

        public static BindSignalToBinder<TSignal> BindSignal<TSignal>(this DiContainer container)
        {
            return new BindSignalToBinder<TSignal>(container);
        }
    }
}
