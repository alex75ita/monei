﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Monei.DataAccessLayer.Interfaces;
using Monei.DataAccessLayer.SqlServer;
using Monei.MvcApplication;
using Monei.MvcApplication.Api;
using Monei.MvcApplication.Code;
using Monei.MvcApplication.Core.Installers;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System.Configuration;
using Monei.Entities;

namespace Monei.Test.IntegrationTest.MvcApplication.Api
{
    [TestFixture]
    public class ApiControllerTestBase :IDisposable
    {
        protected ISessionFactoryProvider sessionFactoryProvider = new SessionFactoryProvider();

        public string testAccountGuid = "00000000-0000-0000-0000-000000000000";

        protected HttpMethod GET = HttpMethod.Get;
        protected HttpMethod POST = HttpMethod.Post;
        protected HttpMethod DELETE = HttpMethod.Delete ;
        protected Random random = new Random(DateTime.Now.Millisecond);
        protected TestDataProvider testDataProvider;

        private HttpServer server;
        private const string BASE_URL = "http://www.apitest.com/";
        private IWindsorContainer container;

        // todo: CLEAN THIS CLASS

        public ApiControllerTestBase()
        {            
            testDataProvider = new TestDataProvider();
        }

        /// <summary>
        /// Return a new HttpServer. A new one is created every time because it can be "dirty" from prevous use.
        /// </summary>
        /// <returns></returns>
        public HttpServer InitializeServer()
        {
            //todo: is this needed?
            if (server != null)
                server.Dispose();
            server = new HttpServer(GetConfiguration());
            return server;
        }


        [OneTimeSetUp]
        public void Initialize()
        {			
            InitializeWindsorContainer();
            //server = new HttpServer(GetConfiguration());
        }


        protected HttpClient GetClient()
        {
            return new HttpClient(InitializeServer());
        }


        private void InitializeWindsorContainer()
        {
            container = new WindsorContainer();

            container.Register(
                Component.For<ISessionFactoryProvider>().ImplementedBy<SessionFactoryProvider>(),

                Component.For<IAccountRepository>().ImplementedBy(typeof(AccountRepository)), //.LifestylePerWebRequest(),
                Component.For<IRegistryRepository>().ImplementedBy(typeof(RegistryRepository)), //.LifestylePerWebRequest(),
                Component.For<ICurrencyRepository>().ImplementedBy(typeof(CurrencyRepository)), //.LifestylePerWebRequest(),
                Component.For<ICategoryRepository>().ImplementedBy(typeof(CategoryRepository)), //.LifestylePerWebRequest(),
                Component.For<ISubcategoryRepository>().ImplementedBy(typeof(SubcategoryRepository)) //.LifestylePerWebRequest(),

                //Component.For(typeof(SubcategoryManager)).ImplementedBy(typeof(SubcategoryManager)), //.LifestylePerWebRequest()
            );
            
            // todo: try to use this...
            //container.Install(new RepositoriesInstaller());
            // give this error:
            /*
            An exception of type 'Castle.MicroKernel.ComponentResolutionException' occurred in Castle.Windsor.dll but was not handled in user code
            Additional information: Looks like you forgot to register the http module Castle.MicroKernel.Lifestyle.PerWebRequestLifestyleModule
            */

            // for check...
            IAccountRepository accountREpository = container.Resolve<IAccountRepository>();

            container.Install( new ControllerInstaller());

            //container.Resolve<MoneiControllerBase>();
            //ApiController controller = container.Resolve<ApiControllerBase>();

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            

            var httpDependencyResolver = new WindsorDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = httpDependencyResolver;
        }

        protected HttpConfiguration GetConfiguration()
        {
            HttpConfiguration configuration = new HttpConfiguration();		
            var dependencyResolver = new WindsorDependencyResolver(container);
            configuration.DependencyResolver = dependencyResolver;
            WebApiConfig.Register(configuration);

            return configuration;
        }

        /// <summary>
        /// Create a HttpRequestMessage that can be used with HttpClient.SendAsync() method.		/// 
        /// </summary>
        /// <param name="url">Partial URL of the API <example>api/mycontroller/param</example></param>
        /// <param name="method">GET, POST ...</param>
        /// <param name="content"></param>
        /// <param name="mediaType">HTTP "Header Accept" value, default is for JSON data</param>
        /// <returns></returns>
        protected HttpRequestMessage CreateRequest(string url, HttpMethod method, string mediaType = "application/json")
        {
            var request = new HttpRequestMessage(method, BASE_URL + url);
            request.Headers.Add("account-guid", testAccountGuid);

            // I don't know what this does. Copied from one example.
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            return request;
        }

        /// <summary>
        /// Create a HttpRequestMessage that can be used with HttpClient.SendAsync() method.		/// 
        /// </summary>
        /// <typeparam name="T">Type of the class passed in Content of the request</typeparam>
        /// <param name="url">Partial URL of the API <example>api/mycontroller/param</example></param>
        /// <param name="method">GET, POST ...</param>
        /// <param name="content">The object data passed in the request</param>
        /// <param name="mediaType">HTTP "Header Accept" value, default is for JSON data</param>
        /// <returns></returns>
        protected HttpRequestMessage CreateRequest<T>(string url, HttpMethod method,  T content, string mediaType = "application/json")
        {
            var request = CreateRequest(url, method, mediaType);
            request.Headers.Add("account-guid", testAccountGuid);
            // todo: is this needed? (CamelCasePropertyNameContractResolver)
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            request.Content = new ObjectContent<T>(content, formatter);
            return request;
        }


        protected void CallApi(string url, HttpMethod httpMethod)
            //where TReturn : class
        {
            using (var client = GetClient())
            using (var result = client.SendAsync(CreateRequest(url, httpMethod)).Result)
                CheckResult(url, result);
        }

        protected TReturn CallApi<TReturn>(string url, HttpMethod httpMethod)
            where TReturn : class
        {
            using (var client = GetClient())
            using (var result = client.SendAsync(CreateRequest(url, httpMethod)).Result)
                return LoadReturnValue<TReturn>(url, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPost">Type represented by HTTP form collection data</typeparam>
        /// <typeparam name="TReturn">Returned object</typeparam>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="data">POST form collection data</param>
        /// <returns></returns>
        protected TReturn CallApi<TPost, TReturn>(string url, HttpMethod httpMethod, TPost data)
            where TPost : class
            //where TReturn : class
        {
            using (var client = GetClient())
            using (var result = client.SendAsync(CreateRequest<TPost>(url, httpMethod, data)).Result)
                return LoadReturnValue<TReturn>(url, result);
        }

        /// <summary>
        /// Call API
        /// </summary>
        /// <typeparam name="TPost">Type represented by HTTP form collection data</typeparam>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="data">POST form collection data</param>
        protected void CallApi<TPost>(string url, HttpMethod httpMethod, TPost data)
            where TPost : class
        {
            using (var client = GetClient())
            using (var result = client.SendAsync(CreateRequest<TPost>(url, httpMethod, data)).Result)
                CheckResult(url, result);
        }

        /// <summary>
        /// Call API with POST passing the data
        /// </summary>
        /// <typeparam name="TPost">Type represented by HTTP form collection data</typeparam>
        /// <param name="url"></param>
        /// <param name="data">POST form collection data</param>
        protected void CallApi<TPost>(string url, TPost data) where TPost:class
        {
            CallApi<TPost>(url, POST, data);
        }


        private static TReturn LoadReturnValue<TReturn>(string url, HttpResponseMessage result) 
            //where TReturn : class
        {
            CheckResult(url, result);
            return result.Content.ReadAsAsync<TReturn>().Result;
        }

        private static void CheckResult(string url, HttpResponseMessage result)
        {
            if (!result.IsSuccessStatusCode)
                Assert.Fail("Server error. Url: " + url + ".\r\n" + result);
        }


        protected int RandomInt()
        {
            return random.Next();
        }


        public void Dispose()
        {
            if (server != null)
                server.Dispose();
        }      

    }
}
