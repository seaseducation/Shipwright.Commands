// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Shipwright.Commands.Decorators;

public class CommandValidationDecoratorTests
{
    readonly Mock<ICommandHandler<FakeCommand, Guid>> mockHandler = new( MockBehavior.Strict );
    readonly Mock<IValidator<FakeCommand>> mockValidator = new( MockBehavior.Strict );
    ICommandHandler<FakeCommand, Guid> handler;
    IValidator<FakeCommand> validator;
    ICommandHandler<FakeCommand, Guid> instance() => new CommandValidationDecorator<FakeCommand, Guid>( handler, validator );

    public CommandValidationDecoratorTests()
    {
        handler = mockHandler.Object;
        validator = mockValidator.Object;
    }

    public class Constructor : CommandValidationDecoratorTests
    {
        [Fact]
        public void requires_handler()
        {
            handler = null!;
            Assert.Throws<ArgumentNullException>( nameof(handler), instance );
        }

        [Fact]
        public void requires_validator()
        {
            validator = null!;
            Assert.Throws<ArgumentNullException>( nameof(validator), instance );
        }
    }

    public class Execute : CommandValidationDecoratorTests
    {
        FakeCommand command = new();
        CancellationToken cancellationToken;
        Task<Guid> method() => instance().Execute( command, cancellationToken );

        [Fact]
        public async Task requires_command()
        {
            command = null!;
            await Assert.ThrowsAsync<ArgumentNullException>( nameof(command), method );
        }

        [Theory]
        [InlineData( true )]
        [InlineData( false )]
        public async Task throws_when_validation_fails( bool canceled )
        {
            cancellationToken = new( canceled );

            var errors = new Fixture().CreateMany<ValidationFailure>().ToArray();
            var result = new ValidationResult( errors );
            mockValidator.Setup( _ => _.ValidateAsync( command, cancellationToken ) ).ReturnsAsync( result );

            var ex = await Assert.ThrowsAsync<ValidationException>( method );
            Assert.Equal( errors, ex.Errors );
        }

        [Theory]
        [InlineData( true )]
        [InlineData( false )]
        public async Task executes_handler_and_returns_result_when_valid( bool canceled )
        {
            cancellationToken = new( canceled );
            var errors = Array.Empty<ValidationFailure>();
            var result = new ValidationResult( errors );
            mockValidator.Setup( _ => _.ValidateAsync( command, cancellationToken ) ).ReturnsAsync( result );

            var expected = Guid.NewGuid();
            mockHandler.Setup( _ => _.Execute( command, cancellationToken ) ).ReturnsAsync( expected );

            var actual = await method();
            Assert.Equal( expected, actual );
        }
    }
}
