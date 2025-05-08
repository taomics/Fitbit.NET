using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Fitbit.Api.Portable;
using NUnit.Framework;

namespace Fitbit.Portable.Tests
{
    [TestFixture]
    public class RateLimitTests
    {
        [Test]
        [Category("Portable")]
        public void RateLimit_With_Reset_Header()
        {
            var responseMessage = new Func<HttpResponseMessage>(() => 
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests) 
                { 
                    Content = new StringContent(SampleDataHelper.GetContent("ApiError-Request-BadRequest.json"))
                };
                response.Headers.Add("Fitbit-Rate-Limit-Reset", "60");
                return response;
            });

            var verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            var fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            var exception = Assert.ThrowsAsync<FitbitRateLimitException>(async () => 
                await fitbitClient.GetUserProfileAsync());
                
            var expectedRetryTime = DateTime.UtcNow.AddSeconds(60);
            Assert.That(exception.RetryAfter, Is.EqualTo(expectedRetryTime).Within(5).Seconds);
        }

        [Test]
        [Category("Portable")]
        public void RateLimit_Without_Reset_Header()
        {
            var responseMessage = new Func<HttpResponseMessage>(() => 
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests) 
                { 
                    Content = new StringContent(SampleDataHelper.GetContent("ApiError-Request-BadRequest.json"))
                };
                return response;
            });

            var verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            var fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            var exception = Assert.ThrowsAsync<FitbitRateLimitException>(async () => 
                await fitbitClient.GetUserProfileAsync());
                
            var expectedRetryTime = DateTime.UtcNow.AddSeconds(3600);
            Assert.That(exception.RetryAfter, Is.EqualTo(expectedRetryTime).Within(5).Seconds);
        }
    }
}
