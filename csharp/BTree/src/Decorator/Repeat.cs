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

using System;

namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 重复N次
/// </summary>
/// <typeparam name="T"></typeparam>
public class Repeat<T> : LoopDecorator<T>
{
    /** 总是计数 */
    public const int ModeAlways = 0;
    /** 成功时计数 */
    public const int ModeOnlySuccess = 1;
    /** 失败时计数 */
    public const int ModeOnlyFailed = 2;
    /** 不计数 - 可能无限执行 */
    public const int ModeNever = 3;

    /** 考虑到Java枚举与其它语言的兼容性问题，我们在编辑器中使用数字 */
    private int countMode = ModeAlways;
    /** 需要重复的次数 */
    private int required = 1;
    /** 当前计数 */
    [NonSerialized] private int count;

    public override void resetForRestart() {
        base.resetForRestart();
        count = 0;
    }

    protected override void beforeEnter() {
        base.beforeEnter();
        count = 0;
    }

    protected override void enter(int reentryId) {
        base.enter(reentryId);
        if (required < 1) {
            setSuccess();
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        if (child.IsCancelled()) {
            setCancelled();
            return;
        }
        bool match = countMode switch // 还是更喜欢Java的switch...
        {
            ModeAlways => true,
            ModeOnlySuccess => child.IsSucceeded(),
            ModeOnlyFailed => child.IsFailed(),
            _ => false
        };
        if (match && ++count >= required) {
            setSuccess();
        }
    }

    #region 序列化

    /** 计数模式 */
    public int CountMode {
        get => countMode;
        set => countMode = value;
    }

    /** 期望的次数 */
    public int Required {
        get => required;
        set => required = value;
    }

    #endregion
}