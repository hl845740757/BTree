#region LICENSE

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
/// Switch-选择一个分支运行，直到其结束
/// </summary>
/// <typeparam name="T"></typeparam>
public class Switch<T> : SingleRunningChildBranch<T> where T : class
{
    public Switch() {
    }

    public Switch(List<Task<T>>? children) : base(children) {
    }

    protected override void Execute() {
        if (runningChild == null && !selectChild()) {
            SetFailed(Status.ERROR);
            return;
        }
        template_runChildDirectly(runningChild!);
    }

    private bool selectChild() {
        for (int idx = 0; idx < children.Count; idx++) {
            Task<T> child = children[idx];
            if (!template_checkGuard(child.GetGuard())) {
                child.SetGuardFailed(null); // 不接收通知
                continue;
            }
            this.runningChild = child;
            this.runningIndex = idx;
            return true;
        }
        return false;
    }

    protected override void OnChildCompleted(Task<T> child) {
        runningChild = null;
        SetCompleted(child.GetStatus(), true);
    }
}