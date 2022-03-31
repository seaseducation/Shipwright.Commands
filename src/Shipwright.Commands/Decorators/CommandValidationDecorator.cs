// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using FluentValidation;

namespace Shipwright.Commands.Decorators;

/// <summary>
/// Decorates a command handler to validate a command before executing.
/// </summary>
/// <typeparam name="TCommand">Type of the commands whose handler is decorated.</typeparam>
/// <typeparam name="TResult">Type returned by executing a command.</typeparam>
public class CommandValidationDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : Command<TResult>
{
    readonly ICommandHandler<TCommand, TResult> _handler;
    readonly IValidator<TCommand> _validator;

    public CommandValidationDecorator( ICommandHandler<TCommand, TResult> handler, IValidator<TCommand> validator )
    {
        _handler = handler ?? throw new ArgumentNullException( nameof(handler) );
        _validator = validator ?? throw new ArgumentNullException( nameof(validator) );
    }

    public async Task<TResult> Execute( TCommand command, CancellationToken cancellationToken )
    {
        if ( command == null ) throw new ArgumentNullException( nameof(command) );

        var result = await _validator.ValidateAsync( command, cancellationToken );

        if ( !result.IsValid )
            throw new ValidationException( $"Failed to validate command of type {typeof(TCommand)}", result.Errors );

        return await _handler.Execute( command, cancellationToken );
    }
}
