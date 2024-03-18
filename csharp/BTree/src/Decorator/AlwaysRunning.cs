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

#pragma warning disable CS1591
namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 在子节点完成之后仍返回运行。
/// 注意：在运行期间只运行一次子节点
/// </summary>
/// <typeparam name="T"></typeparam>
public class AlwaysRunning<T> : Decorator<T>
{
    /** 记录子节点上次的重入id，这样不论enter和execute是否分开执行都不影响 */
    [NonSerialized] private int childPrevReentryId;

    public AlwaysRunning() {
    }

    public AlwaysRunning(Task<T> child) : base(child) {
    }

    protected override void BeforeEnter() {
        base.BeforeEnter();
        if (child == null) {
            childPrevReentryId = 0;
        } else {
            childPrevReentryId = child.GetReentryId();
        }
    }

    protected override void Execute() {
        if (child == null) {
            return;
        }
        bool started = child.IsExited(childPrevReentryId);
        if (started && child.IsCompleted()) { // 勿轻易调整
            return;
        }
        template_runChild(child);
    }

    protected override void OnChildCompleted(Task<T> child) {
        if (child.IsCancelled()) { // 不响应其它状态，但还是需要响应取消...
            SetCancelled();
        }
    }
}