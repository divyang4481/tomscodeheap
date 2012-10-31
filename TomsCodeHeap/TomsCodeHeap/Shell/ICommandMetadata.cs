// ========================================================================
// File:     ICommandMetadata.cs
// 
// Author:   
// Date:     $LastChangedDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2012] [$Author$]
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

namespace CH.Froorider.Codeheap.Shell
{
    using System.ComponentModel;

    public interface ICommandMetadata
    {
        [DefaultValue("NoName")]
        string Name
        {
            get;
            set;
        }

        [DefaultValue("NoName command has not been described by the lazy hacker yet.")]
        string Description
        {
            get;
            set;
        }
    }
}
