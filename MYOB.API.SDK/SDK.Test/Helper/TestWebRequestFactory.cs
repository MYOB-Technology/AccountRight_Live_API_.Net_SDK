﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Communication;
using NSubstitute;

namespace SDK.Test.Helper
{
    public class TestWebRequestFactory : IWebRequestFactory
    {
        private readonly Dictionary<string, Func<WebResponse>> lookup = new Dictionary<string, Func<WebResponse>>();

        public void RegisterResultForUri(string uri, string resultBody, HttpStatusCode returnCode = HttpStatusCode.OK, string location = null)
        {
            lookup.Add(uri.TrimEnd('/').ToLowerInvariant(), () => ReturnBody(resultBody, returnCode, location));
        }

        public void RegisterCompressedResultForUri(string uri, string resultBody, HttpStatusCode returnCode = HttpStatusCode.OK)
        {
            lookup.Add(uri.TrimEnd('/').ToLowerInvariant(), () => ReturnCompressedBody(resultBody, returnCode));
        }

        public void RegisterExceptionForUri<TEx>(string uri)
            where TEx : Exception, new()
        {
            lookup.Add(uri.TrimEnd('/').ToLowerInvariant(), () => { throw new TEx(); });
        }

        static WebResponse ReturnBody(string body, HttpStatusCode returnCode, string location)
        {
            var response = Substitute.For<HttpWebResponse>();
            var byteArray = Encoding.ASCII.GetBytes(body ?? string.Empty);
            response.GetResponseStream().Returns(new MemoryStream(byteArray));
            response.StatusCode.Returns(returnCode);
            response.Headers.Returns(location != null ? new WebHeaderCollection() {{"Location", location}} : new WebHeaderCollection());
            return response;
        }

        private static WebResponse ReturnCompressedBody(string body, HttpStatusCode returnCode)
        {
            var response = Substitute.For<HttpWebResponse>();
            var byteArray = Encoding.ASCII.GetBytes(body);
            var outStream = new MemoryStream();
            using (var stream = new MemoryStream(byteArray))
            {
                using (var zip = new GZipStream(outStream, CompressionMode.Compress))
                {
                    stream.CopyTo(zip);
                }
            }
            response.GetResponseStream().Returns(new MemoryStream(outStream.ToArray()));
            response.StatusCode.Returns(returnCode);
            response.Headers.Returns(new WebHeaderCollection() {{HttpRequestHeader.ContentEncoding, "gzip"}});
            return response;
        }

        public WebRequest Create(Uri requestUri)
        {
            var uri = requestUri.ToString().TrimEnd('/').ToLowerInvariant();
            if (lookup.ContainsKey(uri))
                return CreateWebRequest(new Uri(uri), lookup[uri]);

            Trace.WriteLine(string.Format("No result setup for URI {0}", uri));

            return null;
        }

        private static WebRequest CreateWebRequest(Uri uri, Func<WebResponse> toReturn)
        {
            var request = Substitute.For<WebRequest>();
            var asyncResult = Substitute.For<IAsyncResult>();

            request.RequestUri.Returns(uri);

            request.Headers.Returns(new WebHeaderCollection());

            request.BeginGetResponse(Arg.Any<AsyncCallback>(), Arg.Any<object>())
                   .Returns(c =>
                       {
                           asyncResult.AsyncState.Returns(c[1]);
                           c.Arg<AsyncCallback>()(asyncResult);
                           return asyncResult;
                       });

            request.EndGetRequestStream(Arg.Any<IAsyncResult>()).Returns(c => new MemoryStream());

            request.BeginGetRequestStream(Arg.Any<AsyncCallback>(), Arg.Any<object>())
                   .Returns(c =>
                       {
                           asyncResult.AsyncState.Returns(c[1]);
                           c.Arg<AsyncCallback>()(asyncResult);
                           return asyncResult;
                       });

            request.EndGetResponse(Arg.Any<IAsyncResult>())
                   .Returns(c => toReturn());

            return request;
        }
    }
}