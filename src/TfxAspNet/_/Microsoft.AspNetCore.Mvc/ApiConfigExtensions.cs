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

#if NETSTANDARD2_0
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Buffers;

namespace Microsoft.AspNetCore.Mvc
{

  public static class ApiConfigExtensions
  {

    public static void EnableJsonForHtml(this MvcOptions config)
    {
      config.OutputFormatters.RemoveType<JsonOutputFormatter>();
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      config.OutputFormatters.Add(new JsonOutputFormatter(settings, ArrayPool<char>.Shared));
    }

    public static void EnableXmlForHtml(this MvcOptions config)
    {
      config.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
      config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
    }

    public static void UseCamelCaseForPropertyNames(this MvcOptions config)
    {
      config.OutputFormatters.RemoveType<JsonOutputFormatter>();
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      config.OutputFormatters.Add(new JsonOutputFormatter(settings, ArrayPool<char>.Shared));
    }
  }
}
#endif
