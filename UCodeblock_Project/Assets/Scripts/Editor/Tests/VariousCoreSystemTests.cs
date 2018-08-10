using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UCodeblock.Tests
{
    public class VariousCoreSystemTests
    {
        /// <summary>
        /// Runs a [UnityTest] for the specified CodeblockTestType.
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
    public abstract class UCodeblock_TestBase : MonoBehaviour, IMonoBehaviourTest
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

    public sealed class UCodeblock_ForLoopTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            DebugLogBlock dlb = new DebugLogBlock();
            dlb.Message = "This will run 4 times.";

            DynamicInputOperator<int> count = new DynamicInputOperator<int>();
            count.Value = 4;

            ForLoopBlock flb = new ForLoopBlock();
            flb.LoopCount = count;

            flb.Children.Codeblocks.Add(dlb);
            flb.Children.EntryID = dlb.Identity.ID;

            system.Blocks.Codeblocks.Add(flb);
            system.Blocks.EntryID = flb.Identity.ID;

            return system;
        }
    }
    public sealed class UCodeblock_IfElseConditionTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            DebugLogBlock ifTrue = new DebugLogBlock();
            ifTrue.Message = "This ran, because the condition was true.";

            DebugLogBlock ifFalse = new DebugLogBlock();
            ifFalse.Message = "This ran, because the condition was false.";

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
            ifelse.Children.Codeblocks.Add(ifTrue);
            ifelse.Children.EntryID = ifTrue.Identity.ID;
            ifelse.ElseChildren.Codeblocks.Add(ifFalse);
            ifelse.ElseChildren.EntryID = ifFalse.Identity.ID;

            system.Blocks.Codeblocks.Add(ifelse);
            system.Blocks.EntryID = ifelse.Identity.ID;

            return system;
        }
    }
    public sealed class UCodeblock_BreakForLoopTest : UCodeblock_TestBase
    {
        protected override CodeblockSystem BuildSystem()
        {
            CodeblockSystem system = new CodeblockSystem();

            DebugLogBlock dlb = new DebugLogBlock();
            dlb.Message = "This should never run.";

            BreakCodeblock bc = new BreakCodeblock();
            bc.Identity.ToID = dlb.Identity.ID;
            dlb.Identity.FromID = bc.Identity.ID;

            DynamicInputOperator<int> count = new DynamicInputOperator<int>();
            count.Value = 100;

            ForLoopBlock flb = new ForLoopBlock();
            flb.LoopCount = count;

            flb.Children.Codeblocks.Add(dlb);
            flb.Children.Codeblocks.Add(bc);
            flb.Children.EntryID = bc.Identity.ID;

            system.Blocks.Codeblocks.Add(flb);
            system.Blocks.EntryID = flb.Identity.ID;

            return system;
        }
    }
    public sealed class UCodeblock_ArithmeticAndLogicTest : UCodeblock_TestBase
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

            DebugLogBlock dlb = new DebugLogBlock();
            dlb.Message = "0 <= 2 || 4 == 6  -->  TRUE";

            IfBlock ifblock = new IfBlock();
            ifblock.Children.Codeblocks.Add(dlb);
            ifblock.Children.EntryID = dlb.Identity.ID;
            ifblock.Condition = logic;

            system.Blocks.Codeblocks.Add(ifblock);
            system.Blocks.EntryID = ifblock.Identity.ID;

            return system;
        }
    }
}