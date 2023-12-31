﻿#region LICENSE

// Copyright 2023-2024 wjybxx(845740757@qq.com)
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
using Wjybxx.BTree.Leaf;

namespace Wjybxx.BTree.Branch;

/// <summary>
/// 多选Selector。
/// 如果{required}小于等于0，则等同于<see cref="Success{T}"/>
/// 如果{required}等于1，则等同于<see cref="Selector{T}"/>
/// 如果{required}等于<code>children.code</code>，则在所有child成功之后成功 -- 默认不会提前失败。
/// 如果{required}大于<code>children.size</code>，则在所有child运行完成之后失败 -- 默认不会提前失败。
/// </summary>
/// <typeparam name="T"></typeparam>
public class SelectorN<T> : SingleRunningChildBranch<T>
{
    /** 需要达成的次数 */
    private int required = 1;
    /** 是否快速失败 */
    private bool failFast;
    /** 当前计数 */
    [NonSerialized] private int count;

    public void resetForRestart() {
        base.resetForRestart();
        count = 0;
    }

    protected override void beforeEnter() {
        base.beforeEnter();
        count = 0;
    }

    protected override void enter(int reentryId) {
        base.enter(reentryId);
        if (required < 1) {
            setSuccess();
        } else if (getChildCount() == 0) {
            setFailed(Status.CHILDLESS);
        } else if (checkFailFast()) {
            setFailed(Status.INSUFFICIENT_CHILD);
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        runningChild = null;
        if (child.IsCancelled()) {
            setCancelled();
            return;
        }
        if (child.IsSucceeded() && ++count >= required) {
            setSuccess();
        } else if (isAllChildCompleted() || checkFailFast()) {
            setFailed(Status.ERROR);
        } else if (!isExecuting()) {
            template_execute();
        }
    }

    private bool checkFailFast() {
        return failFast && (children.Count - getCompletedCount() < required - count);
    }

    //
    public int Required {
        get => required;
        set => required = value;
    }
    public bool FailFast {
        get => failFast;
        set => failFast = value;
    }
}