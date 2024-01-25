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
namespace Wjybxx.BTree.Branch.Join;

/// <summary>
/// Join版本的Selector
/// </summary>
/// <typeparam name="T"></typeparam>
public class JoinSelector<T> : JoinPolicy<T>
{
    public void resetForRestart() {
    }

    public void beforeEnter(Join<T> join) {
    }

    public void enter(Join<T> join) {
        if (join.getChildCount() == 0) {
            join.setFailed(Status.CHILDLESS);
        }
    }

    public void onChildCompleted(Join<T> join, Task<T> child) {
        if (child.IsSucceeded()) {
            join.setSuccess();
        } else if (join.isAllChildCompleted()) {
            join.setFailed(Status.ERROR);
        }
    }

    public void onEvent(Join<T> join, object eventObj) {
    }
}