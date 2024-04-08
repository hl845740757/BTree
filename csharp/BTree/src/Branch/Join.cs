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
using Wjybxx.BTree.Branch.Join;
using Wjybxx.Commons;

#pragma warning disable CS1591
namespace Wjybxx.BTree.Branch;

public class Join<T> : Parallel<T> where T : class
{
#nullable disable
    protected JoinPolicy<T> policy;

    /** 子节点的重入id -- 判断本轮是否需要执行 */
    [NonSerialized] protected int[] childPrevReentryIds;
    /** 已进入完成状态的子节点 */
    [NonSerialized] protected int completedCount;
    /** 成功完成的子节点 */
    [NonSerialized] protected int succeededCount;
#nullable enable

    public Join() {
    }

    public Join(List<Task<T>>? children) : base(children) {
    }

    public override void ResetForRestart() {
        base.ResetForRestart();
        completedCount = 0;
        succeededCount = 0;
        policy.ResetForRestart();
    }

    protected override void BeforeEnter() {
        if (policy == null) {
            policy = JoinSequence<T>.GetInstance();
        }
        completedCount = 0;
        succeededCount = 0;
        // policy的数据重置
        policy.BeforeEnter(this);
    }

    protected override void Enter(int reentryId) {
        // 记录子类上下文 -- 由于beforeEnter可能改变子节点信息，因此在enter时处理
        RecordContext();
        policy.Enter(this);
    }

    private void RecordContext() {
        List<Task<T>> children = this.children;
        if (childPrevReentryIds == null || childPrevReentryIds.Length != children.Count) {
            childPrevReentryIds = new int[children.Count];
        }
        for (int i = 0; i < children.Count; i++) {
            Task<T> child = children[i];
            child.CancelToken = cancelToken.NewChild(); // child默认可读取取消
            childPrevReentryIds[i] = child.ReentryId;
        }
    }

    protected override void Execute() {
        List<Task<T>> children = this.children;
        if (children.Count == 0) {
            return;
        }
        int[] childPrevReentryIds = this.childPrevReentryIds;
        int reentryId = ReentryId;
        for (int i = 0; i < children.Count; i++) {
            Task<T> child = children[i];
            bool started = child.IsExited(childPrevReentryIds[i]);
            if (started && child.IsCompleted) { // 勿轻易调整
                continue;
            }
            Template_RunChild(child);
            if (CheckCancel(reentryId)) {
                return;
            }
        }
        if (completedCount >= children.Count) { // child全部执行，但没得出结果
            throw new IllegalStateException();
        }
    }

    protected override void OnChildCompleted(Task<T> child) {
        completedCount++;
        if (child.IsSucceeded) {
            succeededCount++;
        }
        cancelToken.Unregister(child.CancelToken); // 删除分配的token
        child.CancelToken.Reset();
        child.CancelToken = null;

        policy.OnChildCompleted(this, child);
    }

    protected override void OnEventImpl(object eventObj) {
        policy.OnEvent(this, eventObj);
    }

    // region
    public override bool IsAllChildCompleted() {
        return completedCount >= children.Count;
    }

    public bool IsAllChildSucceeded() {
        return succeededCount >= children.Count;
    }

    public int GetCompletedCount() {
        return completedCount;
    }

    public int GetSucceededCount() {
        return succeededCount;
    }
    // endregion

    /// <summary>
    /// join扩展策略
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public JoinPolicy<T> Policy {
        get => policy;
        set => policy = value ?? throw new ArgumentNullException();
    }
}