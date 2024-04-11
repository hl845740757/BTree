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
/// 循环子节点直到给定的条件达成
/// </summary>
/// <typeparam name="T"></typeparam>
public class UntilCond<T> : LoopDecorator<T> where T : class
{
    /** 循环条件 -- 不能直接使用child的guard，意义不同 */
    private Task<T>? cond;

    protected override void OnChildCompleted(Task<T> child) {
        if (Template_CheckGuard(cond)) {
            SetSuccess();
        }
    }

    /// <summary>
    /// 子节点的循条件
    /// </summary>
    public Task<T>? Cond {
        get => cond;
        set => cond = value;
    }
}