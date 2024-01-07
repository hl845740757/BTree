﻿#region LICENSE

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
/// 只执行一次。
/// 1.适用那些不论成功与否只执行一次的行为。
/// 2.在调用<see cref="Task{T}.resetForRestart()"/>后可再次运行。
/// </summary>
/// <typeparam name="T"></typeparam>
public class OnlyOnce<T> : Decorator<T>
{
    public OnlyOnce() {
    }

    public OnlyOnce(Task<T> child) : base(child) {
    }

    protected override void execute() {
        if (child.IsCompleted()) {
            setCompleted(child.GetStatus(), true);
        } else {
            template_runChild(child);
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        setCompleted(child.GetStatus(), true);
    }
}