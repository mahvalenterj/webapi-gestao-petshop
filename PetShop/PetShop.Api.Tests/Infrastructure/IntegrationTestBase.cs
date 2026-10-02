namespace PetShop.Api.Tests.Infrastructure
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected IntegrationTestBase()
        {
            Factory = new PetShopApiFactory();
            Client = Factory.CreateClient();
        }

        protected PetShopApiFactory Factory { get; }

        protected HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
