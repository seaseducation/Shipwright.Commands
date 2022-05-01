// SPDX-License-Identifier: Apache-2.0
// Copyright (c) TTCO Holding Company, Inc. and Contributors
// Licensed under the Apache License, Version 2.0
// See https://opensource.org/licenses/Apache-2.0 or the LICENSE file in the repository root for the full text of the license.

namespace Shipwright.Commands.Implementations;

public class CommandDispatcherTests
{
    readonly Mock<IServiceProvider> mockServiceProvider = new( MockBehavior.Strict );

    IServiceProvider serviceProvider;
    ICommandDispatcher instance() => new CommandDispatcher( serviceProvider );

    protected CommandDispatcherTests()
    {
        serviceProvider = mockServiceProvider.Object;
    }

    public class Constructor : CommandDispatcherTests
    {
        [Fact]
        public void requires_serviceProvider()
        {
            serviceProvider = null!;
            Assert.Throws<ArgumentNullException>( nameof(serviceProvider), instance );
        }
    }

    public class Execute : CommandDispatcherTests
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
        public async Task throws_when_handler_not_registered( bool canceled )
        {
            cancellationToken = new( canceled );
            mockServiceProvider.Setup( _ => _.GetService( typeof(ICommandHandler<FakeCommand, Guid>) ) ).Returns( null );

            await Assert.ThrowsAsync<InvalidOperationException>( method );
        }

        [Theory]
        [InlineData( true )]
        [InlineData( false )]
        public async Task executes_handler_and_returns_result_when_handler_registered( bool canceled )
        {
            cancellationToken = new( canceled );

            var expected = Guid.NewGuid();
            var mockHandler = new Mock<ICommandHandler<FakeCommand, Guid>>( MockBehavior.Strict );
            mockHandler.Setup( _ => _.Execute( command, cancellationToken ) ).ReturnsAsync( expected );
            mockServiceProvider.Setup( _ => _.GetService( typeof(ICommandHandler<FakeCommand, Guid>) ) ).Returns( mockHandler.Object );

            var actual = await method();
            Assert.Equal( expected, actual );
        }
    }
}
