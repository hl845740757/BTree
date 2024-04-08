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
using System.Diagnostics;

#pragma warning disable CS1591
namespace Wjybxx.BTree.Branch;

/// <summary>
/// 简单并发节点。
/// 1.其中第一个任务为主要任务，其余任务为次要任务l；
/// 2.一旦主要任务完成，则节点进入完成状态。
/// 3.外部事件将派发给主要任务。
/// </summary>
/// <typeparam name="T"></typeparam>
public class SimpleParallel<T> : Parallel<T> where T : class
{
    protected override void Execute() {
        List<Task<T>> children = this.children;
        Task<T> mainTask = children[0];

        int reentryId = GetReentryId();
        template_runChild(mainTask);
        if (CheckCancel(reentryId)) { // 得出结果或取消
            return;
        }

        for (int idx = 1; idx < children.Count; idx++) {
            Task<T> child = children[idx];
            template_runHook(child);
            if (CheckCancel(reentryId)) { // 得出结果或取消
                return;
            }
        }
    }

    protected override void OnChildCompleted(Task<T> child) {
        Debug.Assert(child == children[0]);
        SetCompleted(child.GetStatus(), true);
    }

    protected override void OnEventImpl(object eventObj) {
        children[0].OnEvent(eventObj);
    }
}