using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventHook;
using SFML.Audio;
using SFML.Window;
using zmland.Win32;

namespace WindowNotifier
{
    class Program
    {
        private static readonly SoundBuffer _notificationSound = new SoundBuffer("notification.ogg");
        private static readonly Rectangle _screenSize = SystemInformation.VirtualScreen;

        static void Main(string[] args)
        {
            InitializeListener();
            ApplicationWatcher.Start();
            ApplicationWatcher.OnApplicationWindowChange += (sender, eventArgs) =>
            {
                if (eventArgs.Event == ApplicationEvents.Closed) return;
                tagRECT windowRect;
                NativeMethods.GetWindowRect(eventArgs.ApplicationData.HWnd, out windowRect);
                var cx = windowRect.left + (windowRect.right - windowRect.left) / 2.0f;
                var cy = windowRect.top + (windowRect.bottom - windowRect.top) / 2.0f;
                var width = windowRect.right - windowRect.left;
                var height = windowRect.bottom - windowRect.top;
                var soundPosition = new Vector3f(cx / _screenSize.Width, cy / _screenSize.Height, 0);
                new Sound(_notificationSound)
                {
                    Position = soundPosition,
                    Attenuation = 2,
                    Pitch = (_screenSize.Width * (float)_screenSize.Height) / (width * (float)height) * 0.25f,
                    RelativeToListener = false
                }.Play();
            };
            Console.ReadLine();
            ApplicationWatcher.Stop();
        }

        private static void InitializeListener()
        {
            Console.WriteLine("Click on the center of your workspace to initialize the listener position.");
            while (!Mouse.IsButtonPressed(Mouse.Button.Left)) {}
            var mousePosition = Mouse.GetPosition();
            Listener.Position = new Vector3f(mousePosition.X / (float)_screenSize.Width, mousePosition.Y / (float)_screenSize.Height, 0);
            Console.WriteLine($"Listener position set to {Listener.Position}");
        }
    }
}
