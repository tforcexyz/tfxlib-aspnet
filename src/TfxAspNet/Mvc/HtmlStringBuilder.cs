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

using System.Text;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
#else
using System.Web.Mvc;
#endif

namespace Xyz.TForce.AspNet.Mvc
{

  public class HtmlStringBuilder
  {

    private readonly StringBuilder _builder;

    public HtmlStringBuilder()
    {
      _builder = new StringBuilder();
    }

    public void Append(string str)
    {
      _ = _builder.Append(str);
    }

#if NETSTANDARD2_0
    public void Append(IHtmlContent htmlContent)
    {
      System.IO.StringWriter writer = new System.IO.StringWriter();
      htmlContent.WriteTo(writer, HtmlEncoder.Default);
      _ = _builder.Append(writer.ToString());
    }
#else
    public void Append(MvcHtmlString mvcString)
    {
      _ = _builder.Append(mvcString.ToString());
    }
#endif

#if NETSTANDARD2_0
    public HtmlString ToHtml()
    {
      return new HtmlString(_builder.ToString());
    }
#else
    public MvcHtmlString ToHtml()
    {
      return new MvcHtmlString(_builder.ToString());
    }
#endif

    public override string ToString()
    {
      return _builder.ToString();
    }
  }
}
