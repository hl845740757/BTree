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


using System;
using System.Collections.Generic;
using Wjybxx.Commons.Collections;

namespace Wjybxx.BTree;

/// <summary>
/// 分支节点抽象
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BranchTask<T> : Task<T>
{
#nullable disable
    protected List<Task<T>> children;
#nullable enable

    protected BranchTask() {
        children = new List<Task<T>>();
    }

    protected BranchTask(List<Task<T>>? children) {
        this.children = children ?? new List<Task<T>>();
    }

    protected BranchTask(Task<T> first, Task<T>? second) {
        if (first == null) throw new ArgumentNullException(nameof(first));
        children = new List<Task<T>>(2);
        children.Add(first);
        if (second != null) {
            children.Add(second);
        }
    }

    #region

    /** 是否是第一个子节点 */
    public bool isFirstChild(Task<T> child) {
        int count = this.children.Count;
        if (count == 0) {
            return false;
        }
        return this.children[0] == child;
    }

    /** 是否是第最后一个子节点 */
    public bool isLastChild(Task<T> child) {
        int count = this.children.Count;
        if (count == 0) {
            return false;
        }
        return children[count - 1] == child;
    }

    /** 获取第一个子节点 -- 主要为MainPolicy提供帮助 */
    public Task<T>? getFirstChild() {
        int size = children.Count;
        return size > 0 ? children[0] : null;
    }

    /** 获取最后一个子节点 */
    public Task<T>? getLastChild() {
        int size = children.Count;
        return size > 0 ? children[size - 1] : null;
    }

    /** 是否所有的子节点已进入完成状态 */
    public virtual bool isAllChildCompleted() {
        // 在判断是否全部完成这件事上，逆序遍历有优势
        for (int idx = children.Count - 1; idx >= 0; idx--) {
            Task<T> child = children[idx];
            if (child.IsRunning()) {
                return false;
            }
        }
        return true;
    }

    /** 用于避免测试的子节点过于规律 */
    internal void ShuffleChild() {
        CollectionUtil.Shuffle(children);
    }

    #endregion

    #region child

    public sealed override void removeAllChild() {
        children.ForEach(e => e.unsetControl());
        children.Clear();
    }

    public sealed override int indexChild(Task<T> task) {
        return CollectionUtil.IndexOfRef(children, task);
    }

    public sealed override List<Task<T>> ListChildren() {
        return new List<Task<T>>(children);
    }

    public sealed override int getChildCount() {
        return children.Count;
    }

    public sealed override Task<T> getChild(int index) {
        return children[index];
    }

    protected sealed override int addChildImpl(Task<T> task) {
        children.Add(task);
        return children.Count - 1;
    }

    protected sealed override Task<T> setChildImpl(int index, Task<T> task) {
        return children[index] = task;
    }

    protected sealed override Task<T> removeChildImpl(int index) {
        Task<T> child = children[index];
        children.RemoveAt(index);
        return child;
    }

    #endregion

    /// <summary>
    /// 该接口仅用于序列化
    /// </summary>
    public List<Task<T>>? Children {
        get => children;
        set => children = value ?? new List<Task<T>>();
    }
}