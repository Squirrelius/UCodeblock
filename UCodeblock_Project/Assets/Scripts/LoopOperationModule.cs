using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCodeblock
{
    public enum LoopCancelReason
    {
        None = 0,
        Break,
        Continue
    }

    public class LoopOperationModule
    {
        /// <summary>
        /// The last reason a coroutine was cancelled.
        /// </summary>
        public LoopCancelReason LastCancelReason { get; set; }
        public bool HasLoopCoroutine => _coroutineStack.Count > 0;

        private Stack<IEnumerator> _coroutineStack;

        public LoopOperationModule()
        {
            _coroutineStack = new Stack<IEnumerator>();
            LastCancelReason = LoopCancelReason.None;
        }

        /// <summary>
        /// Registers a new coroutine as the top level coroutine.
        /// </summary>
        public void RegisterLoopCoroutine (IEnumerator coroutine)
        {
            _coroutineStack.Push(coroutine);
        }
        /// <summary>
        /// Unregisters the top coroutine. Returns false if there is no coroutine to unregister.
        /// </summary>
        public bool UnregisterTopCoroutine()
        {
            return CancelTopCoroutine(LoopCancelReason.None);
        }
        /// <summary>
        /// Cancels the top level coroutine, due to a given reason. Returns false if there is no coroutine to cancel.
        /// </summary>
        public bool CancelTopCoroutine (LoopCancelReason reason, MonoBehaviour context = null)
        {
            // Return false if the stack is empty.
            if (!HasLoopCoroutine)
                return false;

            // If the coroutine was cancelled for a reason, stop the execution of it.
            if (reason != LoopCancelReason.None && context != null)
                context.StopCoroutine(_coroutineStack.Peek());

            // Otherwise pop the topmost element and assign the reason.
            _coroutineStack.Pop();
            LastCancelReason = reason;

            return true;
        }
        /// <summary>
        /// Attempts to complete the given coroutine. Returns false, if the current top level coroutine isn't the given one.
        /// </summary>
        public bool CompleteCoroutine (IEnumerator coroutine)
        {
            if (!HasLoopCoroutine)
                return false;

            if (_coroutineStack.Peek() == coroutine)
            {
                CancelTopCoroutine(LoopCancelReason.None);
                return true;
            }
            return false;
        }
    }
}