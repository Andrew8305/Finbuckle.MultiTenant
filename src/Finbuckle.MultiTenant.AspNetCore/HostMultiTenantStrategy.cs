//    Copyright 2018 Andrew White
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.AspNetCore;
using Microsoft.AspNetCore.Http;
using Finbuckle.MultiTenant.Core;
using Finbuckle.MultiTenant.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Finbuckle.MultiTenant.AspNetCore
{
    public class HostMultiTenantStrategy : IMultiTenantStrategy
    {
        private readonly string regex;
        private readonly ILogger<HostMultiTenantStrategy> logger;

        public HostMultiTenantStrategy(string template) : this(template, null)
        {
        }

        public HostMultiTenantStrategy(string template, ILogger<HostMultiTenantStrategy> logger)
        {
            // Check for valid template. Template cannot have "*" on each side of __tenant__ placeholder.
            if (string.IsNullOrWhiteSpace(template) ||
                Regex.Match(template, @"^.*\*.*\.__tenant__\..*\*.*$").Success)
            {
                throw new MultiTenantException("Invalid host template.");
            }

            template = template.Trim().Replace(".", @"\.");
            string wildcardSegmentsPattern = @"(\.[^\.]+)#";
            string singleSegmentPattern = @"[^\.]+";
            if (template.Substring(template.Length - 3, 3) == @"\.*")
            {
                template = template.Substring(0, template.Length - 3) + wildcardSegmentsPattern;
            }

            wildcardSegmentsPattern = @"([^\.]+\.)#";
            template = template.Replace(@"*\.", wildcardSegmentsPattern);
            template = template.Replace("?", singleSegmentPattern);
            template = template.Replace("__tenant__", @"(?<identifier>[^\.]+)");
            template = $"^{template}$".Replace("#", "*");

            this.regex = template;
            this.logger = logger;
        }

        public virtual string GetIdentifier(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException("\"context\" type must be of type HttpContext", nameof(context)));

            var host = (context as HttpContext).Request.Host;

            Utilities.TryLogInfo(logger, $"Host:  \"{host.Host ?? "<null>"}\"");

            if (host.HasValue == false)
                return null;

            string identifier = null;

            var match = Regex.Match(host.Host, regex,
                RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(100));

            if (match.Success)
            {
                identifier = match.Groups["identifier"].Value;
            }

            Utilities.TryLogInfo(logger, $"Found identifier:  \"{identifier ?? "<null>"}\"");

            return identifier;
        }
    }
}