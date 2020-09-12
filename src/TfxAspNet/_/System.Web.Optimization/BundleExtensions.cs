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
using System.Web.Optimization;

namespace Xyz.TForce.AspNet.Mvc
{

  public static class BundleExtensions
  {

    public static void RegisterScriptBundle(this BundleCollection bundles, string virtualPath, params string[] scriptFilePaths)
    {
      ScriptBundle bundle = new ScriptBundle(virtualPath);
      foreach (string path in scriptFilePaths)
      {
        _ = bundle.Include(path);
      }
      bundle.Orderer = new AsIsBundleOrderer();
      bundles.Add(bundle);
    }

    public static void RegisterStyleBundle(this BundleCollection bundles, string virtualPath, params string[] styleFilePaths)
    {
      StyleBundle bundle = new StyleBundle(virtualPath);
      foreach (string path in styleFilePaths)
      {
        _ = bundle.Include(path);
      }
      bundle.Orderer = new AsIsBundleOrderer();
      bundles.Add(bundle);
    }
  }
}
#endif
