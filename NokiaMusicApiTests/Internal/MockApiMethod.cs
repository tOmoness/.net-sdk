// -----------------------------------------------------------------------
// <copyright file="MockApiMethod.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tests.Internal
{
    public class MockApiMethod : ApiMethod
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
    }
}
