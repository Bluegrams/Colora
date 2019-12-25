using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Colora.Helpers
{
    /// <summary>
    /// Wraps a global Windows hotkey.
    /// </summary>
    public class HotKey : IDisposable
    {
        // Based on https://stackoverflow.com/a/9330358/9145461.

        private static Dictionary<int, HotKey> hotKeyDict;

        #region Win Api methods
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_HOTKEY = 0x0312;
        #endregion

        private bool _disposed = false;

        public KeyCombination KeyCombination { get; private set; }
        public Action<HotKey> Action { get; private set; }
        public int Id { get; set; }

        public bool IsRegistered => hotKeyDict.ContainsKey(Id);

        public HotKey(KeyCombination keyCombination, Action<HotKey> action, bool register = true)
        {
            KeyCombination = keyCombination;
            Action = action;
            if (register)
            {
                Register();
            }
        }

        /// <summary>
        /// Globally registers a hotkey.
        /// </summary>
        public bool Register()
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(KeyCombination.Key);
            Id = virtualKeyCode + ((int)KeyCombination.Modifiers * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, Id, (UInt32)KeyCombination.Modifiers, (UInt32)virtualKeyCode);

            if (hotKeyDict == null)
            {
                hotKeyDict = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(componentDispatcherThreadFilterMessage);
            }

            if (result)
                hotKeyDict.Add(Id, this);

            return result;
        }

        /// <summary>
        /// Unregisters a registered hotkey.
        /// </summary>
        public void Unregister()
        {
            if (hotKeyDict.ContainsKey(Id))
            {
                UnregisterHotKey(IntPtr.Zero, Id);
                hotKeyDict.Remove(Id);
            }
        }

        private static void componentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WM_HOTKEY)
                {
                    if (hotKeyDict.TryGetValue((int)msg.wParam, out HotKey hotKey))
                    {
                        hotKey.Action?.Invoke(hotKey);
                        handled = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Unregister();
                }
                _disposed = true;
            }
        }
    }
}
