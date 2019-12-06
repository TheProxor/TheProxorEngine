using System;
using System.Collections.Generic;
using System.Text;

namespace ProxorEditor.GUI_Contexts
{
    internal class DX_GUI_Context
    {
        private struct Win32WindowInfo
        {
            public IntPtr Sdl2Window;
            public IntPtr hdc;
            public IntPtr hinstance;
        }


        public void OnPostRender()
        {
            throw new NotImplementedException();
        }

        public void OnPreRender()
        {
            throw new NotImplementedException();
        }

        public void OnRender()
        {
            throw new NotImplementedException();
        }
    }
}
