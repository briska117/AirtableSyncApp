// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.QueryParamHelper
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Collections.Generic;
using System.Web;

namespace AirtableApiClient
{
  internal class QueryParamHelper
  {
    internal static string FlattenSortParam(IEnumerable<Sort> sort)
    {
      int num = 0;
      string str1 = string.Empty;
      string str2 = string.Empty;
      foreach (Sort sort1 in sort)
      {
        if (string.IsNullOrEmpty(str2) && num > 0)
          str2 = "&";
        string str3 = string.Format("sort[{0}][field]", (object) num);
        str1 = str1 + str2 + HttpUtility.UrlEncode(str3) + "=" + HttpUtility.UrlEncode(sort1.Field);
        string str4 = string.Format("sort[{0}][direction]", (object) num);
        str1 = str1 + "&" + HttpUtility.UrlEncode(str4) + "=" + HttpUtility.UrlEncode(sort1.Direction.ToString().ToLower());
        ++num;
      }
      return str1;
    }

    internal static string FlattenFieldsParam(IEnumerable<string> fields)
    {
      int num = 0;
      string str1 = string.Empty;
      string str2 = string.Empty;
      foreach (string field in fields)
      {
        if (string.IsNullOrEmpty(str2) && num > 0)
          str2 = "&";
        string str3 = string.Format("fields[{0}]", (object) num);
        str1 = str1 + str2 + HttpUtility.UrlEncode(str3) + "=" + HttpUtility.UrlEncode(field);
        ++num;
      }
      return str1;
    }
  }
}
