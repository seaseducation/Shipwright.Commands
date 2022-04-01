// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

using Moq;
using Xunit;

namespace Shipwright.Commands.Decorators;

public class CommandCancellationDecoratorTests
{
    readonly Mock<ICommandHandler<FakeCommand, Guid>> mockHandler = new( MockBehavior.Strict );
    ICommandHandler<FakeCommand, Guid> handler;
    ICommandHandler<FakeCommand, Guid> instance() => new CommandCancellationDecorator<FakeCommand, Guid>( handler );

    protected CommandCancellationDecoratorTests()
    {
        handler = mockHandler.Object;
    }

    public class Constructor : CommandCancellationDecoratorTests
    {
        [Fact]
        public void requires_handler()
        {
            handler = null!;
            Assert.Throws<ArgumentNullException>( nameof(handler), instance );
        }
    }

    public class Execute : CommandCancellationDecoratorTests
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

        [Fact]
        public async Task throws_when_canceled()
        {
            cancellationToken = new( true );
            await Assert.ThrowsAsync<OperationCanceledException>( method );
        }

        [Fact]
        public async Task executes_handler_and_returns_result_when_not_canceled()
        {
            cancellationToken = new( false );

            var expected = Guid.NewGuid();
            mockHandler.Setup( _ => _.Execute( command, cancellationToken ) ).ReturnsAsync( expected );

            var actual = await method();
            Assert.Equal( expected, actual );
        }
    }
}
