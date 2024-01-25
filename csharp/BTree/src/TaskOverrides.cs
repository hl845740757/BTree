#region LICENSE

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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Wjybxx.BTree;

internal class TaskOverrides
{
    public const int MASK_BEFORE_ENTER = 1;
    public const int MASK_ENTER = 1 << 1;
    public const int MASK_EXIT = 1 << 2;
    private const int MASK_ALL = 15;

    private static readonly Type TypeTask = typeof(Task<>);
    private static readonly ConcurrentDictionary<Type, int> maskCacheMap = new ConcurrentDictionary<Type, int>();

    public static int maskOfTask(Type clazz) {
        if (maskCacheMap.TryGetValue(clazz, out int cachedMask)) {
            return cachedMask;
        }
        int mask = MASK_ALL; // 默认为全部重写
        try {
            if (IsSkippable(clazz, "beforeEnter")) {
                mask &= ~MASK_BEFORE_ENTER;
            }
            if (IsSkippable(clazz, "enter", typeof(int))) {
                mask &= ~MASK_ENTER;
            }
            if (IsSkippable(clazz, "exit")) {
                mask &= ~MASK_EXIT;
            }
        }
        catch (Exception) {
        }
        maskCacheMap.TryAdd(clazz, mask);
        return mask;
    }

    private static bool IsSkippable(Type handlerType, string methodName, params Type[] paramTypes) {
        MethodInfo methodInfo = handlerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, paramTypes);
        if (methodInfo == null) {
            return true;
        }
        Type declaringType = methodInfo.DeclaringType;
        Debug.Assert(declaringType != null);
        return IsSkippable(declaringType);
    }

    private static bool IsSkippable(Type type) {
        type = type.GetGenericTypeDefinition();
        return type == TypeTask;
    }
}