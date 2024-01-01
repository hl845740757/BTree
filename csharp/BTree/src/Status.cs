﻿#region LICENSE

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

namespace Wjybxx.BTree;

/// <summary>
/// Task的基础状态码
/// </summary>
public static class Status
{
    /** 初始状态 */
    public const int NEW = 0;
    /** 执行中 */
    public const int RUNNING = 1;
    /** 执行成功 -- 最小的完成状态 */
    public const int SUCCESS = 2;

    /** 被取消 -- 需要放在所有失败码的前面，用户可以可以通过比较大小判断；向上传播时要小心 */
    public const int CANCELLED = 3;
    /** 默认失败码 -- 是最小的失败码 */
    public const int ERROR = 4;
    /** 前置条件检查失败 -- 未运行的情况下直接失败；注意！该错误码不能向父节点传播 */
    public const int GUARD_FAILED = 5;
    /** 没有子节点 */
    public const int CHILDLESS = 6;
    /** 子节点不足 */
    public const int INSUFFICIENT_CHILD = 7;
    /** 执行超时 */
    public const int TIMEOUT = 8;

    /** 这是Task类能捕获的最大前一个状态的值，超过该值时将被修正该值 */
    public const int MAX_PREV_STATUS = 63;

    //
    /** 状态码是否表示运行中 */
    public static bool IsRunning(int status) {
        return status == Status.RUNNING;
    }

    /** 状态码是否表示已完成 */
    public static bool IsCompleted(int status) {
        return status >= Status.SUCCESS;
    }

    /** 状态码是否表示成功 */
    public static bool IsSucceeded(int status) {
        return status == Status.SUCCESS;
    }

    /** 状态码是否表示取消 */
    public static bool IsCancelled(int status) {
        return status == Status.CANCELLED;
    }

    /** 状态码是否表示失败 */
    public static bool IsFailed(int status) {
        return status > Status.CANCELLED;
    }

    /** 状态码是否表示取消或失败 */
    public static bool IsFailedOrCancelled(int status) {
        return status >= Status.CANCELLED;
    }

    //

    /** 将给定状态码归一化，所有的失败码将被转为<code>ERROR</code>  */
    public static int Normalize(int status) {
        if (status < 0) return 0;
        if (status > ERROR) return ERROR;
        return status;
    }

    /** 如果给定状态是失败码，则返回参数，否则返回默认失败码 */
    public static int ToFailure(int status) {
        return status < ERROR ? ERROR : status;
    }
}