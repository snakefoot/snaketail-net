#region License statement
/* SnakeTail is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeTail
{
    /// <summary>
    /// Executes the requests in a single-threaded fashion.
    /// Will not start a new request until the result of the previous has been checked
    /// </summary>
    class ThreadPoolQueue : IDisposable
    {
        readonly object _syncLock = new object();

        Queue<KeyValuePair<WaitCallback, object>> _requests;
        IAsyncResult _pendingReplyResult;

        public ThreadPoolQueue(int workItemCapacity = 0)
        {
            _requests = new Queue<KeyValuePair<WaitCallback, object>>(workItemCapacity);
        }

        /// <summary>
        /// Executes the request, if no other request is pending
        /// </summary>
        public bool ExecuteRequest(WaitCallback request, object state = null)
        {
            KeyValuePair<WaitCallback, object>? nextPendingRequest = null;
            lock (_syncLock)
            {
                if (_requests == null)
                    return false;    // Has been disposed, nothing will be executed

                if (_requests.Count == 0)
                {
                    _requests.Enqueue(new KeyValuePair<WaitCallback, object>(request, state));
                    nextPendingRequest = _requests.Peek();
                }
            }
            if (nextPendingRequest.HasValue)
            {
                nextPendingRequest.Value.Key.BeginInvoke(nextPendingRequest.Value.Value, RequestCallback, nextPendingRequest.Value.Key);
                return true;    // Will be executed now
            }
            else
            {
                return false;   // Will not be executed (already one in progress)
            }
        }

        /// <summary>
        /// Queues the request, and starts execution if no request is currently active
        /// </summary>
        public bool QueueRequest(WaitCallback request, object state = null)
        {
            KeyValuePair<WaitCallback, object>? nextPendingRequest = null;
            lock (_syncLock)
            {
                if (_requests == null)
                    return false;    // Has been disposed, nothing will be executed

                _requests.Enqueue(new KeyValuePair<WaitCallback, object>(request, state));
                if (_requests.Count == 1)
                {
                    nextPendingRequest = _requests.Peek();
                }
            }

            if (nextPendingRequest.HasValue)
            {
                nextPendingRequest.Value.Key.BeginInvoke(nextPendingRequest.Value.Value, RequestCallback, nextPendingRequest.Value.Key);
                return true;    // Will be executed now
            }
            else
            {
                return false;   // Will be executed later
            }
        }

        /// <summary>
        /// Checks the result of the last request, and starts execution of the next pending request
        /// </summary>
        public object CheckResult()
        {
            KeyValuePair<WaitCallback, object>? nextPendingRequest = null;
            KeyValuePair<WaitCallback, object>? originalRequest = null;
            IAsyncResult resultReply = null;
            lock (_syncLock)
            {
                if (_requests == null)
                    return null;    // Has been disposed

                if (_pendingReplyResult != null)
                {
                    resultReply = _pendingReplyResult;
                    originalRequest = _requests.Dequeue();
                    if (_requests.Count > 0)
                        nextPendingRequest = _requests.Peek();
                    _pendingReplyResult = null;
                }
            }

            if (nextPendingRequest.HasValue)
            {
                nextPendingRequest.Value.Key.BeginInvoke(nextPendingRequest.Value.Value, RequestCallback, nextPendingRequest.Value.Key);
            }

            if (resultReply != null)
            {
                WaitCallback action = resultReply.AsyncState as WaitCallback;
                action.EndInvoke(resultReply);  // Can throw if any pending exception
            }

            if (originalRequest.HasValue)
                return originalRequest.Value.Value;
            else
                return null;
        }

        /// <summary>
        /// Clears all pending requests, and waits for any active request to complete
        /// </summary>
        public void Dispose()
        {
            IAsyncResult resultReply = null;
            KeyValuePair<WaitCallback, object>? originalRequest = null;

            lock (_syncLock)
            {
                if (_requests != null)
                {
                    if (_requests.Count > 0)
                        originalRequest = _requests.Dequeue();
                    _requests.Clear();
                    _requests = null;
                }
                resultReply = _pendingReplyResult;
                _pendingReplyResult = null;
            }

            if (originalRequest != null && resultReply == null)
            {
                // We need to wait for the request to complete
                while (originalRequest != null)
                {
                    System.Threading.Thread.Sleep(10);
                    lock (_syncLock)
                    {
                        if (_pendingReplyResult != null)
                        {
                            originalRequest = null;
                            resultReply = _pendingReplyResult;
                            _pendingReplyResult = null;
                        }
                    }
                }
            }
            
            if (resultReply != null)
            {
                try
                {
                    WaitCallback action = resultReply.AsyncState as WaitCallback;
                    action.EndInvoke(resultReply);
                }
                catch
                {
                }
            }
        }

        void RequestCallback(IAsyncResult ar)
        {
            lock (_syncLock)
            {
                _pendingReplyResult = ar;
            }
        }
    };
}
