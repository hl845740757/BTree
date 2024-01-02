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
using Wjybxx.Commons.Ex;

namespace Wjybxx.BTree;

/// <summary>
/// 叶子节点的超类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class LeafTask<T> : Task<T>
{
    protected sealed override void onChildRunning(Task<T> child) {
        throw new AssertionError();
    }

    protected sealed override void onChildCompleted(Task<T> child) {
        throw new AssertionError();
    }

    #region child

    public sealed override int indexChild(Task<T> task) {
        return -1;
    }

    public sealed override List<Task<T>> ListChildren() {
        return new List<Task<T>>(0);
    }

    public sealed override int getChildCount() {
        return 0;
    }

    public sealed override Task<T> getChild(int index) {
        throw new IndexOutOfRangeException("A leaf task can not have any child");
    }

    protected sealed override int addChildImpl(Task<T> task) {
        throw new IllegalStateException("A leaf task cannot have any children");
    }

    protected sealed override Task<T> setChildImpl(int index, Task<T> task) {
        throw new IllegalStateException("A leaf task cannot have any children");
    }

    protected sealed override Task<T> removeChildImpl(int index) {
        throw new IndexOutOfRangeException("A leaf task can not have any child");
    }

    #endregion
}