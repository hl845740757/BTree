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

/// <summary>
/// 状态切换参数
/// 建议用户通过原型对象的{@link #withExtraInfo(object)}等方法创建
/// </summary>
public class ChangeStateArgs
{
    public const int CMD_NONE = 0;
    public const int CMD_UNDO = 1;
    public const int CMD_REDO = 2;

    /** 不延迟 */
    public const int DELAY_NONE = 0;
    /** 在当前子节点完成的时候切换 -- 其它延迟模式也会在状态完成时触发；通常用于状态主动退出时； */
    public const int DELAY_CURRENT_COMPLETED = 1;
    /** 下一帧执行 */
    public const int DELAY_NEXT_FRAME = 2;

    #region 共享原型

    public static readonly ChangeStateArgs PLAIN = new(0, 0, 0, null);
    public static readonly ChangeStateArgs PLAIN_WHEN_COMPLETED = new(0, DELAY_CURRENT_COMPLETED, 0, null);
    public static readonly ChangeStateArgs PLAIN_NEXT_FRAME = new(0, DELAY_NEXT_FRAME, -1, null);

    public static readonly ChangeStateArgs UNDO = new(CMD_UNDO, 0, 0, null);
    public static readonly ChangeStateArgs UNDO_WHEN_COMPLETED = new(CMD_UNDO, DELAY_CURRENT_COMPLETED, 0, null);
    public static readonly ChangeStateArgs UNDO_NEXT_FRAME = new(CMD_UNDO, DELAY_NEXT_FRAME, -1, null);

    public static readonly ChangeStateArgs REDO = new(CMD_REDO, 0, 0, null);
    public static readonly ChangeStateArgs REDO_WHEN_COMPLETED = new(CMD_REDO, DELAY_CURRENT_COMPLETED, 0, null);
    public static readonly ChangeStateArgs REDO_NEXT_FRAME = new(CMD_REDO, DELAY_NEXT_FRAME, -1, null);

    #endregion

    /** 切换命名 */
    public readonly int cmd;

    /** 延迟模式 */
    public readonly int delayMode;

    /** 期望开始运行的帧号；-1表示尚未指定 */
    public readonly int frame;

    /** 期望传递给Listener的数据 */
    public readonly object extraInfo;

    /** 通过原型对象创建 */
    private ChangeStateArgs(int cmd, int delayMode, int frame, object extraInfo) {
//        checkCmd(cmd); // 封闭构造方法后可不校验
        checkDelayMode(delayMode);
        this.delayMode = delayMode;
        this.cmd = cmd;
        this.frame = frame;
        this.extraInfo = extraInfo;
    }

    public bool isPlain() {
        return cmd == 0;
    }

    public bool isUndo() {
        return cmd == CMD_UNDO;
    }

    public bool isRedo() {
        return cmd == CMD_REDO;
    }

    // region 原型方法

    public ChangeStateArgs withDelayMode(int delayMode) {
        if (delayMode == this.delayMode) {
            return this;
        }
        return new ChangeStateArgs(cmd, delayMode, frame, extraInfo);
    }

    public ChangeStateArgs withFrame(int frame) {
        if (frame == this.frame) {
            return this;
        }
        return new ChangeStateArgs(cmd, delayMode, frame, extraInfo);
    }

    public ChangeStateArgs withExtraInfo(Object extraInfo) {
        if (extraInfo == this.extraInfo) {
            return this;
        }
        return new ChangeStateArgs(cmd, delayMode, frame, extraInfo);
    }
    // endregion

    private static void checkCmd(int cmd) {
        if (cmd < CMD_NONE || cmd > CMD_REDO) {
            throw new ArgumentException("cmd: " + cmd);
        }
    }

    private static void checkDelayMode(int delayMode) {
        if (delayMode < DELAY_NONE || delayMode > DELAY_NEXT_FRAME) {
            throw new ArgumentException("invalid delayMode: " + delayMode);
        }
    }
}