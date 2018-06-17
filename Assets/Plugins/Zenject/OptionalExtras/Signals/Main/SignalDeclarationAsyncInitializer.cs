using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    // This class just exists to solve a circular dependency that would otherwise happen if we
    // attempted to inject TickableManager into either SignalDeclaration or SignalBus
    // And we need to directly depend on TickableManager because we need each SignalDeclaration
    // to have a unique tick priority
    public class SignalDeclarationAsyncInitializer
    {
        public SignalDeclarationAsyncInitializer(
            [Inject(Source = InjectSources.Local)]
            List<SignalDeclaration> signalDeclarations,
            [Inject(Optional = true, Source = InjectSources.Local)]
            TickableManager tickManager)
        {
            for (int i = 0; i < signalDeclarations.Count; i++)
            {
                var declaration = signalDeclarations[i];

                if (declaration.IsAsync)
                {
                    Assert.IsNotNull(tickManager, "TickableManager is required when using asynchronous signals");
                    tickManager.Add(declaration, declaration.TickPriority);
                }
            }
        }
    }
}

