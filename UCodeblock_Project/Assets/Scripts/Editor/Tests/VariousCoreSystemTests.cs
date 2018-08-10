using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UCodeblock.Tests
{
    /// <summary>
    /// Holds various core <see cref="CodeblockSystem"/> tests.
    /// </summary>
    public class VariousCoreSystemTests
    {
        /// <summary>
        /// Runs a [UnityTest] for the specified CodeblockTestType.
        /// </summary>
        /// <typeparam name="T">The type of the test to run.</typeparam>
        /// <returns></returns>
        private static IEnumerator RunCodeblockTestType<T>() where T : UCodeblock_TestBase, IMonoBehaviourTest
        {
            MonoBehaviourTest<T> test = new MonoBehaviourTest<T>();
            yield return test;
            
            Assert.True((test.component as UCodeblock_TestBase).IsTestSuccess);
            yield break;
        }

        [UnityTest]
        public IEnumerator ForLoopTest() { yield return RunCodeblockTestType<UCodeblock_ForLoopTest>(); }
        [UnityTest]
        public IEnumerator IfConditionTest() { yield return RunCodeblockTestType<UCodeblock_IfElseConditionTest>(); }
        [UnityTest]
        public IEnumerator BreakForLoopTest () { yield return RunCodeblockTestType<UCodeblock_BreakForLoopTest>(); }
        [UnityTest]
        public IEnumerator ArithmeticAndLogicTest() { yield return RunCodeblockTestType<UCodeblock_ArithmeticAndLogicTest>(); }
    }

    /// <summary>
    /// The base class for <see cref="CodeblockSystem"/> tests.
    /// </summary>
    internal abstract class UCodeblock_TestBase : MonoBehaviour, IMonoBehaviourTest
    {
        private bool _isDone;
        public bool IsTestFinished => _isDone;
        public bool IsTestSuccess { get; private set; } = false;

        protected abstract CodeblockSystem BuildSystem();

        private IEnumerator Start()
        {
            try
            {
                CodeblockSystem system = BuildSystem();

                if (system.AnyError)
                {
                    IEnumerable<IBlockError> errors = system.Errors;
                    string[] errorMessages = errors.Select(e => e.ToString()).ToArray();
                    Debug.LogError(string.Join("\n", errorMessages));
                }
                else
                {
                    ICodeblockExecutionContext context = new StandardContext(this);
                    yield return StartCoroutine(system.Blocks.ExecuteCodeblocks(context));
                }
                _isDone = true;
            }
            finally
            {
                IsTestSuccess = true;
            }
            yield break;
        }
    }

    /// <summary>
    /// Tests if a for-loop block can run for a given amount of times.
    /// </summary>
    internal sealed class UCodeblock_ForLoopTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            const int desiredLoopCount = 4;
            int loopRunCount = 0;
            ActionBlock loopCountIncrement = new ActionBlock();
            loopCountIncrement.Action = () => loopRunCount++;

            DynamicInputOperator<int> count = new DynamicInputOperator<int>();
            count.Value = desiredLoopCount;

            ForLoopBlock forLoop = new ForLoopBlock();
            forLoop.LoopCount = count;

            AssertTestBlock assert = new AssertTestBlock();
            assert.Condition = () => loopRunCount == desiredLoopCount;

            forLoop.Children.InsertItem(loopCountIncrement, null);

            system.Blocks.InsertItem(forLoop, null);
            system.Blocks.InsertItem(assert, forLoop);

            return system;
        }
    }
    /// <summary>
    /// Checks if an if-else block can implement and evaluate a condition.
    /// </summary>
    internal sealed class UCodeblock_IfElseConditionTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            AssertTestBlock ifTrue = new AssertTestBlock();
            ifTrue.Condition = () => true;

            AssertTestBlock ifFalse = new AssertTestBlock();
            ifFalse.Condition = () => false;

            var left = new DynamicInputOperator<float>();
            left.Value = 5;
            var right = new DynamicInputOperator<string>();
            right.Value = "1";

            ComparisonOperationCodeblock coc = new ComparisonOperationCodeblock();
            coc.Operation = ComparisonOperation.GreaterThan;
            coc.Left = left;
            coc.Right = right;

            IfElseBlock ifelse = new IfElseBlock();
            ifelse.Condition = coc;

            ifelse.Children.InsertItem(ifTrue, null);
            ifelse.ElseChildren.InsertItem(ifFalse, null);

            system.Blocks.InsertItem(ifelse, null);

            return system;
        }
    }
    /// <summary>
    /// Checks if the break block works inside a for loop.
    /// </summary>
    internal sealed class UCodeblock_BreakForLoopTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            AssertTestBlock poisonedBlock = new AssertTestBlock();
            poisonedBlock.Condition = () => false;

            BreakCodeblock breakBlock = new BreakCodeblock();

            DynamicInputOperator<int> count = new DynamicInputOperator<int>();
            count.Value = 100;

            ForLoopBlock flb = new ForLoopBlock();
            flb.LoopCount = count;

            flb.Children.InsertItem(breakBlock, null);
            flb.Children.InsertItem(poisonedBlock, breakBlock);

            system.Blocks.InsertItem(flb, null);

            return system;
        }
    }
    /// <summary>
    /// Checks various arithmetic and logic codeblocks.
    /// </summary>
    internal sealed class UCodeblock_ArithmeticAndLogicTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            DynamicInputOperator<int>[] inputs = new DynamicInputOperator<int>[4];
            for (int i = 0; i < 4; i++)
            {
                DynamicInputOperator<int> input = new DynamicInputOperator<int>();
                input.Value = i * 2;
                inputs[i] = input;
            }

            ComparisonOperationCodeblock left = new ComparisonOperationCodeblock()
            {
                Left = inputs[0],
                Right = inputs[1],
                Operation = ComparisonOperation.SmallerOrEqual
            };
            ComparisonOperationCodeblock right = new ComparisonOperationCodeblock()
            {
                Left = inputs[2],
                Right = inputs[3],
                Operation = ComparisonOperation.Equal
            };

            LogicOperationCodeblock logic = new LogicOperationCodeblock()
            {
                Left = left,
                Right = right,
                Operation = LogicalOperation.OR
            };

            AssertTestBlock ifTrue = new AssertTestBlock();
            ifTrue.Condition = () => true;

            AssertTestBlock ifFalse = new AssertTestBlock();
            ifFalse.Condition = () => false;

            IfElseBlock ifElseBlock = new IfElseBlock();
            ifElseBlock.Condition = logic;

            ifElseBlock.Children.InsertItem(ifTrue, null);
            ifElseBlock.ElseChildren.InsertItem(ifFalse, null);

            system.Blocks.InsertItem(ifElseBlock, null);

            return system;
        }
    }
}