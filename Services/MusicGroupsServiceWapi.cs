using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Models.Interfaces;
using Newtonsoft.Json;

namespace Services;
//-----------------------------------RIKARD URVEGARD LUNDBERG-------------------------------------\\
//To ensure Json deserializern is using the class implementations instead of interfaces
public class MusicGroupsServiceWapi : IMusicGroupsService
{
    private readonly ILogger<MusicGroupsServiceWapi> _logger;
    private readonly HttpClient _httpClient;
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

        _logger.LogInformation($"Reading music groups from WebApi: {uri}");

        //Send the HTTP Message and await the response
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response data
        string jsonContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponsePageDto<IMusicGroup>>(jsonContent, _jsonSettings);

        return result;
    }

    public async Task<ResponseItemDto<IMusicGroup>> ReadMusicGroupAsync(Guid id, bool flat)
    {
        //Vilken endpoint man tittar på
        string uri = $"musicgroups/readitem?id={id}&flat={flat}";
        _logger.LogInformation($"Reading music group {id} from WebApi");

        //Skickar HTTP - förfrågan
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Säkerhetställer att den går igenom/lyckas
        await response.EnsureSuccessStatusMessage();

        //Deserialize - Deseraliserar objektet till JSON
        string jsonContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(jsonContent, _jsonSettings);

        return result;
    }

    public async Task<ResponseItemDto<IMusicGroup>> DeleteMusicGroupAsync(Guid id)
    {
        string uri = $"musicgroups/deleteitem/{id}";
        _logger.LogInformation($"Deleting music group {id} via WebApi");

        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);
        await response.EnsureSuccessStatusMessage();

        string jsonContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(jsonContent, _jsonSettings);

        return result;
    }

    public async Task<ResponseItemDto<IMusicGroup>> UpdateMusicGroupAsync(MusicGroupCUdto item)
    {
        string uri = $"musicgroups/updateitem/{item.MusicGroupId}";
        _logger.LogInformation($"Updating music group {item.MusicGroupId} via WebApi");

        //Serialiserar DTO:n till JSON
        string jsonContent = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //Skickar HTTP PUT förfrågan
        HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
        await response.EnsureSuccessStatusMessage();

        //Deserialiserar response/svaret till JSON
        string responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(responseContent, _jsonSettings);

        return result;
    }

    public async Task<ResponseItemDto<IMusicGroup>> CreateMusicGroupAsync(MusicGroupCUdto item)
    {
        string uri = $"musicgroups/createitem";
        _logger.LogInformation($"Creating new music group via WebApi");

        string jsonContent = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(uri, content);
        await response.EnsureSuccessStatusMessage();

        string responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(responseContent, _jsonSettings);

        return result;
    }
}