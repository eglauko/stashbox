﻿using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                if (serviceRegistration.ShouldHandleDisposal && serviceRegistration.RegistrationContext.ExistingInstance is IDisposable disposable)
                    resolutionContext.RootScope.AddDisposableTracking(disposable);

                if (serviceRegistration.RegistrationContext.Finalizer != null)
                {
                    var finalizerExpression = base.HandleFinalizer(serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(),
                        serviceRegistration, resolutionContext.CurrentScopeParameter.Prop(Constants.RootScopeProperty));
                    return this.expression = finalizerExpression.CompileDelegate(resolutionContext)(resolutionContext.ResolutionScope).AsConstant();
                }

                return this.expression = serviceRegistration.RegistrationContext.ExistingInstance.AsConstant();
            }
        }

        public override IObjectBuilder Produce() => new InstanceObjectBuilder();

        public override bool HandlesObjectLifecycle => true;
    }
}