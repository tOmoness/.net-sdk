// -----------------------------------------------------------------------
// <copyright file="MockApiCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Commands;
using Nokia.Music.Internal.Request;

namespace Nokia.Music.Tests.Internal
{
    internal class MockApiCommand : MusicClientCommand
    {
        internal override HttpMethod HttpMethod
        {
            get
            {
                return HttpMethod.Post;
            }
        }

        internal override string ContentType
        {
            get 
            { 
                return "text/xml";
            }
        }

        internal override string BuildRequestBody()
        {
            return string.Empty;
        }

        protected override void Execute()
        {
        }
    }
}
