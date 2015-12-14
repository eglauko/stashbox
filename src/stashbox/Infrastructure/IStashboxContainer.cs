﻿using Stashbox.Entity;
using Stashbox.Infrastructure.ContainerExtension;
using System;

namespace Stashbox.Infrastructure
{
    public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver, IDisposable
    {
        void RegisterExtension(IContainerExtension containerExtension);
        void RegisterResolver(Func<IContainerContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory);
        IStashboxContainer CreateChildContainer();
        IStashboxContainer ParentContainer { get; }
    }
}
