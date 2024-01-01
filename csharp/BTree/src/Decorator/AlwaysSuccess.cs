﻿#region LICENSE

// Copyright 2023 wjybxx(845740757@qq.com)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

#pragma warning disable CS1591
namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 在子节点完成之后固定返回成功
/// </summary>
/// <typeparam name="T"></typeparam>
public class AlwaysSuccess<T> : Decorator<T>
{
    protected override void execute() {
        if (child == null) {
            setSuccess();
        } else {
            template_runChild(child);
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        setSuccess();
    }
}