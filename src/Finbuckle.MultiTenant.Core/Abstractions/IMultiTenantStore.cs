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

namespace Finbuckle.MultiTenant.Core.Abstractions
{
    /// <summary>
    /// Interface definition for tenant stores.
    /// </summary>
    public interface IMultiTenantStore
    {
        /// <summary>
        /// Try to add the <c>TenantContext</c> to the store.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<bool> TryAdd(TenantContext context);

        /// <summary>
        /// Try to remove the <c>TenantContext</c> from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<bool> TryRemove(string identifier);

        /// <summary>
        /// Retrieve the <c>TenantContext<c> for a given identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<TenantContext> GetByIdentifierAsync(string identifier);
    }
}