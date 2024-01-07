#region LICENSE

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

namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 子树引用
/// </summary>
/// <typeparam name="T"></typeparam>
public class SubtreeRef<T> : Decorator<T>
{
#nullable disable
    private string subtreeName;
#nullable enable

    public SubtreeRef() {
    }

    public SubtreeRef(string subtreeName) {
        this.subtreeName = subtreeName;
    }

    protected override void enter(int reentryId) {
        if (child == null) {
            Task<T> rootTask = GetTaskEntry().TreeLoader.loadRootTask<T>(subtreeName);
            addChild(rootTask);
        }
    }

    protected override void execute() {
        template_runChild(child);
    }

    protected override void onChildCompleted(Task<T> child) {
        setCompleted(child.GetStatus(), true);
    }

    /// <summary>
    /// 子树的名字
    /// </summary>
    public string SubtreeName {
        get => subtreeName;
        set => subtreeName = value;
    }
}