// -----------------------------------------------------------------------
// <copyright file="MockApiCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal.Request;

namespace Nokia.Music.Phone.Tests.Internal
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
