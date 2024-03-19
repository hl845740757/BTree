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

namespace Wjybxx.BTree.FSM;

public class ChangeStateTask<T> : LeafTask<T>
{
    /** 下一个状态的guid -- 延迟加载 */
    private string? nextStateGuid;
    /** 下一个状态的对象缓存，通常延迟加载以避免循环引用 */
    [NonSerialized] private Task<T>? nextState;
    /** 目标状态的属性 */
    private object? stateProps;
    /** 为当前状态设置结果 -- 用于避免当前状态进入被取消状态；使用该特性时避免curState为自身 */
    private int curStateResult;

    /** 目标状态机的名字，以允许切换更顶层的状态机 */
    private string? machineName;
    /** 延迟模式 */
    private int delayMode;

    public ChangeStateTask() {
    }

    public ChangeStateTask(Task<T> nextState) {
        this.nextState = nextState;
    }

    protected override void Execute() {
        if (nextState == null) {
            nextState = GetTaskEntry().TreeLoader.LoadRootTask<T>(nextStateGuid);
        }
        if (stateProps != null) {
            nextState.SharedProps = stateProps;
        }
        StateMachineTask<T> stateMachine = StateMachineTask<T>.FindStateMachine(this, machineName);
        Task<T> curState = stateMachine.GetCurState();
        // 在切换状态前将当前状态标记为成功或失败；只有延迟通知的情况下才可以设置状态的结果，否则状态机会切换到其它状态
        if (Status.IsCompleted(curStateResult) && curState != null && !curState.IsDisableDelayNotify()) {
            curState.SetCompleted(curStateResult, false);
        }

        if (!IsDisableDelayNotify()) {
            // 先设置成功，然后再切换状态，当前Task可保持为成功状态；
            // 记得先把nextState保存下来，因为会先执行exit；最好只在未禁用延迟通知的情况下采用
            SetSuccess();
            stateMachine.ChangeState(nextState, ChangeStateArgs.PLAIN.WithDelayMode(delayMode));
        } else {
            // 该路径基本不会走到，这里只是给个示例
            int reentryId = GetReentryId();
            stateMachine.ChangeState(nextState, ChangeStateArgs.PLAIN.WithDelayMode(delayMode));
            if (!IsExited(reentryId)) {
                SetSuccess();
            }
        }
    }

    protected override void OnEventImpl(object eventObj) {
    }

    public string? NextStateGuid {
        get => nextStateGuid;
        set => nextStateGuid = value;
    }

    public Task<T>? NextState {
        get => nextState;
        set => nextState = value;
    }

    public object? StateProps {
        get => stateProps;
        set => stateProps = value;
    }

    public int CurStateResult {
        get => curStateResult;
        set => curStateResult = value;
    }

    public string? MachineName {
        get => machineName;
        set => machineName = value;
    }

    public int DelayMode {
        get => delayMode;
        set => delayMode = value;
    }
}