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
/// 主动选择节点
/// 每次运行时都会重新测试节点的运行条件，选择一个新的可运行节点。
/// 如果新选择的运行节点与之前的运行节点不同，则取消之前的任务。
/// </summary>
/// <typeparam name="T"></typeparam>
public class ActiveSelector<T> : SingleRunningChildBranch<T>
{
    public ActiveSelector() {
    }

    public ActiveSelector(List<Task<T>>? children) : base(children) {
    }

    protected override void execute() {
        Task<T> childToRun = null;
        int childIndex = -1;
        for (int idx = 0; idx < children.Count; idx++) {
            Task<T> child = children[idx];
            if (!template_checkGuard(child.GetGuard())) {
                child.setGuardFailed(null); // 不接收通知
                continue;
            }
            childToRun = child;
            childIndex = idx;
            break;
        }

        Task<T>? runningChild = this.runningChild;
        if (runningChild != null && runningChild != childToRun) {
            runningChild.stop();
            this.runningChild = null;
            this.runningIndex = -1;
        }

        if (childToRun == null) {
            setFailed(Status.ERROR);
            return;
        }

        this.runningChild = childToRun;
        this.runningIndex = childIndex;
        template_runChildDirectly(childToRun);
    }

    protected override void onChildCompleted(Task<T> child) {
        runningChild = null;
        setCompleted(child.GetStatus(), true);
    }
}