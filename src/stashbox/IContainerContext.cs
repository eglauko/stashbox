﻿using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public interface IContainerContext
    {
        /// <summary>
        /// The repository of the registrations.
        /// </summary>
        IRegistrationRepository RegistrationRepository { get; }

        /// <summary>
        /// The decorator repository.
        /// </summary>
        IDecoratorRepository DecoratorRepository { get; }

        /// <summary>
        /// The container itself.
        /// </summary>
        IStashboxContainer Container { get; }

        /// <summary>
        /// The resolution strategy.
        /// </summary>
        IResolutionStrategy ResolutionStrategy { get; }

        /// <summary>
        /// Indicates that the container should track transient objects for disposal or not.
        /// </summary>
        IContainerConfigurator ContainerConfigurator { get; }
    }
}
