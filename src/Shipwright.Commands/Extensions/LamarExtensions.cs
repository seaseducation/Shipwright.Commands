// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using Lamar;
using Lamar.Scanning.Conventions;
using Shipwright.Commands;
using Shipwright.Commands.Decorators;
using Shipwright.Commands.Dispatch;

// ReSharper disable once CheckNamespace
namespace Shipwright;

public static class LamarExtensions
{
    /// <summary>
    /// Registers the Shipwright command dispatcher.
    /// </summary>
    /// <param name="registry">Lamar service registry.</param>
    public static ServiceRegistry AddShipwrightCommandDispatch( this ServiceRegistry registry )
    {
        if ( registry == null ) throw new ArgumentNullException( nameof(registry) );
        registry.For<ICommandDispatcher>().Add<CommandDispatcher>();
        return registry;
    }

    /// <summary>
    /// Registers the Shipwright command cancellation decorator.
    /// </summary>
    /// <param name="registry">Lamar service registry.</param>
    public static ServiceRegistry AddShipwrightCommandCancellation( this ServiceRegistry registry )
    {
        if ( registry == null ) throw new ArgumentNullException( nameof(registry) );
        registry.For( typeof(ICommandHandler<,>) ).DecorateAllWith( typeof(CommandCancellationDecorator<,>) );
        return registry;
    }

    /// <summary>
    /// Registers the Shipwright command validation decorator.
    /// </summary>
    /// <param name="registry">Lamar service registry.</param>
    public static ServiceRegistry AddShipwrightCommandValidation( this ServiceRegistry registry )
    {
        if ( registry == null ) throw new ArgumentNullException( nameof(registry) );
        registry.For( typeof(ICommandHandler<,>) ).DecorateAllWith( typeof(CommandValidationDecorator<,>) );
        return registry;
    }

    /// <summary>
    /// Locates all command handlers in the scanned assemblies.
    /// </summary>
    /// <param name="scanner">Lamar assembly scanner.</param>
    public static IAssemblyScanner AddCommandHandlers( this IAssemblyScanner scanner )
    {
        if ( scanner == null ) throw new ArgumentNullException( nameof(scanner) );
        scanner.ConnectImplementationsToTypesClosing( typeof(ICommandHandler<,>) );
        return scanner;
    }
}
