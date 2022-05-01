// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands.Implementations;

/// <summary>
/// Implementation of <see cref="ICommandDispatcher" /> that uses the default dependency injection
/// container as a service locator.
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    readonly IServiceProvider _serviceProvider;

    public CommandDispatcher( IServiceProvider serviceProvider )
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException( nameof(serviceProvider) );
    }

    public async Task<TResult> Execute<TResult>( Command<TResult> command, CancellationToken cancellationToken )
    {
        if ( command == null ) throw new ArgumentNullException( nameof(command) );

        var commandType = command.GetType();
        var resultType = typeof(TResult);
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType( commandType, resultType );

        dynamic handler =
            _serviceProvider.GetService( handlerType ) ??
            throw new InvalidOperationException( $"Missing service registration: {handlerType}" );

        return await handler.Execute( (dynamic)command, cancellationToken );
    }
}
