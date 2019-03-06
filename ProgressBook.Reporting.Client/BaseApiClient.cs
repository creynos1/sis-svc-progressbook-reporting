using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressBook.Reporting.Client
{
        using System.Net.Http;
        using System.Net.Http.Headers;
        //using SoftwareAnswers.Library.ApiClient.Core;
        using SoftwareAnswers.ProgressBook.IdentityServer.Client.CentralAdmin.Handlers;

        public abstract class BaseApiClient
        {
            private readonly TokenMessageHandlerBase _handler;
            private readonly IExagoUri _exagoUri;

            protected BaseApiClient()
            {
            }

            protected BaseApiClient(TokenMessageHandlerBase handler, IExagoUri exagoUri)
            {
                _handler = handler;
                _exagoUri = exagoUri;
            }

            internal HttpClient GetHttpClient()
            {
                var httpClient = new HttpClient(_handler)
                {
                    BaseAddress = _exagoUri.GetExagoUri() // Settings.GetWebServiceUri()
                };

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return httpClient;
            }
        }
    
}
