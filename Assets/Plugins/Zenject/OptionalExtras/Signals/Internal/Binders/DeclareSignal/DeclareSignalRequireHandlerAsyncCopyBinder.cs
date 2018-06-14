using System;
using ModestTree;

namespace Zenject
{
    public class DeclareSignalRequireHandlerAsyncCopyBinder : DeclareSignalAsyncCopyBinder
    {
        public DeclareSignalRequireHandlerAsyncCopyBinder(
            SignalDeclarationBindInfo signalBindInfo, BindInfo bindInfo)
            : base(signalBindInfo, bindInfo)
        {
        }

        public DeclareSignalAsyncCopyBinder RequireSubscriber()
        {
            SignalBindInfo.MissingHandlerResponse = SignalMissingHandlerResponses.Throw;
            return this;
        }

        public DeclareSignalAsyncCopyBinder OptionalSubscriber()
        {
            SignalBindInfo.MissingHandlerResponse = SignalMissingHandlerResponses.Ignore;
            return this;
        }

        public DeclareSignalAsyncCopyBinder OptionalSubscriberWithWarning()
        {
            SignalBindInfo.MissingHandlerResponse = SignalMissingHandlerResponses.Warn;
            return this;
        }
    }
}

