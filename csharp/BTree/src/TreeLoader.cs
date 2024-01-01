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

using System;
using System.Collections.Generic;

namespace Wjybxx.BTree;

public interface TreeLoader
{
    /**
     * 1.加载时，通常应按照名字加载，再尝试按照guid加载。
     * 2.如果对象是一棵树，行为树的结构必须是稳定的。
     *
     * @param nameOrGuid 行为树的名字或guid
     * @return 编辑器导出的对象
     */
    object? tryLoadObject(string nameOrGuid);

    object loadObject(string nameOrGuid) {
        object result = tryLoadObject(nameOrGuid);
        if (result == null) {
            throw new ArgumentException("target object is absent, name: " + nameOrGuid);
        }
        return result;
    }

    /**
     * 批量加载指定文件中的对象
     *
     * @param fileName 文件名，通常不建议带扩展后缀
     * @param sharable 是否共享；如果为true，则返回前不进行拷贝
     * @param filter   过滤器，为null则加载给定文件全部的入口对象；不要修改Entry对象的数据。
     */
    List<object> loadManyFromFile(string fileName, bool sharable, Predicate<IEntry>? filter);

    /**
     * 尝试加载行为树的根节点
     *
     * @param treeName 行为树的名字或guid
     * @return rootTask
     */
    Task<T> tryLoadRootTask<T>(string treeName) {
        object result = tryLoadObject(treeName);
        if (result == null) return null;
        if (!(result is Task<T>)) {
            throw new ArgumentException("target object is not a task, name: " + treeName);
        }
        return (Task<T>)result;
    }

    Task<T> loadRootTask<T>(string treeName) {
        object result = tryLoadObject(treeName);
        if (result == null) {
            throw new ArgumentException("target tree is absent, name: " + treeName);
        }
        if (!(result is Task<T>)) {
            throw new ArgumentException("target object is not a task, name: " + treeName);
        }
        return (Task<T>)result;
    }

    TaskEntry<T> loadTree<T>(string treeName) {
        Task<T> rootTask = loadRootTask(treeName);
        return new TaskEntry<T>(treeName, rootTask, null, this);
    }

    # region entry

    interface IEntry
    {
        /** 入口对象的名字 */
        string Name { get; }

        /** 入口对象的guid */
        string Guid { get; }

        /** 入口对象的标记信息 */
        int Flags { get; }

        /** 入口对象的类型，通常用于表示其作用 */
        int Type { get; }

        /** 入口对象绑定的Root对象 */
        object Root { get; }
    }

    #endregion

    #region NullLoader

    /// <summary>
    /// 获取不加载对象的空加载器
    /// </summary>
    /// <returns></returns>
    static TreeLoader NullLoader() {
        return CNullLoader.Instance;
    }

    private class CNullLoader : TreeLoader
    {
        internal static readonly CNullLoader Instance = new CNullLoader();

        public object? tryLoadObject(string nameOrGuid) {
            return null;
        }

        public List<object> loadManyFromFile(string fileName, bool sharable, Predicate<IEntry>? filter) {
            return new List<object>();
        }
    }

    #endregion
}