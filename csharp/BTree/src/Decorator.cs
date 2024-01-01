#region LICENSE

// Copyright 2023 wjybxx(845740757@qq.com)
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

namespace Wjybxx.BTree;

/// <summary>
/// 装饰节点基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Decorator<T> : Task<T>
{
    protected Task<T>? child;

    public Decorator() {
    }

    public Decorator(Task<T> child) {
        this.child = child;
    }

    protected override void stopRunningChildren() {
        stop(child);
    }

    protected override void onChildRunning(Task<T> child) {
    }

    protected override void onEventImpl(object eventObj) {
        if (child != null) {
            child.onEvent(eventObj);
        }
    }

    #region child

    public sealed override int indexChild(Task<T>? task) {
        if (task != null && task == this.child) {
            return 0;
        }
        return -1;
    }

    public sealed override List<Task<T>> ListChildren() {
        return child == null ? new List<Task<T>>(0) : new List<Task<T>> { child };
    }

    public sealed override int getChildCount() {
        return child == null ? 0 : 1;
    }

    public sealed override Task<T> getChild(int index) {
        if (index == 0 && child != null) {
            return child;
        }
        throw new IndexOutOfRangeException(index.ToString());
    }

    protected sealed override int addChildImpl(Task<T> task) {
        if (child != null) {
            throw new IllegalStateException("A task entry cannot have more than one child");
        }
        child = task;
        return 0;
    }

    protected sealed override Task<T> setChildImpl(int index, Task<T> task) {
        if (index == 0 && child != null) {
            Task<T> r = this.child;
            child = task;
            return r;
        }
        throw new IndexOutOfRangeException(index.ToString());
    }

    protected sealed override Task<T> removeChildImpl(int index) {
        if (index == 0 && child != null) {
            Task<T> r = this.child;
            child = null;
            return r;
        }
        throw new IndexOutOfRangeException(index.ToString());
    }

    # endregion

    #region 序列化

    public Task<T>? Child {
        get => child;
        set => child = value;
    }

    #endregion
}