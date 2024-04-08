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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wjybxx.Commons;

#pragma warning disable CS1591
namespace Wjybxx.BTree.Branch;

/// <summary>
/// 非并行分支节点抽象（最多只有一个运行中的子节点）
///
/// 如果<see cref="Task{T}.Execute"/>方法是有循环体的，那么一定要注意：
/// 只有循环的尾部运行child才是安全的，如果在运行child后还读写其它数据，可能导致bug(小心递归)。
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingleRunningChildBranch<T> : BranchTask<T> where T : class
{
    /** 运行中的子节点 */
    [NonSerialized] protected Task<T>? runningChild;
    /** 运行中的子节点索引 */
    [NonSerialized] protected int runningIndex = -1;

    protected SingleRunningChildBranch() {
    }

    protected SingleRunningChildBranch(List<Task<T>>? children) : base(children) {
    }

    protected SingleRunningChildBranch(Task<T> first, Task<T>? second) : base(first, second) {
    }

    /** 允许外部在结束后查询 */
    public int GetRunningIndex() {
        return runningIndex;
    }

    /** 已完成的子节点数量 */
    public int GetCompletedCount() {
        return runningIndex + 1;
    }

    public override bool IsAllChildCompleted() {
        return runningIndex + 1 >= children.Count;
    }
    //

    #region logic

    public override void ResetForRestart() {
        base.ResetForRestart();
        runningChild = null;
        runningIndex = -1;
    }

    protected override void BeforeEnter() {
        // 这里不调用super是安全的
        runningChild = null;
        runningIndex = -1;
    }

    protected override void Exit() {
        // index不立即重置，允许返回后查询
        runningChild = null;
    }

    protected override void StopRunningChildren() {
        Stop(runningChild);
    }

    protected override void OnEventImpl(object eventObj) {
        if (runningChild != null) {
            runningChild.OnEvent(eventObj);
        }
    }

    protected override void Execute() {
        int reentryId = GetReentryId();
        Task<T> runningChild = this.runningChild;
        for (int i = 0, retryCount = children.Count; i < retryCount; i++) { // 避免死循环
            if (runningChild == null) {
                this.runningChild = runningChild = nextChild();
            }
            template_runChild(runningChild);
            if (CheckCancel(reentryId)) { // 得出结果或被取消
                return;
            }
            if (runningChild.IsRunning) { // 子节点未结束
                return;
            }
            runningChild = null;
        }
        throw new IllegalStateException(IllegalStateMsg());
    }

    protected Task<T> nextChild() {
        // 避免状态错误的情况下修改了index
        int nextIndex = runningIndex + 1;
        if (nextIndex < children.Count) {
            runningIndex = nextIndex;
            return children[nextIndex];
        }
        throw new IllegalStateException(IllegalStateMsg());
    }

    /** 没有可继续运行的子节点 */
    protected string IllegalStateMsg() {
        return $"numChildren: {children.Count}, currentIndex: {runningIndex}";
    }

    protected override void OnChildRunning(Task<T> child) {
        runningChild = child; // 部分实现可能未在选择child之后就赋值
    }

    /// <summary>
    ///  子类的实现模板：
    /// <code>
    ///  protected void OnChildCompleted(Task child) {
    ///     runningChild = null;
    ///     // 尝试计算结果（记得处理取消）
    ///      ...
    ///      // 如果未得出结果
    ///      if (!isExecuting()) {
    ///         template_execute();
    ///     }
    ///  }
    /// </code> 
    /// </summary>
    /// <param name="child"></param>
    protected override void OnChildCompleted(Task<T> child) {
        Debug.Assert(child == runningChild);
        runningChild = null; // 子类可直接重写此句以不调用super
    }

    #endregion
}