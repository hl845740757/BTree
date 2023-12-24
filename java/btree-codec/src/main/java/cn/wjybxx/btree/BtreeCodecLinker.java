/*
 * Copyright 2023 wjybxx(845740757@qq.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package cn.wjybxx.btree;

import cn.wjybxx.btree.branch.*;
import cn.wjybxx.btree.branch.join.*;
import cn.wjybxx.btree.decorator.*;
import cn.wjybxx.btree.fsm.ChangeStateTask;
import cn.wjybxx.btree.fsm.StateMachineTask;
import cn.wjybxx.btree.leaf.*;
import cn.wjybxx.dson.codec.ClassImpl;
import cn.wjybxx.dson.codec.CodecLinker;
import cn.wjybxx.dson.codec.CodecLinkerGroup;

/**
 * 这是一个配置文件，用于生成行为树关联的codec
 *
 * @author wjybxx
 * date - 2023/12/24
 */
@CodecLinkerGroup(outputPackage = "cn.wjybxx.btree")
public class BtreeCodecLinker {

    @CodecLinker(classImpl = @ClassImpl)
    public TaskEntry<?> taskEntry;

    @CodecLinkerGroup(outputPackage = "cn.wjybxx.btree.fsm")
    private static class FsmLinker {
        @CodecLinker(classImpl = @ClassImpl)
        private ChangeStateTask<?> changeStateTask;
        @CodecLinker(classImpl = @ClassImpl)
        private StateMachineTask<?> stateMachineTask;
    }

    @CodecLinkerGroup(outputPackage = "cn.wjybxx.btree.branch")
    private static class BranchLinker {
        @CodecLinker(classImpl = @ClassImpl)
        private ActiveSelector<?> activeSelector;
        @CodecLinker(classImpl = @ClassImpl)
        private FixedSwitch<?> fixedSwitch;
        @CodecLinker(classImpl = @ClassImpl)
        private Foreach<?> foreachTask;
        @CodecLinker(classImpl = @ClassImpl)
        private Join<?> join;
        @CodecLinker(classImpl = @ClassImpl)
        private Selector<?> selector;
        @CodecLinker(classImpl = @ClassImpl)
        private SelectorN<?> selectorN;
        @CodecLinker(classImpl = @ClassImpl)
        private Sequence<?> sequence;
        @CodecLinker(classImpl = @ClassImpl)
        private ServiceParallel<?> serviceParallel;
        @CodecLinker(classImpl = @ClassImpl)
        private SimpleParallel<?> simpleParallel;
        @CodecLinker(classImpl = @ClassImpl)
        private Switch<?> switchTask;

        @CodecLinker(classImpl = @ClassImpl(singleton = "getInstance"))
        private JoinAnyOf<?> joinAnyOf;
        @CodecLinker(classImpl = @ClassImpl(singleton = "getInstance"))
        private JoinMain<?> joinMain;
        @CodecLinker(classImpl = @ClassImpl(singleton = "getInstance"))
        private JoinSelector<?> joinSelector;
        @CodecLinker(classImpl = @ClassImpl)
        private JoinSelectorN<?> joinSelectorN; // selectorN有状态，不能单例
        @CodecLinker(classImpl = @ClassImpl(singleton = "getInstance"))
        private JoinSequence<?> joinSequence;
        @CodecLinker(classImpl = @ClassImpl(singleton = "getInstance"))
        private JoinWaitAll<?> joinWaitAll;
    }

    @CodecLinkerGroup(outputPackage = "cn.wjybxx.btree.decorator")
    private static class DecoratorLinker {
        @CodecLinker(classImpl = @ClassImpl)
        private AlwaysCheckGuard<?> alwaysCheckGuard;
        @CodecLinker(classImpl = @ClassImpl)
        private AlwaysFail<?> alwaysFail;
        @CodecLinker(classImpl = @ClassImpl)
        private AlwaysRunning<?> alwaysRunning;
        @CodecLinker(classImpl = @ClassImpl)
        private AlwaysSuccess<?> alwaysSuccess;
        @CodecLinker(classImpl = @ClassImpl)
        private Inverter<?> inverter;
        @CodecLinker(classImpl = @ClassImpl)
        private OnlyOnce<?> onlyOnce;
        @CodecLinker(classImpl = @ClassImpl)
        private Repeat<?> repeat;
        @CodecLinker(classImpl = @ClassImpl)
        private SubtreeRef<?> subtreeRef;
        @CodecLinker(classImpl = @ClassImpl)
        private UntilCond<?> untilCond;
        @CodecLinker(classImpl = @ClassImpl)
        private UntilFail<?> untilFail;
        @CodecLinker(classImpl = @ClassImpl)
        private UntilSuccess<?> untilSuccess;
    }

    @CodecLinkerGroup(outputPackage = "cn.wjybxx.btree.leaf")
    private static class LeafLinker {
        @CodecLinker(classImpl = @ClassImpl)
        private Failure<?> failure;
        @CodecLinker(classImpl = @ClassImpl)
        private Running<?> running;
        @CodecLinker(classImpl = @ClassImpl)
        private SimpleRandom<?> simpleRandom;
        @CodecLinker(classImpl = @ClassImpl)
        private Success<?> success;
        @CodecLinker(classImpl = @ClassImpl)
        private WaitFrame<?> waitFrame;
    }
}