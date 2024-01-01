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
/// 每一帧都检查子节点的前置条件，如果前置条件失败，则取消child执行并返回失败
/// </summary>
/// <typeparam name="T"></typeparam>
public class AlwaysCheckGuard<T> : Decorator<T>
{
    protected override void execute() {
        if (template_checkGuard(child!.GetGuard())) {
            template_runChildDirectly(child);
        } else {
            child.stop();
            setFailed(Status.ERROR);
        }
    }

    protected override void onChildCompleted(Task<T> child) {
        setCompleted(child.GetStatus(), true);
    }
}