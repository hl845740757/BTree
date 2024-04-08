﻿#region LICENSE

// Copyright 2024 wjybxx(845740757@qq.com)
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

using System.Collections.Generic;

#pragma warning disable CS1591
namespace Wjybxx.BTree.Branch;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Selector<T> : SingleRunningChildBranch<T> where T : class
{
    public Selector() {
    }

    public Selector(List<Task<T>>? children) : base(children) {
    }

    public Selector(Task<T> first, Task<T>? second) : base(first, second) {
    }

    protected override void OnChildCompleted(Task<T> child) {
        runningChild = null;
        if (child.IsCancelled()) {
            SetCancelled();
            return;
        }
        if (child.IsSucceeded()) {
            SetSuccess();
        } else if (IsAllChildCompleted()) {
            SetFailed(Status.ERROR);
        } else if (!IsExecuting()) {
            template_execute();
        }
    }
}