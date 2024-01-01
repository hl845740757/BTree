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

#pragma warning disable CS1591
namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 在子节点完成之后固定返回失败
/// </summary>
/// <typeparam name="T"></typeparam>
public class AlwaysFail<T> : Decorator<T>
{
    private int failureStatus;

    protected override void execute() {
        if (child == null) {
            setFailed(Status.ToFailure(failureStatus));
        } else {
            template_runChild(child);
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        setCompleted(Status.ToFailure(child.GetStatus()), true); // 错误码有传播的价值
    }

    /// <summary>
    /// 失败时使用的状态码
    /// </summary>
    public int FailureStatus {
        get => failureStatus;
        set => failureStatus = value;
    }
}