using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Models.Interfaces;
using Newtonsoft.Json;

namespace Services;

public class MusicGroupsServiceWapi : IMusicGroupsService
{
    private readonly ILogger<MusicGroupsServiceWapi> _logger;
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

    public MusicGroupsServiceWapi(IHttpClientFactory httpClientFactory, ILogger<MusicGroupsServiceWapi> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(name: "MusicWebApi");
    }

    public async Task<ResponsePageDto<IMusicGroup>> ReadMusicGroupsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) 
    {
        string uri = $"musicgroups/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponsePageDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }

    public async Task<ResponseItemDto<IMusicGroup>> ReadMusicGroupAsync(Guid id, bool flat)
    {
        string uri = $"/api/MusicGroups/ReadItemDto?id={id}&flat={flat}";
        var response = await _httpClient.GetAsync(uri);

        await response.EnsureSuccessStatusMessage();

        string s = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
    }
    public async Task<ResponseItemDto<IMusicGroup>> DeleteMusicGroupAsync(Guid id)
    {
        string uri = $"/api/MusicGroups/DeleteItem/{id}";
        var response = await _httpClient.DeleteAsync(uri);
        await response.EnsureSuccessStatusMessage();
        string s = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
    }
   public async Task<ResponseItemDto<IMusicGroup>> UpdateMusicGroupAsync(MusicGroupCUdto item)
    {
        string uri = $"/api/MusicGroups/UpdateItem/{item.MusicGroupId}";
        var response = await _httpClient.PutAsJsonAsync(uri, item);

        await response.EnsureSuccessStatusMessage();

        string s = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
    }
    public async Task<ResponseItemDto<IMusicGroup>> CreateMusicGroupAsync(MusicGroupCUdto item)
    {
        string uri = "/api/MusicGroups/CreateItem";
        item.MusicGroupId = null;
        var response = await _httpClient.PostAsJsonAsync(uri, item);

        await response.EnsureSuccessStatusMessage();

        string s = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
    }
}

