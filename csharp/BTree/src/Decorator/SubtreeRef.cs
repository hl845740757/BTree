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
namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 子树引用
/// </summary>
/// <typeparam name="T"></typeparam>
public class SubtreeRef<T> : Decorator<T> where T : class
{
#nullable disable
    private string subtreeName;
#nullable enable

    public SubtreeRef() {
    }

    public SubtreeRef(string subtreeName) {
        this.subtreeName = subtreeName;
    }

    protected override void Enter(int reentryId) {
        if (child == null) {
            Task<T> rootTask = TaskEntry.TreeLoader.LoadRootTask<T>(subtreeName);
            AddChild(rootTask);
        }
    }

    protected override void Execute() {
        template_runChild(child);
    }

    protected override void OnChildCompleted(Task<T> child) {
        SetCompleted(child.GetStatus(), true);
    }

    /// <summary>
    /// 子树的名字
    /// </summary>
    public string SubtreeName {
        get => subtreeName;
        set => subtreeName = value;
    }
}