using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DwollaDotNet
{
    public class DwollaAPI
    {
        //===============================================================
        public DwollaAPI(String accessToken)
        {
            AccessToken = accessToken;
        }
        //===============================================================
        public String AccessToken { get; private set; }
        //===============================================================
        private String ToDestinationTypeString(DestinationType type)
        {
            switch (type)
            {
                case DestinationType.Dwolla:
                    return "dwolla";
                case DestinationType.Twitter:
                    return "twitter";
                case DestinationType.Facebook:
                    return "facebook";
                case DestinationType.Phone:
                    return "phone";
                case DestinationType.Email:
                    return "email";
                default:
                    throw new ArgumentException("Unknown destination type encountered");
            }
        }
        //===============================================================
        private RestClient CreateClient()
        {
            return new RestClient("https://www.dwolla.com/oauth/rest");
        }
        //===============================================================
        private RestRequest CreateRequest(String endpoint, Method method)
        {
            var request = new RestRequest(endpoint + "?oauth_token=" + AccessToken, method);
            request.RequestFormat = DataFormat.Json;
            return request;
        }
        //===============================================================
        public DwollaSendResponse Send(int userPIN, String destinationID, decimal amount, DestinationType destinationType = DestinationType.Email, String notes = null)
        {
            var client = CreateClient();
            var request = CreateRequest("transactions/send", Method.POST);
            var body = new { pin = userPIN, destinationId = destinationID, destinationType = ToDestinationTypeString(destinationType), amount = amount, notes = notes };
            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<DwollaSendResponse>(response.Content);
        }
        //===============================================================

    }

    [TestFixture]
    public class APITests
    {
        //===============================================================
        [Test]
        public void No404()
        {
            var api = new DwollaAPI("c6A7OuWESlCrtbbXYvvtidGF7Lir6njnZObFCBUH0ekUKPb24W");
            var response = api.Send(1209, "b.frayne29@gmail.com", .1m);

            Assert.True(response.Success);
        }
        //===============================================================
    }
}
