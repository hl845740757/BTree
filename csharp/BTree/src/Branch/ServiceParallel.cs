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
/// 服务并发节点
/// 1.其中第一个任务为主要任务，其余任务为后台服务。
/// 2.每次所有任务都会执行一次，但总是根据第一个任务执行结果返回结果。
/// 3.外部事件将派发给主要任务。
/// </summary>
/// <typeparam name="T"></typeparam>
public class ServiceParallel<T> : Parallel<T>
{
    public ServiceParallel() {
    }

    public ServiceParallel(List<Task<T>>? children) : base(children) {
    }

    protected override void execute() {
        List<Task<T>> children = this.children;
        Task<T> mainTask = children[0];
        template_runChild(mainTask);

        for (int idx = 1; idx < children.Count; idx++) {
            Task<T> child = children[idx];
            template_runHook(child);
        }
        if (mainTask.IsCompleted()) {
            setCompleted(mainTask.GetStatus(), true);
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        Debug.Assert(child == children[0]);
        if (!isExecuting()) {
            setSuccess();
        }
    }

    protected override void onEventImpl(object eventObj) {
        children[0].onEvent(eventObj);
    }
}