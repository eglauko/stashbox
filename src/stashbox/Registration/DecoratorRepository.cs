﻿using Stashbox.Entity;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a decorator repository.
    /// </summary>
    public class DecoratorRepository : IDecoratorRepository
    {
        private AvlTreeKeyValue<Type, ArrayStoreKeyed<Type, IServiceRegistration>> repository;

        /// <summary>
        /// Constructs a <see cref="DecoratorRepository"/>.
        /// </summary>
        public DecoratorRepository()
        {
            this.repository = AvlTreeKeyValue<Type, ArrayStoreKeyed<Type, IServiceRegistration>>.Empty;
        }

        /// <inheritdoc />
        public void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool remap, bool replace)
        {
            var newRepository = new ArrayStoreKeyed<Type, IServiceRegistration>(serviceRegistration.ImplementationType, serviceRegistration);

            if (remap)
                Swap.SwapValue(ref this.repository, repo => repo.AddOrUpdate(type, newRepository, (oldValue, newValue) => newValue));
            else
                Swap.SwapValue(ref this.repository, repo => repo.AddOrUpdate(type, newRepository, (oldValue, newValue) => oldValue
                    .AddOrUpdate(serviceRegistration.ImplementationType, serviceRegistration, replace)));
        }

        /// <inheritdoc />
        public KeyValue<Type, IServiceRegistration>[] GetDecoratorsOrDefault(Type type) =>
             this.repository.GetOrDefault(type)?.Repository;
    }
}
