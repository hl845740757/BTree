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

using Wjybxx.Commons;

#pragma warning disable CS1591
namespace Wjybxx.BTree.Decorator;

/// <summary>
/// 反转装饰器，它用于反转子节点的执行结果。
/// 如果被装饰的任务失败，它将返回成功；
/// 如果被装饰的任务成功，它将返回失败；
/// 如果被装饰的任务取消，它将返回取消。
/// </summary>
/// <typeparam name="T"></typeparam>
public class Inverter<T> : Decorator<T> where T : class
{
    public Inverter() {
    }

    public Inverter(Task<T> child) : base(child) {
    }

    protected override void Execute() {
        template_runChild(child!);
    }

    protected override void OnChildCompleted(Task<T> child) {
        switch (child.NormalizedStatus) {
            case TaskStatus.SUCCESS: {
                SetFailed(TaskStatus.ERROR);
                break;
            }
            case TaskStatus.ERROR: {
                SetSuccess();
                break;
            }
            case TaskStatus.CANCELLED: {
                SetCancelled(); // 取消是个奇葩情况
                break;
            }
            default: throw new AssertionError();
        }
    }
}