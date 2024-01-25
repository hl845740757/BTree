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

#pragma warning disable CS1591
namespace Wjybxx.BTree.Branch.Join;

/// <summary>
/// Join版本的SelectorN
/// </summary>
/// <typeparam name="T"></typeparam>
public class JoinSelectorN<T> : JoinPolicy<T>
{
    /** 需要达成的次数 */
    private int required = 1;
    /** 是否快速失败 */
    private bool failFast;

    public JoinSelectorN() {
    }

    public JoinSelectorN(int required, bool failFast = false) {
        this.required = required;
        this.failFast = failFast;
    }

    public void resetForRestart() {
    }

    public void beforeEnter(Join<T> join) {
    }

    public void enter(Join<T> join) {
        if (required <= 0) {
            join.setSuccess();
        } else if (join.getChildCount() == 0) {
            join.setFailed(Status.CHILDLESS);
        } else if (CheckFailFast(join)) {
            join.setFailed(Status.INSUFFICIENT_CHILD);
        }
    }

    public void onChildCompleted(Join<T> join, Task<T> child) {
        if (join.getSucceededCount() >= required) {
            join.setSuccess();
        } else if (join.isAllChildCompleted() || CheckFailFast(join)) {
            join.setFailed(Status.ERROR);
        }
    }

    private bool CheckFailFast(Join<T> join) {
        return failFast && (join.getChildCount() - join.getCompletedCount()) < required - join.getSucceededCount();
    }

    public void onEvent(Join<T> join, object eventObj) {
    }

    public int Required {
        get => required;
        set => required = value;
    }
    public bool FailFast {
        get => failFast;
        set => failFast = value;
    }
}