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
/// 并行节点基类
/// 定义该类主要说明一些注意事项，包括：
/// 1.在处理子节点完成事件的时候，避免运行<see cref="Task{T}.Execute"/>方法，否则可能导致其它task单帧内运行多次。
/// 2.如果有缓存数据，务必小心维护。
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Parallel<T> : BranchTask<T> where T : class
{
    protected Parallel() {
    }

    protected Parallel(List<Task<T>>? children) : base(children) {
    }

    /**
     * 并发节点通常不需要在该事件中将自己更新为运行状态，而是应该在<see cref="Task{T}.Execute"/>方法的末尾更新
     */
    protected override void OnChildRunning(Task<T> child) {
    }
}