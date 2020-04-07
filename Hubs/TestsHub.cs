namespace SignalRTests.Hubs
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;

    public class TestsHub : Hub
    {
        public TestResponse runTest(TestRequest request)
        {
            TestResponse response = new TestResponse();
            response.data = this.generate(request.blockSize);
            return response;
        }
        private static Random random = new Random();
        public string generate(int blockSize)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = new string
                (Enumerable.Repeat(chars, blockSize).Select(
                    s => s[random.Next(s.Length)]
                ).ToArray()
            );
            return result;
        }
    }
}