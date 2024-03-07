using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;

namespace SmoothScrollBug {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();

#if ANDROID
            builder.ConfigureLifecycleEvents(events => {
                
                events.AddAndroid(android => android.OnCreate((activity, bundle) => {
                    Debug.WriteLine("TRY TO TURN ON HARDWARE ACCEL");
                    if (activity.Window != null) {
                        Debug.WriteLine("WINDOW EXISTS");
                        activity.Window.SetFlags(Android.Views.WindowManagerFlags.HardwareAccelerated, Android.Views.WindowManagerFlags.HardwareAccelerated);
                    }
                }));

            });
#endif
            builder
                .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
