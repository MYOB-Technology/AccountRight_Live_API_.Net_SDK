﻿using System;
#if ASYNC
using System.Threading.Tasks;
#endif
using MYOB.AccountRight.SDK.Contracts.Version2;
using MYOB.AccountRight.SDK.Contracts.Version2.Sale;

namespace MYOB.AccountRight.SDK.Services.Sale
{
    public sealed class CustomerPaymentService : MutableService<CustomerPayment>
    {
        public CustomerPaymentService(IApiConfiguration configuration, IWebRequestFactory webRequestFactory = null, IOAuthKeyService keyService = null)
            : base(configuration, webRequestFactory, keyService)
        {
        }

        public override string Route
        {
            get { return "Sale/CustomerPayment"; }
        }

        public override string Update(Contracts.CompanyFile cf, CustomerPayment entity, ICompanyFileCredentials credentials)
        {
            throw new NotSupportedException();
        }

        public override void Update(Contracts.CompanyFile cf, CustomerPayment entity, ICompanyFileCredentials credentials, Action<System.Net.HttpStatusCode, string> onComplete, Action<Uri, Exception> onError)
        {
            throw new NotSupportedException();
        }
#if ASYNC
        public override Task<string> UpdateAsync(Contracts.CompanyFile cf, CustomerPayment entity, ICompanyFileCredentials credentials)
        {
            return Task.Factory.StartNew<string>(() => { throw new NotSupportedException(); });       
        }
#endif
    }
}