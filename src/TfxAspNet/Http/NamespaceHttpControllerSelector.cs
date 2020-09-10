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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Xyz.TForce.Diagnostics;
#if NETSTANDARD2_0
using Microsoft.Web.Http.Controllers;
#else
using System.Web.Http.Controllers;
#endif

namespace Xyz.TForce.AspNet.Http
{

  // Source: http://www.forgetfulcoder.com/2015/02/web-api-and-namespace-routing.html
  public class NamespaceHttpControllerSelector : IHttpControllerSelector
  {

    private const string NamespaceKey = "namespace";
    private const string ControllerKey = "controller";
    private readonly HttpConfiguration _configuration;
    private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllerDictionary;
    private readonly Lazy<Dictionary<string, List<HttpControllerDescriptor>>> _altControllerDictionary;
    private readonly HashSet<string> _duplicatedControllers;

    public NamespaceHttpControllerSelector(HttpConfiguration config)
    {
      _configuration = config;
      _controllerDictionary = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
      _altControllerDictionary = new Lazy<Dictionary<string, List<HttpControllerDescriptor>>>(InitializeAltControllerDictionary);
      _duplicatedControllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public NamespaceHttpControllerSelector(HttpConfiguration config, IAdvancedLog logger)
        : this(config)
    {
      _ = logger;
    }

    public HttpControllerDescriptor SelectController(HttpRequestMessage request)
    {
      IHttpRouteData routeData = request.GetRouteData();
      if (routeData == null)
      {
        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
      }
      string controllerName = GetRouteVariable<string>(routeData, ControllerKey);
      if (controllerName == null)
      {
        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
      }
      string namespaceName = GetRouteVariable<string>(routeData, NamespaceKey);
      if (string.IsNullOrWhiteSpace(namespaceName))
      {
        string key = controllerName;
        if (_altControllerDictionary.Value.TryGetValue(key, out List<HttpControllerDescriptor> controllerDescriptors))
        {
          if (controllerDescriptors.Count == 1)
          {
            return controllerDescriptors.First();
          }
          throw new HttpResponseException(
            request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError,
              "Multiple controllerDictionary were found that match this request."));
        }
        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
      }
      else
      {
        string key = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", namespaceName, controllerName);
        HttpControllerDescriptor controllerDescriptor;
        if (_controllerDictionary.Value.TryGetValue(key, out controllerDescriptor))
        {
          return controllerDescriptor;
        }
        if (_duplicatedControllers.Contains(key))
        {
          throw new HttpResponseException(
            request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError,
              "Multiple controllerDictionary were found that match this request."));
        }
        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
      }
    }

    public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
    {
      return _controllerDictionary.Value;
    }

    private static T GetRouteVariable<T>(IHttpRouteData routeData, string name)
    {
      object result;
      return routeData.Values.TryGetValue(name, out result) ? (T)result : default(T);
    }

    private Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
    {
      Dictionary<string, HttpControllerDescriptor> dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);
      IAssembliesResolver assembliesResolver = _configuration.Services.GetAssembliesResolver();
      IHttpControllerTypeResolver controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();
      ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
      foreach (Type controllerType in controllerTypes)
      {
        if (controllerType.Namespace == null)
        {
          continue;
        }
        string key = controllerType.FullName.Remove(controllerType.FullName.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);
        if (dictionary.Keys.Contains(key))
        {
          _ = _duplicatedControllers.Add(key);
        }
        else
        {
          dictionary[key] = new HttpControllerDescriptor(_configuration, controllerType.Name, controllerType);
        }
      }
      foreach (string key in _duplicatedControllers)
      {
        _ = dictionary.Remove(key);
      }
      return dictionary;
    }

    private Dictionary<string, List<HttpControllerDescriptor>> InitializeAltControllerDictionary()
    {
      Dictionary<string, List<HttpControllerDescriptor>> dictionary = new Dictionary<string, List<HttpControllerDescriptor>>(StringComparer.OrdinalIgnoreCase);
      IAssembliesResolver assembliesResolver = _configuration.Services.GetAssembliesResolver();
      IHttpControllerTypeResolver controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();
      ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
      foreach (Type controllerType in controllerTypes)
      {
        if (controllerType.Namespace == null)
        {
          continue;
        }
        string key = controllerType.Name.Remove(controllerType.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);
        HttpControllerDescriptor value = new HttpControllerDescriptor(_configuration, controllerType.Name, controllerType);
        if (dictionary.Keys.Contains(key))
        {
          dictionary[key].Add(value);
        }
        else
        {
          dictionary[key] = new List<HttpControllerDescriptor> { value };
        }
      }
      return dictionary;
    }
  }
}
#endif
