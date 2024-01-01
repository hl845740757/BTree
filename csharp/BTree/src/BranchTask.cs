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


using System.Collections.Generic;
using Wjybxx.Commons.Collections;

namespace Wjybxx.BTree;

/// <summary>
/// 分支节点抽象
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BranchTask<T> : Task<T>
{
    protected List<Task<T>> children;

    protected BranchTask() {
        children = new List<Task<T>>();
    }

    protected BranchTask(List<Task<T>>? children) {
        this.children = children ?? new List<Task<T>>();
    }

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
}