# stashbox [![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/stashbox-0vuru/master.svg)](https://ci.appveyor.com/project/pcsajtai/stashbox-0vuru/build/tests) [![Coverage Status](https://img.shields.io/codecov/c/github/z4kn4fein/stashbox.svg)](https://codecov.io/gh/z4kn4fein/stashbox) [![Quality Gate](https://sonarqube.com/api/badges/gate?key=stashbox)](https://sonarcloud.io/dashboard?id=stashbox%3Astashbox%3A21317234-0499-4178-BCA5-558FBA2CB9AC)

Stashbox is a lightweight, portable dependency injection framework for .NET based solutions. [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/gitter/room/z4kn4fein/stashbox.svg)](https://gitter.im/z4kn4fein/stashbox?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Slack](https://img.shields.io/badge/chat-on%20slack-orange.svg?style=flat)](https://stashbox-slack-in.herokuapp.com/)

Github (stable) | NuGet (stable) | MyGet (pre-release) 
--- | --- | ---
[![Github release](https://img.shields.io/github/release/z4kn4fein/stashbox.svg)](https://github.com/z4kn4fein/stashbox/releases) | [![NuGet Version](https://buildstats.info/nuget/Stashbox)](https://www.nuget.org/packages/Stashbox/) | [![MyGet package](https://img.shields.io/myget/pcsajtai/v/Stashbox.svg?label=myget)](https://www.myget.org/feed/pcsajtai/package/nuget/Stashbox)

## Features

 - **Fluent interface** - for faster and easier configuration, attributes also can be used.
 - **Interface/type mapping** - single service, instance registration, remapping of existing registrations also supported.
 - **Resolution via delegates** - any number of parameters can be injected, they will be reused for subdenpendency resolution as well.
 - **Named registration** - multiple registrations with the same interface type.
 - **Assembly registration** - service lookup in assemblies, composition root implementations also supported.
 - **Factory registration** - factories with several parameters can be registered.
 - **Initializer / finalizer** - custom initializer and finalizer actions can be set.
 - **Multiple service resolution** - all implementation registered for an interface can be obtained.
 - **Unknown type resolution** - unregistered services can be resolved or injected.
 - **Default and optional value injection** - primitive types or dependencies with default or optional value can be injected.
 - **Open generic type resolution** - concrete generic types can be resolved from open generic definitions, constraint checking and nested generic definitions are checked.
 - **Constructor, property and field injection** - supports attribute driven injection and auto injection as well, where there is no chance to decorate members with attributes.
 - **Injection method** - methods decorated with `InjectionMethod` attribute will be called at resolution time.
 - **Wiring into container** - member injection can be executed on existing instances.
 - **Building up existing instance** - member injection can be executed on existing instance without registering it into the container.
 - **Child scopes** - parent/child container support.
 - **Lifetime scopes** - hierarchical/named scoping support.
 - **Lifetime management** - including `Singleton`, `Transient`, `Scoped`, `NamedScope` and `PerResolutionRequest` lifetime, custom user defined lifetimes can also be used.
 - **Conditional resolution** - attribute, parent-type and custom user defined conditions can be specified.
 - **IDisposable object tracking** - `IDisposable` objects are being disposed by the container.
 - **Cleanup delegates** - custom delegate can be configured which'll be called when the container/scope is being disposed.
 - **Circular dependency tracking** - the container checks the dependency graph for circular dependencies, specific excpetion will be thrown if any found.
 - **Special types** - generic wrappers:
     - Collections: everything assignable to `IEnumerable<T>` e.g. `T[]`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IList<T>` etc.
     - `Lazy<>`, `Func<>`, `Tuple<>`
     - Parameter injection over factory method arguments e.g. `Func<TParam, TService>`, `Func<TParam1, TParam2, TService>`, etc. applied to subdependencies as well.
     - Nested wrappers e.g. `Tuple<TService, IEnumerable<Func<TParam, Lazy<TService1>>>>`.
 - **Custom resolvers** - the existing activation rutines can be extended with custom resolvers.
 - **Container extensions** - the container's functionality can be extended with custom extensions, e.g. [Auto configuration parser extension](https://github.com/z4kn4fein/stashbox-configuration-extension)
 - **Custom configuration** - the behavior of the container can be controlled with custom configurations.
 - **Container validation** - the resolution graph can be validated by calling the `Validate()` function.
 - **Decorator support / Interception** - service decorators can be registered and also can used for interception with Castle DynamicProxy

## Supported platforms

 - .NET 4.0 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 - .NET Standard 1.0
 - .NET Standard 1.3
 - .NET Standard 2.0

## Sample usage
```c#
class Wulfgar : IBarbarian
{
    public Wulfgar(IWeapon weapon)
    {
        //...
    }
}

var container = new StashboxContainer();

container.RegisterType<IWeapon, AegisFang>();
container.RegisterType<IBarbarian, Wulfgar>();

var wulfgar = container.Resolve<IBarbarian>();
```
## Extensions
 - [Decorator extension](https://github.com/z4kn4fein/stashbox-decoratorextension) (obsolate, the decorator pattern support is a built-in feature from version 2.3.0)
 - [Stashbox.Web.WebApi](https://github.com/z4kn4fein/stashbox-web-webapi)
 - [Stashbox.AspNet.WebApi.Owin](https://github.com/z4kn4fein/stashbox-webapi-owin)
 - [Stashbox.Web.Mvc](https://github.com/z4kn4fein/stashbox-web-mvc)
 - [Stashbox.AspNet.SingalR](https://github.com/z4kn4fein/stashbox-signalr)
 - [Stashbox.AspNet.SingalR.Owin](https://github.com/z4kn4fein/stashbox-signalr-owin)
 - [Stashbox.Owin](https://github.com/z4kn4fein/stashbox-owin)
 - [Stashbox.Extension.Wcf](https://github.com/devworker55/stashbox-extension-wcf)
 - [Stashbox.Extensions.Dependencyinjection](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection)
     - [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) adapter for ASP.NET Core.
     - [Microsoft.AspNetCore.Hosting](https://github.com/aspnet/Hosting) `IWebHostBuilder` extension.
 - [Stashbox.Mocking](https://github.com/z4kn4fein/stashbox-mocking) (Moq, FakeItEasy, NSubstitute, RhinoMocks, Rocks)
 - [Stashbox.Configuration](https://github.com/z4kn4fein/stashbox-configuration-extension) auto configuration parser

## Documentation
 - [Wiki](https://github.com/z4kn4fein/stashbox/wiki)
 - [ASP.NET Core sample](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection/tree/master/sample)
 
## Benchmarks
 - [Performance](http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison)
 - [Feature](http://featuretests.apphb.com/DependencyInjection.html)

<br/>

*Powered by [Jetbrains'](https://www.jetbrains.com) [Open Source License](https://www.jetbrains.com/community/opensource)*

[![Jetbrains](https://cdn.rawgit.com/z4kn4fein/stashbox/dev/img/jetbrains.svg)](https://www.jetbrains.com)
