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
namespace Xyz.TForce.AspNet.Security
{

  internal class PolicyTypes
  {

    public const string Advanced = "PolicyType=TfxAdvanced";

    public const string Permission = "PolicyType=TfxPermission";

    public const string Role = "PolicyType=TfxRole";

    public const string User = "PolicyType=TfxUser";
  }

  internal class PolicySubTypes
  {

    public const string Permission = "Permissions";

    public const string Role = "Roles";

    public const string User = "Users";
  }
}
#endif
