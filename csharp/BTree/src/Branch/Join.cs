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
using Wjybxx.Commons;

#pragma warning disable CS1591
namespace Wjybxx.BTree.Branch;

public class Join<T> : Parallel<T>
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

    public override void resetForRestart() {
        base.resetForRestart();
        completedCount = 0;
        succeededCount = 0;
        policy.resetForRestart();
    }

    protected override void beforeEnter() {
        if (policy == null) {
            policy = JoinSequence.getInstance();
        }
        completedCount = 0;
        succeededCount = 0;
        // policy的数据重置
        policy.beforeEnter(this);
    }

    protected override void enter(int reentryId) {
        // 记录子类上下文 -- 由于beforeEnter可能改变子节点信息，因此在enter时处理
        recordContext();
        policy.enter(this);
    }

    private void recordContext() {
        List<Task<T>> children = this.children;
        if (childPrevReentryIds == null || childPrevReentryIds.Length != children.Count) {
            childPrevReentryIds = new int[children.Count];
        }
        for (int i = 0; i < children.Count; i++) {
            Task<T> child = children[i];
            child.CancelToken = cancelToken.newChild(); // child默认可读取取消
            childPrevReentryIds[i] = child.getReentryId();
        }
    }

    protected override void execute() {
        List<Task<T>> children = this.children;
        if (children.Count == 0) {
            return;
        }
        int[] childPrevReentryIds = this.childPrevReentryIds;
        int reentryId = getReentryId();
        for (int i = 0; i < children.Count; i++) {
            Task<T> child = children[i];
            bool started = child.isExited(childPrevReentryIds[i]);
            if (started && child.IsCompleted()) { // 勿轻易调整
                continue;
            }
            template_runChild(child);
            if (checkCancel(reentryId)) {
                return;
            }
        }
        if (completedCount >= children.Count) { // child全部执行，但没得出结果
            throw new IllegalStateException();
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        completedCount++;
        if (child.IsSucceeded()) {
            succeededCount++;
        }
        cancelToken.unregister(child.CancelToken); // 删除分配的token
        child.CancelToken.reset();
        child.CancelToken = null;

        policy.onChildCompleted(this, child);
    }

    protected override void onEventImpl(object eventObj) {
        policy.onEvent(this, eventObj);
    }

    // region
    public override bool isAllChildCompleted() {
        return completedCount >= children.Count;
    }

    public bool isAllChildSucceeded() {
        return succeededCount >= children.Count;
    }

    public int getCompletedCount() {
        return completedCount;
    }

    public int getSucceededCount() {
        return succeededCount;
    }
    // endregion

    public JoinPolicy<T> getPolicy() {
        return policy;
    }

    public void setPolicy(JoinPolicy<T> policy) {
        this.policy = policy;
    }
}