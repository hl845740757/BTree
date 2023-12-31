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
using System.Diagnostics;
using Wjybxx.Commons;

#pragma warning disable CS1591
namespace Wjybxx.BTree;

/// <summary>
///任务入口（可联想程序的Main）
/// 
///1. 该实现并不是典型的行为树实现，而是更加通用的任务树，因此命名TaskEntry。
///2. 该类允许继承，以提供一些额外的方法，但核心方法是禁止重写的。
///3. Entry的数据尽量也保存在黑板中，尤其是绑定的实体（Entity），尽可能使业务逻辑仅依赖黑板即可完成。
///4. Entry默认不检查{@link #getGuard()}，如果需要由用户（逻辑上的control）检查。
///5. 如果要复用行为树，应当以树为单位整体复用，万莫以Task为单位复用 -- 节点之间的引用千丝万缕，容易内存泄漏。
///6. 该行为树虽然是事件驱动的，但心跳不是事件，仍需要每一帧调用{@link #update(int)}方法。
///7. 避免直接使用外部的{@link CancelToken}，可将Entry的Token注册为外部的Child -- {@link CancelToken#addChild(CancelToken)}。
/// 
/// </summary>
public class TaskEntry<T> : Task<T>
{
    /** 行为树的名字 */
    private string? name;
    /** 行为树的根节点 */
    private Task<T>? rootTask;
    /** 行为树的类型 -- 表示用途 */
    private int type;

    /** 行为树绑定的实体 -- 最好也存储在黑板里；这里的字段本是为了提高性能 */
    [NonSerialized] private object? entity;
    /** 行为树加载器 -- 用于加载Task或配置 */
    [NonSerialized] private ITreeLoader treeLoader;
    /** 当前帧号 */
    [NonSerialized] private int curFrame;
    /** 用于Entry的事件驱动 */
    [NonSerialized] private ITaskEntryHandler<T>? handler;

    public TaskEntry()
        : this(null, null) {
    }

    public TaskEntry(string? name, Task<T>? rootTask, object? entity = null, ITreeLoader? treeLoader = null) {
        this.name = name;
        this.rootTask = rootTask;
        this.type = type;
        this.entity = entity;
        this.treeLoader = treeLoader ?? ITreeLoader.NullLoader();
    }

    #region getter/setter

    public string? Name => name;
    public Task<T>? RootTask => rootTask;
    public int Type => type;
    public ITreeLoader TreeLoader => treeLoader;
    public ITaskEntryHandler<T>? Handler => handler;

    public void SetName(string? name) {
        this.name = name;
    }

    public void SetRootTask(Task<T>? rootTask) {
        this.rootTask = rootTask;
    }

    public void SetType(int type) {
        this.type = type;
    }

    public void SetEntity(object? entity) {
        this.entity = entity;
    }

    public void SetTreeLoader(ITreeLoader? treeLoader) {
        this.treeLoader = ObjectUtil.NullToDef(treeLoader, ITreeLoader.NullLoader());
    }

    public void SetHandler(ITaskEntryHandler<T> handler) {
        this.handler = handler;
    }

    // C#重写属性的时候不能增加set，还是有点麻烦
    public override object Entity => entity;

    public override int CurFrame => curFrame;

    #endregion

    #region logic

    /// <summary>
    /// 用户需要在每一帧调用该方法以驱动心跳逻辑
    /// </summary>
    /// <param name="curFrame">当前帧号</param>
    public void Update(int curFrame) {
        this.curFrame = curFrame;
        if (GetStatus() == Status.RUNNING) {
            template_execute();
        } else {
            Debug.Assert(IsInited());
            template_enterExecute(null, 0);
        }
    }

    protected override void execute() {
        template_runChild(rootTask);
    }

    protected override void onChildRunning(Task<T> child) {
    }

    protected override void onChildCompleted(Task<T> child) {
        setCompleted(child.GetStatus(), true);
        cancelToken.clear(); // 避免内存泄漏
        if (handler != null) {
            handler.OnCompleted(this);
        }
    }

    public override bool canHandleEvent(object eventObj) {
        if (IsRunning()) {
            return true;
        }
        return rootTask != null && blackboard != null; // 只测isInited的关键属性即可
    }

    protected override void onEventImpl(object eventObj) {
        rootTask.onEvent(eventObj);
    }

    public override void resetForRestart() {
        base.resetForRestart();
        cancelToken.clear();
        curFrame = 0;
    }

    internal bool IsInited() {
        return rootTask != null && blackboard != null && cancelToken != null && treeLoader != null;
    }

    #endregion

    #region child

    public sealed override int indexChild(Task<T>? task) {
        if (task != null && task == this.rootTask) {
            return 0;
        }
        return -1;
    }

    public override List<Task<T>> ListChildren() {
        return rootTask == null ? new List<Task<T>>() : new List<Task<T>> { rootTask };
    }

    public override int getChildCount() {
        return rootTask == null ? 0 : 1;
    }

    public override Task<T> getChild(int index) {
        if (index == 0 && rootTask != null) {
            return rootTask;
        }
        throw new IndexOutOfRangeException(index.ToString());
    }

    protected override int addChildImpl(Task<T> task) {
        if (rootTask != null) {
            throw new IllegalStateException("A task entry cannot have more than one child");
        }
        rootTask = task;
        return 0;
    }

    protected override Task<T> setChildImpl(int index, Task<T> task) {
        if (index == 0 && rootTask != null) {
            Task<T> r = this.rootTask;
            rootTask = task;
            return r;
        }
        throw new IndexOutOfRangeException(index.ToString());
    }

    protected override Task<T> removeChildImpl(int index) {
        if (index == 0 && rootTask != null) {
            Task<T> r = this.rootTask;
            rootTask = null;
            return r;
        }
        throw new IndexOutOfRangeException(index.ToString());
    }

    #endregion
}