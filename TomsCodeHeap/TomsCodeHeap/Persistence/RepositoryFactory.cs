// ========================================================================
// File:     RepositoryFactory.cs
// 
// Author:   $Author$
// Date:     $LastChangeDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2011] [$Author$]
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================
          
using System;

namespace CH.Froorider.Codeheap.Persistence
{
    /// <summary>
    /// Creates instances of <see cref="IRepository"/> according to the given <see cref="RepositoryType"/>.
    /// </summary>
    public class RepositoryFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class.
        /// </summary>
        /// <remarks>Is private to ensure nobody can create an instance of this class accidentally.</remarks>
        private RepositoryFactory()
        {
        }

        /// <summary>
        /// Creates the <see cref="IRepository"/> instance to a given <see cref="RepositoryType"/>.
        /// </summary>
        /// <param name="typeToCreate">The type of <see cref="IRepository"/> to create.</param>
        /// <returns>
        /// An instance of <see cref="IRepository"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="typeToCreate"/> is not a valid <see cref="RepositoryType"/> value.</exception>
        /// <exception cref="NotSupportedException">Is thrown when no <see cref="IRepository"/> instance can be created to the passed <paramref name="typeToCreate"/>.</exception>
        public static IRepository CreateRepository(RepositoryType typeToCreate)
        {
            if (!Enum.IsDefined(typeof(RepositoryType), typeToCreate))
            {
                throw new ArgumentException("Passed RepositoryType value is not defined in the enumeration.", "typeToCreate");
            }

            switch (typeToCreate)
            {
                case RepositoryType.FileRepository:
                    return new FileRepository();
                case RepositoryType.Undefined:
                default:
                    throw new NotSupportedException("Not supported repository type: " + typeToCreate.ToString());
            }
        }
    }
}
