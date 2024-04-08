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

#pragma warning disable CS1591
namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 循环节点抽象
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class LoopDecorator<T> : Decorator<T> where T : class
{
    /** 每帧最大循环次数 - 避免死循环和占用较多CPU；默认1 */
    protected int maxLoopPerFrame = 1;

    protected LoopDecorator() {
    }

    protected LoopDecorator(Task<T> child) : base(child) {
    }

    protected override void Execute() {
        if (maxLoopPerFrame < 1) {
            SetFailed(TaskStatus.ERROR);
            return;
        }
        if (maxLoopPerFrame == 1) {
            Template_RunChild(child);
            return;
        }
        int reentryId = ReentryId;
        for (int _i = maxLoopPerFrame - 1; _i >= 0; _i--) {
            Template_RunChild(child);
            if (CheckCancel(reentryId)) {
                return;
            }
            if (child.IsRunning) { // 子节点未完成
                return;
            }
        }
    }

    /// <summary>
    /// 每帧循环次数
    /// </summary>
    public int MaxLoopPerFrame {
        get => maxLoopPerFrame;
        set => maxLoopPerFrame = value;
    }
}