﻿using Microsoft.AspNetCore.Mvc.Rendering;
using WebMvc.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebMvc.Infrastructure;

namespace WebMvc.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;

        private readonly string _remoteServiceBaseUrl;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _logger = logger;

            _remoteServiceBaseUrl = $"http://localhost:5101/api/v1/catalog/";
        }

        public async Task<CatalogItem> GetCatalogItem(int id)
        {
            var uri = API.Catalog.GetCatalogItem(_remoteServiceBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(uri);
            var catalog = JsonConvert.DeserializeObject<CatalogItem>(responseString);
            return catalog;
        }


        public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
        {
            var uri = API.Catalog.GetAllCatalogItems(_remoteServiceBaseUrl, page, take, brand, type);

            var responseString = await _httpClient.GetStringAsync(uri);

            var catalog = JsonConvert.DeserializeObject<Catalog>(responseString);

            return catalog;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var uri = API.Catalog.GetAllBrands(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };

            var brands = JArray.Parse(responseString);

            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var uri = API.Catalog.GetAllTypes(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            var brands = JArray.Parse(responseString);
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("type")
                });
            }

            return items;
        }
    }
}
