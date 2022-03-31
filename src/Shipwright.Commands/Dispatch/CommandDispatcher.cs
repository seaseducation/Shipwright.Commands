// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using Lamar;
using Lamar.IoC;

namespace Shipwright.Commands.Dispatch;

/// <summary>
/// Dispatcher for locating and executing command handlers.
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    readonly IServiceContext _serviceContext;

    public CommandDispatcher( IServiceContext serviceContext )
    {
        _serviceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
    }

    public async Task<TResult> Execute<TResult>( Command<TResult> command, CancellationToken cancellationToken )
    {
        if ( command == null ) throw new ArgumentNullException( nameof(command) );

        // determine handler type
        var commandType = command.GetType();
        var resultType = typeof(TResult);
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType( commandType, resultType );

        // request handler from container context
        // exception thrown should be redundant, but we check just in case
        dynamic handler =
            _serviceContext.GetInstance( handlerType ) ??
            throw new LamarMissingRegistrationException( handlerType );

        // use of the dynamic type offloads the complex reflection, expression tree caching,
        // and delegate compilation to the DLR. this results in reflection overhead only applying
        // to the first call; subsequent calls perform similar to statically-compiled statements.

        return await handler.Execute( (dynamic)command, cancellationToken );
    }
}
