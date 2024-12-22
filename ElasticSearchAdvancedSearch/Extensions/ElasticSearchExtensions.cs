using ElasticSearchAdvancedSearch.Models;
using Nest;

namespace ElasticSearchAdvancedSearch.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Elasticsearch:Uri"];
            var defaultIndex = configuration["Elasticsearch:index"];

            if(url == null)
                throw new ArgumentNullException(nameof(url));
            var settings = new ConnectionSettings(new Uri(url))
                .PrettyJson()
                .DefaultIndex(defaultIndex);
            
            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);

            if (defaultIndex == null)
                throw new ArgumentNullException(nameof(defaultIndex));
            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<Product>(p => 
            p.IdProperty(x => x.Id));
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            client.Indices.Create(indexName, i => i.Map<Product>(x => x.AutoMap()));
        }
    }
}