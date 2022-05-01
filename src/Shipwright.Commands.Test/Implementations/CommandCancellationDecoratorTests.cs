// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands.Implementations;

public abstract class CommandCancellationDecoratorTests
{
    readonly Mock<ICommandHandler<FakeCommand, Guid>> mockInner = new( MockBehavior.Strict );

    ICommandHandler<FakeCommand, Guid> inner;
    ICommandHandler<FakeCommand, Guid> instance() => new CommandCancellationDecorator<FakeCommand, Guid>( inner );

    protected CommandCancellationDecoratorTests()
    {
        inner = mockInner.Object;
    }

    public class Constructor : CommandCancellationDecoratorTests
    {
        [Fact]
        public void requires_inner()
        {
            inner = null!;
            Assert.Throws<ArgumentNullException>( nameof(inner), instance );
        }
    }

    public abstract class Execute : CommandCancellationDecoratorTests
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

        public class WhenCanceled : Execute
        {
            [Fact]
            public async Task throws_operation_canceled()
            {
                cancellationToken = new( true );
                await Assert.ThrowsAsync<OperationCanceledException>( method );
            }
        }

        public class WhenNotCanceled : Execute
        {
            [Fact]
            public async Task executes_inner_handler_and_returns_result()
            {
                cancellationToken = new( false );

                var expected = Guid.NewGuid();
                mockInner.Setup( _ => _.Execute( command, cancellationToken ) ).ReturnsAsync( expected );

                var actual = await method();
                Assert.Equal( expected, actual );
            }
        }
    }
}
