using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using WiimoteLib;


namespace StarterKit
{
    class Game : GameWindow
    {

        Wiimote _wm;


        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;

            _wm = new Wiimote();
            _wm.WiimoteChanged += wm_WiimoteChanged;
            _wm.Connect();
            _wm.SetReportType(InputReport.IRAccel, true);
            _wm.SetLEDs(false, true, true, false);
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.1f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }


        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            if(args.WiimoteState.ButtonState.A)
                _translation = new Vector3(_translation.X + 0.1f, _translation.Y, _translation.Z);
            if (args.WiimoteState.ButtonState.B)
                _translation = new Vector3(_translation.X - 0.1f, _translation.Y, _translation.Z);

            if(args.WiimoteState.AccelState.Values.X > 0.5)
                _rotationz = _rotationz - 0.1f;

            if (args.WiimoteState.AccelState.Values.X < -0.5)
                _rotationz = _rotationz + 0.1f;
        }

        private Vector3 _translation = new Vector3(0, 0, 0);
        private float _rotationx = 0f;
        private float _rotationy = 0f;
        private float _rotationz = 0f;

        private float facing = 0.0f;

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Right])
                _translation = new Vector3(_translation.X + 0.1f, _translation.Y, _translation.Z);
            if (Keyboard[Key.Left])
                _translation = new Vector3(_translation.X - 0.1f, _translation.Y, _translation.Z);
            if (Keyboard[Key.Up])
                _translation = new Vector3(_translation.X, _translation.Y + 0.1f, _translation.Z);
            if (Keyboard[Key.Down])
                _translation = new Vector3(_translation.X, _translation.Y - 0.1f, _translation.Z);


            if (Keyboard[Key.D])
            {
                facing = facing - 0.1f;
                _rotationz = _rotationz - 0.1f;
            }

            if (Keyboard[Key.F])
            {
                facing = facing + 0.1f;
                _rotationz = _rotationz + 0.1f;
            }


            if (Keyboard[Key.Escape])
                Exit();
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            var look = Matrix4.LookAt(3f, 3f, 2f, 0f, 0f, 0f, 0f, 0f, 1f);
            GL.LoadMatrix(ref look);

            var matrixTranslation = Matrix4.CreateTranslation(_translation);
            var matrixRotationx = Matrix4.CreateRotationX(_rotationx);
            var matrixRotationy = Matrix4.CreateRotationY(_rotationy);
            var matrixRotationz = Matrix4.CreateRotationZ(_rotationz);
            GL.MultMatrix(ref matrixTranslation);
            GL.MultMatrix(ref matrixRotationx);
            GL.MultMatrix(ref matrixRotationy);
            GL.MultMatrix(ref matrixRotationz);


            GL.Begin(BeginMode.Triangles);


            for (var iy = -5; iy < 5; iy++)
            {
                for (var i = -5; i < 5; i++)
                {
                    GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(-0.5f + i, -0.5f + iy, 0f);
                    GL.Color3(1.0f, 0.0f, 0.5f); GL.Vertex3(0.5f + i, -0.5f + iy, 0f);
                    GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(0.0f + i, 0.5f + iy, 0f);
                }
            }


            GL.End();

            SwapBuffers();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}
