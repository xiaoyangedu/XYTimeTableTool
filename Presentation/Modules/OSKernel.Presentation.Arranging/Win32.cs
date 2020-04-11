using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging
{
    public class Win32
    {
        public struct POINT { public Int32 X; public Int32 Y; }

        // During drag-and-drop operations, the position of the mouse cannot be
        // reliably determined through GetPosition. This is because control of
        // the mouse (possibly including capture) is held by the originating
        // element of the drag until the drop is completed, with much of the
        // behavior controlled by underlying Win32 calls. As a workaround, you
        // might need to use Win32 externals such as GetCursorPos.
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT point);
    }
}
