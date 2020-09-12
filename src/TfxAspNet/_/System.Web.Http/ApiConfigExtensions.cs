// Copyright 2017 T-Force Xyz
// Please refer to LICENSE & CONTRIB files in the project root for license information.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if NET45
using System.Net.Http.Headers;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json.Serialization;
using Xyz.TForce.AspNet.Http;

namespace System.Web.Http
{

  public static class ApiConfigExtensions
  {

    public static void EnableJsonForHtml(this HttpConfiguration config)
    {
      // Source: http://stackoverflow.com/questions/9847564/how-do-i-get-asp-net-web-api-to-return-json-instead-of-xml-using-chrome
      config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
    }

    public static void EnableXmlForHtml(this HttpConfiguration config)
    {
      config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
    }

    public static void UseCamelCaseForPropertyNames(this HttpConfiguration config)
    {
      // Source: http://stackoverflow.com/questions/28552567/web-api-2-how-to-return-json-with-camelcased-property-names-on-objects-and-the
      config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
    }

    public static void UseNamespaceHttpControllerSelector(this HttpConfiguration config)
    {
      config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config));
    }
  }
}
#endif
