using Microsoft.Win32;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace xInvaders;

/// <summary>
/// The main entry point for the application.
/// </summary>
internal static class Program
{
    [STAThread]
    //  This is advanced code that sets up global exception handling.
    //  Particularly useful for catching exceptions that occur on background threads or in tasks. The handlers are: 
    //  •	UI thread exceptions(Application.ThreadException)
    //  •	AppDomain unhandled exceptions(background threads)
    //  •	TaskScheduler.UnobservedTaskException(unobserved Task faults)
    //  •	plus a try/catch around game.Run That gives broad coverage so the user sees a message box for virtually any unexpected error.
    private static void Main()
    {
        //// Register global exception handlers so we can show a message box for unexpected errors
        Application.ThreadException += (sender, args) => HandleException(args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            // args.ExceptionObject can be non-Exception
            HandleException(args.ExceptionObject as Exception ?? new Exception("Unhandled exception"));
        };
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            args.SetObserved();
            HandleException(args.Exception);
        };

        try
        {
            using var game = new xInvaders.XInvaders();
            try
            {
                game.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private static void HandleException(Exception ex)
    {
        try
        {
            string text = $"An unexpected error occurred:\n\n{ex.Message}\n\n{ex.StackTrace}";
            MessageBox.Show(text, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch
        {
            try
            {
                // Fallback to console if message box fails
                Console.WriteLine(ex.ToString());
            }
            catch
            {
                // swallow
            }
        }

        // Ensure process exits after showing the error
        try { Environment.Exit(1); } catch { }
    }
}
