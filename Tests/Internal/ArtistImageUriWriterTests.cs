// -----------------------------------------------------------------------
// <copyright file="ArtistImageUriWriterTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MixRadio.Internal;
using NUnit.Framework;

namespace MixRadio.Tests.Internal
{
    [TestFixture]
    public class ArtistImageUriWriterTests
    {
        private static IMusicClientSettings Settings
        {
            get
            {
                return new MockMusicClientSettings("ClientId", "gb", "en") { ApiBaseUrl = "http://www.testme.com/" };
            }
        }

        [Test]
        public void BuildArtistIdUrl()
        {
            var artistId = "12354";
            var result = new ArtistImageUriWriter(ArtistImageUriWriterTests.Settings).BuildForId(artistId, 200, 200);
            Assert.AreEqual("http://www.testme.com/1.x/gb/creators/images/200x200/random/?domain=music&client_id=ClientId&lang=en&id=12354", result.AbsoluteUri);
        }

        [Test]
        public void BuildArtistNameUrl()
        {
            var artistName = "Muse";
            var result = new ArtistImageUriWriter(ArtistImageUriWriterTests.Settings).BuildForName(artistName, 200, 200);
            Assert.AreEqual("http://www.testme.com/1.x/gb/creators/images/200x200/random/?domain=music&client_id=ClientId&lang=en&name=Muse", result.AbsoluteUri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureExceptionThrownForNullSettings()
        {
            new ArtistImageUriWriter(null);
        }
    }
}
