using System;
using Microsoft.Extensions.Logging;
using Models;
using Models.Interfaces;
using Models.DTO;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Services;

public class AlbumsServiceWapi : IAlbumsService
{
    private readonly ILogger<AlbumsServiceWapi> _logger;
    private readonly HttpClient _httpClient;

    //To ensure Json deserializern is using the class implementations instead of interfaces 
    private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        Converters = {
            new AbstractConverter<MusicGroup, IMusicGroup>(),
            new AbstractConverter<Album, IAlbum>(),
            new AbstractConverter<Artist, IArtist>()
        },
    };

    public AlbumsServiceWapi(IHttpClientFactory httpClientFactory, ILogger<AlbumsServiceWapi> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(name: "MusicWebApi");
    }

    public async Task<ResponsePageDto<IAlbum>> ReadAlbumsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        string uri = $"albums/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponsePageDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> ReadAlbumAsync(Guid id, bool flat)
    {
        //Vilken endpoint man tittar på
        string uri = $"albums/readitem?id={id}&flat={flat}";

        //Skickar HTTP meddelande och inväntar svaret
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Kastar en 'exception' om svaret inte går igenom/misslyckas
        await response.EnsureSuccessStatusMessage();

        //Hämtar datan i response
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> DeleteAlbumAsync(Guid id)
    {
        string uri = $"albums/deleteitem/{id}";
        
        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);
        await response.EnsureSuccessStatusMessage();

        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> UpdateAlbumAsync(AlbumCUdto item)
    {
        string uri = $"albums/updateitem/{item.AlbumId}";
        var content = JsonContent.Create(item);
        
        HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
        await response.EnsureSuccessStatusMessage();

        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> CreateAlbumAsync(AlbumCUdto item)
    {
        string uri = $"albums/createitem";
        var content = JsonContent.Create(item);
        
        HttpResponseMessage response = await _httpClient.PostAsync(uri, content);
        await response.EnsureSuccessStatusMessage();

        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
}

