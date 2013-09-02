﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Communication;
using NUnit.Framework;
using SDK.Test.Helper;

namespace SDK.Test.Communication
{
    internal class TestBaseRequestHandler : BaseRequestHandler
    {
        private readonly IWebRequestFactory _webRequestFactory;

        public TestBaseRequestHandler(IWebRequestFactory webRequestFactory)
        {
            _webRequestFactory = webRequestFactory;
        }

        public void MakeRequest(string uri, Action<HttpStatusCode, string, UserContract> onSuccess, Action<Uri, Exception> onError)
        {
            var request = _webRequestFactory.Create(new Uri(uri));

            request.BeginGetResponse(HandleResponseCallback<RequestContext<string, UserContract>, string, UserContract>, new RequestContext<string, UserContract>() { Request = request, OnComplete = onSuccess, OnError = onError });
        }

        async public Task<Tuple<HttpStatusCode, UserContract>> MakeRequestAsync(string uri)
        {
            var request = _webRequestFactory.Create(new Uri(uri));
            var get = await GetResponseTask<UserContract>(request);
            return new Tuple<HttpStatusCode, UserContract>(get.Item1, get.Item3);
        }
    }

    [TestFixture]
    public class BaseRequestHandlerTests
    {
        [Test]
        public void CallOnSuccessIfNoError()
        {
            // arrange
            var uri = "http://localhost/";
            var factory = new TestWebRequestFactory();
            factory.RegisterResultForUri(uri, "null");
            var handler = new TestBaseRequestHandler(factory);

            // act
            var data = new object();
            handler.MakeRequest(uri, (code, location, response) => data = response, null);

            // assert
            Assert.IsNull(data);
        }

        [Test]
        public void CallOnErrorIfError()
        {
            // arrange
            var uri = "http://localhost/";
            var factory = new TestWebRequestFactory();
            factory.RegisterExceptionForUri<WebException>(uri);
            var handler = new TestBaseRequestHandler(factory);

            // act
            var data = new object();
            handler.MakeRequest(uri, null, (exuri, exception) => data = exception);

            // assert
            Assert.IsNotNull(data);
        }

        [Test]
        public void CanExtractJsonEntity()
        {
            // arrange
            var uri = "http://localhost/";
            var factory = new TestWebRequestFactory();
            factory.RegisterResultForUri(uri, "{ \"Name\": \"Paul\" }");
            var handler = new TestBaseRequestHandler(factory);

            // act
            UserContract data = null;
            handler.MakeRequest(uri, (code, location, response) => data = response, null);

            // assert
            Assert.IsNotNull(data);
            Assert.AreEqual("Paul", data.Name);
        }

        [Test]
        public void CanExtractCompressedJsonEntity()
        {
            // arrange
            var uri = "http://localhost/";
            var factory = new TestWebRequestFactory();
            factory.RegisterCompressedResultForUri(uri, "{ \"Name\": \"Paul\" }");
            var handler = new TestBaseRequestHandler(factory);

            // act
            UserContract data = null;
            handler.MakeRequest(uri, (code, location, response) => data = response, null);

            // assert
            Assert.IsNotNull(data);
            Assert.AreEqual("Paul", data.Name);
        }

        [Test]
        async public void CanExtractJsonEntityAsync()
        {
            // arrange
            var uri = "http://localhost/";
            var factory = new TestWebRequestFactory();
            factory.RegisterResultForUri(uri, "{ \"Name\": \"Paul\" }");
            var handler = new TestBaseRequestHandler(factory);

            // act
            var res = await handler.MakeRequestAsync(uri);

            // assert
            Assert.IsNotNull(res.Item2);
            Assert.AreEqual("Paul", res.Item2.Name);
        }

        [Test]
        async public void CanExtractCompressedJsonEntityAsync()
        {
            // arrange
            var uri = "http://localhost/";
            var factory = new TestWebRequestFactory();
            factory.RegisterCompressedResultForUri(uri, "{ \"Name\": \"Paul\" }");
            var handler = new TestBaseRequestHandler(factory);

            // act
            var res = await handler.MakeRequestAsync(uri);

            // assert
            Assert.IsNotNull(res.Item2);
            Assert.AreEqual("Paul", res.Item2.Name);
        }
    }
}
