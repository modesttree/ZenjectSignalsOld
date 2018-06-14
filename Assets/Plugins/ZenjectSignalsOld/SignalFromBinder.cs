using System;
using System.Collections.Generic;

#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // We wrap FromBinderGeneric because some things in there that don't really work for signals
    // like CopyInSubContainers, conditions, subcontainers
    public class SignalFromBinder<TContract> : ScopeConcreteIdArgNonLazyBinder
    {
        readonly BindInfo _info;
        readonly FromBinderGeneric<TContract> _subBinder;

        public SignalFromBinder(
            BindInfo info, FromBinderGeneric<TContract> subBinder)
            : base(info)
        {
            _info = info;
            _subBinder = subBinder;
        }

        //////////////// FromBinderGeneric ////////////////

        public ScopeConcreteIdArgNonLazyBinder FromMethod(Func<InjectContext, TContract> method)
        {
            _subBinder.FromMethod(method);
            return this;
        }

        public ScopeConcreteIdArgNonLazyBinder FromMethodMultiple(
            Func<InjectContext, IEnumerable<TContract>> method)
        {
            _subBinder.FromMethodMultiple(method);
            return this;
        }

        public ScopeNonLazyBinder FromResolveGetter<TObj>(Func<TObj, TContract> method)
        {
            _subBinder.FromResolveGetter<TObj>(method);
            return new ScopeNonLazyBinder(_info);
        }

        public ScopeNonLazyBinder FromResolveGetter<TObj>(object identifier, Func<TObj, TContract> method)
        {
            _subBinder.FromResolveGetter<TObj>(identifier, method);
            return new ScopeNonLazyBinder(_info);
        }

        public ScopeNonLazyBinder FromInstance(TContract instance)
        {
            _subBinder.FromInstance(instance);
            return new ScopeNonLazyBinder(_info);
        }

#if !NOT_UNITY3D

        // These ones don't make sense for signals
        //ScopeConcreteIdArgNonLazyBinder FromComponentInChildren()
        //ScopeArgConditionCopyNonLazyBinder FromComponentInParents()
        //ScopeArgConditionCopyNonLazyBinder FromComponentSibling()

        public ScopeConcreteIdArgNonLazyBinder FromComponentInHierarchy()
        {
            _subBinder.FromComponentInHierarchy();
            return this;
        }
#endif


        //////////////// FromBinder ////////////////

        // This is the default if nothing else is called
        public ScopeConcreteIdArgNonLazyBinder FromNew()
        {
            _subBinder.FromNew();
            return this;
        }

        public ScopeNonLazyBinder FromResolve()
        {
            _subBinder.FromResolve();
            return new ScopeNonLazyBinder(_info);
        }

        public ScopeNonLazyBinder FromResolve(object subIdentifier)
        {
            _subBinder.FromResolve(subIdentifier);
            return new ScopeNonLazyBinder(_info);
        }

        // TODO
        //public SubContainerBinder FromSubContainerResolve()
        //public SubContainerBinder FromSubContainerResolve(object subIdentifier)

#if !NOT_UNITY3D

        public ScopeConcreteIdArgNonLazyBinder FromNewComponentOn(GameObject gameObject)
        {
            _subBinder.FromNewComponentOn(gameObject);
            return this;
        }

        // This one doesn't make sense for signals
        //public ArgNonLazyBinder FromNewComponentSibling()

        public NameTransformScopeConcreteIdArgNonLazyBinder FromNewComponentOnNewPrefab(UnityEngine.Object prefab)
        {
            var gameObjectInfo = new GameObjectCreationParameters();
            _subBinder.FromNewComponentOnNewPrefab(prefab, gameObjectInfo);
            return new NameTransformScopeConcreteIdArgNonLazyBinder(_info, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgNonLazyBinder FromNewComponentOnNewPrefabResource(string resourcePath)
        {
            var gameObjectInfo = new GameObjectCreationParameters();
            _subBinder.FromNewComponentOnNewPrefabResource(resourcePath, gameObjectInfo);
            return new NameTransformScopeConcreteIdArgNonLazyBinder(_info, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgNonLazyBinder FromNewComponentOnNewGameObject()
        {
            var gameObjectInfo = new GameObjectCreationParameters();
            _subBinder.FromNewComponentOnNewGameObject(gameObjectInfo);
            return new NameTransformScopeConcreteIdArgNonLazyBinder(_info, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgNonLazyBinder FromComponentInNewPrefab(UnityEngine.Object prefab)
        {
            var gameObjectInfo = new GameObjectCreationParameters();
            _subBinder.FromComponentInNewPrefab(prefab, gameObjectInfo);
            return new NameTransformScopeConcreteIdArgNonLazyBinder(_info, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgNonLazyBinder FromComponentInNewPrefabResource(string resourcePath)
        {
            var gameObjectInfo = new GameObjectCreationParameters();
            _subBinder.FromComponentInNewPrefabResource(resourcePath, gameObjectInfo);
            return new NameTransformScopeConcreteIdArgNonLazyBinder(_info, gameObjectInfo);
        }

        public ScopeConcreteIdArgNonLazyBinder FromNewScriptableObjectResource(string resourcePath)
        {
            _subBinder.FromNewScriptableObjectResource(resourcePath);
            return this;
        }

        public ScopeConcreteIdArgNonLazyBinder FromScriptableObjectResource(string resourcePath)
        {
            _subBinder.FromScriptableObjectResource(resourcePath);
            return this;
        }

        public ScopeNonLazyBinder FromResource(string resourcePath)
        {
            _subBinder.FromResource(resourcePath);
            return new ScopeNonLazyBinder(_info);
        }

#endif

        public ScopeConcreteIdArgNonLazyBinder FromMethodUntyped(Func<InjectContext, object> method)
        {
            _subBinder.FromMethodUntyped(method);
            return this;
        }
    }
}
