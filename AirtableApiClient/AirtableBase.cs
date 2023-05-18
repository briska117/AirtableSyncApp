// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableBase
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AirtableApiClient
{
  public class AirtableBase : IDisposable
  {
    private const int MAX_PAGE_SIZE = 100;
    private const string AIRTABLE_API_URL = "https://api.airtable.com/v0/";
    private readonly string BaseId;
    private readonly HttpClientWithRetries httpClientWithRetries;

    public bool ShouldNotRetryIfRateLimited
    {
      get => this.httpClientWithRetries.ShouldNotRetryIfRateLimited;
      set => this.httpClientWithRetries.ShouldNotRetryIfRateLimited = value;
    }

    public int RetryDelayMillisecondsIfRateLimited
    {
      get => this.httpClientWithRetries.RetryDelayMillisecondsIfRateLimited;
      set => this.httpClientWithRetries.RetryDelayMillisecondsIfRateLimited = value;
    }

    public AirtableBase(string apiKey, string baseId)
      : this(apiKey, baseId, (DelegatingHandler) null)
    {
    }

    internal AirtableBase(string apiKey, string baseId, DelegatingHandler delegatingHandler)
    {
      if (string.IsNullOrEmpty(apiKey))
        throw new ArgumentException("apiKey cannot be null", nameof (apiKey));
      this.BaseId = !string.IsNullOrEmpty(baseId) ? baseId : throw new ArgumentException("baseId cannot be null", nameof (baseId));
      this.httpClientWithRetries = new HttpClientWithRetries(delegatingHandler, apiKey);
    }

    public async Task<AirtableListRecordsResponse> ListRecords(
      string tableName,
      string offset = null,
      IEnumerable<string> fields = null,
      string filterByFormula = null,
      int? maxRecords = null,
      int? pageSize = null,
      IEnumerable<Sort> sort = null,
      string view = null)
    {
      HttpResponseMessage response = await this.ListRecordsInternal(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view);
      AirtableApiException error = await this.CheckForAirtableException(response);
      return error != null ? new AirtableListRecordsResponse(error) : new AirtableListRecordsResponse(JsonConvert.DeserializeObject<AirtableRecordList>(await response.Content.ReadAsStringAsync()));
    }

    public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(
      string tableName,
      string offset = null,
      IEnumerable<string> fields = null,
      string filterByFormula = null,
      int? maxRecords = null,
      int? pageSize = null,
      IEnumerable<Sort> sort = null,
      string view = null)
    {
      HttpResponseMessage response = await this.ListRecordsInternal(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view);
      AirtableApiException error = await this.CheckForAirtableException(response);
      return error != null ? new AirtableListRecordsResponse<T>(error) : new AirtableListRecordsResponse<T>(JsonConvert.DeserializeObject<AirtableRecordList<T>>(await response.Content.ReadAsStringAsync()));
    }

    public async Task<AirtableRetrieveRecordResponse> RetrieveRecord(
      string tableName,
      string id)
    {
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentException("Table Name cannot be null", nameof (tableName));
      if (string.IsNullOrEmpty(id))
        throw new ArgumentException("Record ID cannot be null", nameof (id));
      HttpResponseMessage response = await this.httpClientWithRetries.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://api.airtable.com/v0/" + this.BaseId + "/" + tableName + "/" + id));
      AirtableApiException error = await this.CheckForAirtableException(response);
      return error != null ? new AirtableRetrieveRecordResponse(error) : new AirtableRetrieveRecordResponse(JsonConvert.DeserializeObject<AirtableRecord>(await response.Content.ReadAsStringAsync()));
    }

    public async Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecord(
      string tableName,
      Fields fields,
      bool typecast = false)
    {
      return await this.CreateUpdateReplaceRecord(tableName, fields, AirtableBase.OperationType.CREATE, typecast: typecast);
    }

    public async Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(
      string tableName,
      Fields fields,
      string id,
      bool typeCast = false)
    {
      return await this.CreateUpdateReplaceRecord(tableName, fields, AirtableBase.OperationType.UPDATE, id, typeCast);
    }

    public async Task<AirtableCreateUpdateReplaceRecordResponse> ReplaceRecord(
      string tableName,
      Fields fields,
      string id,
      bool typeCast = false)
    {
      return await this.CreateUpdateReplaceRecord(tableName, fields, AirtableBase.OperationType.REPLACE, id, typeCast);
    }

    public async Task<AirtableDeleteRecordResponse> DeleteRecord(
      string tableName,
      string id)
    {
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentException("Table name cannot be null", nameof (tableName));
      if (string.IsNullOrEmpty(id))
        throw new ArgumentException("Record ID cannot be null", nameof (id));
      HttpResponseMessage response = await this.httpClientWithRetries.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "https://api.airtable.com/v0/" + this.BaseId + "/" + tableName + "/" + id));
      AirtableApiException error = await this.CheckForAirtableException(response);
      if (error != null)
        return new AirtableDeleteRecordResponse(error);
      AirtableDeletedRecord airtableDeletedRecord = JsonConvert.DeserializeObject<AirtableDeletedRecord>(await response.Content.ReadAsStringAsync());
      return new AirtableDeleteRecordResponse(airtableDeletedRecord.Deleted, airtableDeletedRecord.Id);
    }

    public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateMultipleRecords(
      string tableName,
      Fields[] fields,
      bool typecast = false)
    {
      string json = JsonConvert.SerializeObject((object) new
      {
        records = fields,
        typecast = typecast
      }, new JsonSerializerSettings()
      {
        NullValueHandling = (NullValueHandling) 1
      });
      return await this.CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Post, json);
    }

    public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> UpdateMultipleRecords(
      string tableName,
      IdFields[] idFields,
      bool typecast = false)
    {
      string json = JsonConvert.SerializeObject((object) new
      {
        records = idFields,
        typecast = typecast
      }, new JsonSerializerSettings()
      {
        NullValueHandling = (NullValueHandling) 1
      });
      return await this.CreateUpdateReplaceMultipleRecords(tableName, new HttpMethod("PATCH"), json);
    }

    public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> ReplaceMultipleRecords(
      string tableName,
      IdFields[] idFields,
      bool typecast = false)
    {
      string json = JsonConvert.SerializeObject((object) new
      {
        records = idFields,
        typecast = typecast
      }, new JsonSerializerSettings()
      {
        NullValueHandling = (NullValueHandling) 1
      });
      return await this.CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Put, json);
    }

    public void Dispose() => this.httpClientWithRetries.Dispose();

    private Uri BuildUriForListRecords(
      string tableName,
      string offset,
      IEnumerable<string> fields,
      string filterByFormula,
      int? maxRecords,
      int? pageSize,
      IEnumerable<Sort> sort,
      string view)
    {
      UriBuilder uriBuilder = new UriBuilder("https://api.airtable.com/v0/" + this.BaseId + "/" + tableName);
      if (!string.IsNullOrEmpty(offset))
        this.AddParametersToQuery(ref uriBuilder, "offset=" + HttpUtility.UrlEncode(offset));
      if (fields != null)
      {
        string queryToAppend = QueryParamHelper.FlattenFieldsParam(fields);
        this.AddParametersToQuery(ref uriBuilder, queryToAppend);
      }
      if (!string.IsNullOrEmpty(filterByFormula))
        this.AddParametersToQuery(ref uriBuilder, "filterByFormula=" + HttpUtility.UrlEncode(filterByFormula));
      if (sort != null)
      {
        string queryToAppend = QueryParamHelper.FlattenSortParam(sort);
        this.AddParametersToQuery(ref uriBuilder, queryToAppend);
      }
      if (!string.IsNullOrEmpty(view))
        this.AddParametersToQuery(ref uriBuilder, "view=" + HttpUtility.UrlEncode(view));
      int? nullable;
      if (maxRecords.HasValue)
      {
        nullable = maxRecords;
        int num = 0;
        if (nullable.GetValueOrDefault() <= num & nullable.HasValue)
          throw new ArgumentException("Maximum Number of Records must be > 0", nameof (maxRecords));
        this.AddParametersToQuery(ref uriBuilder, string.Format("maxRecords={0}", (object) maxRecords));
      }
      if (pageSize.HasValue)
      {
        nullable = pageSize;
        int num1 = 0;
        if (!(nullable.GetValueOrDefault() <= num1 & nullable.HasValue))
        {
          nullable = pageSize;
          int num2 = 100;
          if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
          {
            this.AddParametersToQuery(ref uriBuilder, string.Format("pageSize={0}", (object) pageSize));
            goto label_19;
          }
        }
        throw new ArgumentException("Page Size must be > 0 and <= 100", nameof (pageSize));
      }
label_19:
      return uriBuilder.Uri;
    }

    private void AddParametersToQuery(ref UriBuilder uriBuilder, string queryToAppend)
    {
      if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
        uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
      else
        uriBuilder.Query = queryToAppend;
    }

    private async Task<AirtableCreateUpdateReplaceRecordResponse> CreateUpdateReplaceRecord(
      string tableName,
      Fields fields,
      AirtableBase.OperationType operationType,
      string recordId = null,
      bool typecast = false)
    {
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentException("Table Name cannot be null", nameof (tableName));
      string requestUri = "https://api.airtable.com/v0/" + this.BaseId + "/" + tableName + "/";
      HttpMethod method;
      switch (operationType)
      {
        case AirtableBase.OperationType.CREATE:
          method = HttpMethod.Post;
          break;
        case AirtableBase.OperationType.UPDATE:
          requestUri = requestUri + recordId + "/";
          method = new HttpMethod("PATCH");
          break;
        case AirtableBase.OperationType.REPLACE:
          requestUri = requestUri + recordId + "/";
          method = HttpMethod.Put;
          break;
        default:
          throw new ArgumentException("Operation Type must be one of { OperationType.UPDATE, OperationType.REPLACE, OperationType.CREATE }", nameof (operationType));
      }
      string content = JsonConvert.SerializeObject((object) new
      {
        fields = fields.FieldsCollection,
        typecast = typecast
      }, new JsonSerializerSettings()
      {
        NullValueHandling = (NullValueHandling) 1
      });
      HttpResponseMessage response = await this.httpClientWithRetries.SendAsync(new HttpRequestMessage(method, requestUri)
      {
        Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json")
      });
      AirtableApiException error = await this.CheckForAirtableException(response);
      return error != null ? new AirtableCreateUpdateReplaceRecordResponse(error) : new AirtableCreateUpdateReplaceRecordResponse(JsonConvert.DeserializeObject<AirtableRecord>(await response.Content.ReadAsStringAsync()));
    }

    private async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateUpdateReplaceMultipleRecords(
      string tableName,
      HttpMethod method,
      string json)
    {
      string[] strArray = !string.IsNullOrEmpty(tableName) ? new string[5]
      {
        "https://api.airtable.com/v0/",
        this.BaseId,
        "/",
        tableName,
        "/"
      } : throw new ArgumentException("Table Name cannot be null", nameof (tableName));
      HttpResponseMessage response = await this.httpClientWithRetries.SendAsync(new HttpRequestMessage(method, string.Concat(strArray))
      {
        Content = (HttpContent) new StringContent(json, Encoding.UTF8, "application/json")
      });
      AirtableApiException error = await this.CheckForAirtableException(response);
      return error != null ? new AirtableCreateUpdateReplaceMultipleRecordsResponse(error) : new AirtableCreateUpdateReplaceMultipleRecordsResponse(JsonConvert.DeserializeObject<AirtableRecordList>(await response.Content.ReadAsStringAsync()).Records);
    }

    private async Task<AirtableApiException> CheckForAirtableException(
      HttpResponseMessage response)
    {
      switch (response.StatusCode)
      {
        case HttpStatusCode.OK:
          return (AirtableApiException) null;
        case HttpStatusCode.BadRequest:
          return (AirtableApiException) new AirtableBadRequestException();
        case HttpStatusCode.Unauthorized:
          return (AirtableApiException) new AirtableUnauthorizedException();
        case HttpStatusCode.PaymentRequired:
          return (AirtableApiException) new AirtablePaymentRequiredException();
        case HttpStatusCode.Forbidden:
          return (AirtableApiException) new AirtableForbiddenException();
        case HttpStatusCode.NotFound:
          return (AirtableApiException) new AirtableNotFoundException();
        case HttpStatusCode.RequestEntityTooLarge:
          return (AirtableApiException) new AirtableRequestEntityTooLargeException();
        case (HttpStatusCode) 422:
          return (AirtableApiException) new AirtableInvalidRequestException(await AirtableBase.ReadResponseErrorMessage(response));
        case (HttpStatusCode) 429:
          return (AirtableApiException) new AirtableTooManyRequestsException();
        default:
          throw new AirtableUnrecognizedException(response.StatusCode);
      }
    }

    private static async Task<string> ReadResponseErrorMessage(HttpResponseMessage response)
    {
      string str1 = await response.Content.ReadAsStringAsync();
      if (string.IsNullOrEmpty(str1))
        return (string) null;
      JToken jtoken1 = JObject.Parse(str1)["error"];
      string str2;
      if (jtoken1 == null)
      {
        str2 = (string) null;
      }
      else
      {
        JToken jtoken2 = jtoken1[(object) "message"];
        str2 = jtoken2 != null ? Extensions.Value<string>((IEnumerable<JToken>) jtoken2) : (string) null;
      }
      return str2;
    }

    private async Task<HttpResponseMessage> ListRecordsInternal(
      string tableName,
      string offset,
      IEnumerable<string> fields,
      string filterByFormula,
      int? maxRecords,
      int? pageSize,
      IEnumerable<Sort> sort,
      string view)
    {
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentException("Table Name cannot be null", nameof (tableName));
      return await this.httpClientWithRetries.SendAsync(new HttpRequestMessage(HttpMethod.Get, this.BuildUriForListRecords(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view)));
    }

    private enum OperationType
    {
      CREATE,
      UPDATE,
      REPLACE,
    }
  }
}
