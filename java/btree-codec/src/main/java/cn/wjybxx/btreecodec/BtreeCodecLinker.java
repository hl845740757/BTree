/*
 * Copyright 2024 wjybxx(845740757@qq.com)
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
package cn.wjybxx.btreecodec;

import cn.wjybxx.btree.TaskEntry;
import cn.wjybxx.btree.branch.*;
import cn.wjybxx.btree.branch.join.*;
import cn.wjybxx.btree.decorator.*;
import cn.wjybxx.btree.fsm.ChangeStateTask;
import cn.wjybxx.btree.fsm.StateMachineTask;
import cn.wjybxx.btree.leaf.*;
import cn.wjybxx.dsoncodec.annotations.DsonCodecLinker;
import cn.wjybxx.dsoncodec.annotations.DsonCodecLinkerGroup;
import cn.wjybxx.dsoncodec.annotations.DsonSerializable;

/**
 * 这是一个配置文件，用于生成行为树关联的codec
 *
 * @author wjybxx
 * date - 2023/12/24
 */
@DsonCodecLinkerGroup(outputPackage = "cn.wjybxx.btreecodec")
public class BtreeCodecLinker {

    @DsonCodecLinker(props = @DsonSerializable)
    public TaskEntry<?> taskEntry;

    @DsonCodecLinkerGroup(outputPackage = "cn.wjybxx.btreecodec.fsm")
    private static class FsmLinker {
        @DsonCodecLinker(props = @DsonSerializable)
        private ChangeStateTask<?> changeStateTask;
        @DsonCodecLinker(props = @DsonSerializable)
        private StateMachineTask<?> stateMachineTask;
    }

    @DsonCodecLinkerGroup(outputPackage = "cn.wjybxx.btreecodec.branch")
    private static class BranchLinker {
        @DsonCodecLinker(props = @DsonSerializable)
        private ActiveSelector<?> activeSelector;
        @DsonCodecLinker(props = @DsonSerializable)
        private FixedSwitch<?> fixedSwitch;
        @DsonCodecLinker(props = @DsonSerializable)
        private Foreach<?> foreachTask;
        @DsonCodecLinker(props = @DsonSerializable)
        private Join<?> join;
        @DsonCodecLinker(props = @DsonSerializable)
        private Selector<?> selector;
        @DsonCodecLinker(props = @DsonSerializable)
        private SelectorN<?> selectorN;
        @DsonCodecLinker(props = @DsonSerializable)
        private Sequence<?> sequence;
        @DsonCodecLinker(props = @DsonSerializable)
        private ServiceParallel<?> serviceParallel;
        @DsonCodecLinker(props = @DsonSerializable)
        private SimpleParallel<?> simpleParallel;
        @DsonCodecLinker(props = @DsonSerializable)
        private Switch<?> switchTask;
    }

    @DsonCodecLinkerGroup(outputPackage = "cn.wjybxx.btreecodec.branch.join")
    private static class JoinPolicyLinker {
        @DsonCodecLinker(props = @DsonSerializable(singleton = "getInstance"))
        private JoinAnyOf<?> joinAnyOf;
        @DsonCodecLinker(props = @DsonSerializable(singleton = "getInstance"))
        private JoinMain<?> joinMain;
        @DsonCodecLinker(props = @DsonSerializable(singleton = "getInstance"))
        private JoinSelector<?> joinSelector;
        @DsonCodecLinker(props = @DsonSerializable)  // selectorN有状态，不能单例
        private JoinSelectorN<?> joinSelectorN;
        @DsonCodecLinker(props = @DsonSerializable(singleton = "getInstance"))
        private JoinSequence<?> joinSequence;
        @DsonCodecLinker(props = @DsonSerializable(singleton = "getInstance"))
        private JoinWaitAll<?> joinWaitAll;
    }

    @DsonCodecLinkerGroup(outputPackage = "cn.wjybxx.btreecodec.decorator")
    private static class DecoratorLinker {
        @DsonCodecLinker(props = @DsonSerializable)
        private AlwaysCheckGuard<?> alwaysCheckGuard;
        @DsonCodecLinker(props = @DsonSerializable)
        private AlwaysFail<?> alwaysFail;
        @DsonCodecLinker(props = @DsonSerializable)
        private AlwaysRunning<?> alwaysRunning;
        @DsonCodecLinker(props = @DsonSerializable)
        private AlwaysSuccess<?> alwaysSuccess;
        @DsonCodecLinker(props = @DsonSerializable)
        private Inverter<?> inverter;
        @DsonCodecLinker(props = @DsonSerializable)
        private OnlyOnce<?> onlyOnce;
        @DsonCodecLinker(props = @DsonSerializable)
        private Repeat<?> repeat;
        @DsonCodecLinker(props = @DsonSerializable)
        private SubtreeRef<?> subtreeRef;
        @DsonCodecLinker(props = @DsonSerializable)
        private UntilCond<?> untilCond;
        @DsonCodecLinker(props = @DsonSerializable)
        private UntilFail<?> untilFail;
        @DsonCodecLinker(props = @DsonSerializable)
        private UntilSuccess<?> untilSuccess;
    }

    @DsonCodecLinkerGroup(outputPackage = "cn.wjybxx.btreecodec.leaf")
    private static class LeafLinker {
        @DsonCodecLinker(props = @DsonSerializable)
        private Failure<?> failure;
        @DsonCodecLinker(props = @DsonSerializable)
        private Running<?> running;
        @DsonCodecLinker(props = @DsonSerializable)
        private SimpleRandom<?> simpleRandom;
        @DsonCodecLinker(props = @DsonSerializable)
        private Success<?> success;
        @DsonCodecLinker(props = @DsonSerializable)
        private WaitFrame<?> waitFrame;
    }
}