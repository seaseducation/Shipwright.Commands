// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands.Decorators;

/// <summary>
/// Decorates a command handler to check for cancellation before executing.
/// </summary>
/// <typeparam name="TCommand">Type of the commands whose handler is decorated.</typeparam>
/// <typeparam name="TResult">Type returned by executing a command.</typeparam>
public class CommandCancellationDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : Command<TResult>
{
    readonly ICommandHandler<TCommand, TResult> _handler;

    public CommandCancellationDecorator( ICommandHandler<TCommand, TResult> handler )
    {
        _handler = handler ?? throw new ArgumentNullException( nameof(handler) );
    }

    public async Task<TResult> Execute( TCommand command, CancellationToken cancellationToken )
    {
        if ( command == null ) throw new ArgumentNullException( nameof(command) );
        cancellationToken.ThrowIfCancellationRequested();
        return await _handler.Execute( command, cancellationToken );
    }
}
