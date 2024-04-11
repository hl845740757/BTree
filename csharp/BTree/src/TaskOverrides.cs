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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Wjybxx.BTree;

/// <summary>
/// 记录Task重写的方法，以减少不必要的虚方法调用
/// </summary>
internal static class TaskOverrides
{
    public const int MASK_BEFORE_ENTER = 1;
    public const int MASK_ENTER = 1 << 1;
    public const int MASK_EXIT = 1 << 2;
    private const int MASK_ALL = 15;

    private static readonly Type TYPE_TASK = typeof(Task<>);
    private static readonly Type TYPE_INT32 = typeof(int);
    private static readonly ConcurrentDictionary<Type, int> maskCacheMap = new ConcurrentDictionary<Type, int>();

    public static int MaskOfTask(Type clazz) {
        if (clazz.IsGenericType) {
            clazz = clazz.GetGenericTypeDefinition();
        }
        if (maskCacheMap.TryGetValue(clazz, out int cachedMask)) {
            return cachedMask;
        }
        int mask = MASK_ALL; // 默认为全部重写
        try {
            if (IsSkippable(clazz, "BeforeEnter")) {
                mask &= ~MASK_BEFORE_ENTER;
            }
            if (IsSkippable(clazz, "Enter", TYPE_INT32)) {
                mask &= ~MASK_ENTER;
            }
            if (IsSkippable(clazz, "Exit")) {
                mask &= ~MASK_EXIT;
            }
        }
        catch (Exception) {
            // ignored
        }
        maskCacheMap.TryAdd(clazz, mask);
        return mask;
    }

    private static bool IsSkippable(Type handlerType, string methodName, params Type[] paramTypes) {
        MethodInfo methodInfo = handlerType.GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, paramTypes);
        if (methodInfo == null) {
            return true;
        }
        Type declaringType = methodInfo.DeclaringType;
        Debug.Assert(declaringType != null);
        return declaringType == TYPE_TASK;
    }
}