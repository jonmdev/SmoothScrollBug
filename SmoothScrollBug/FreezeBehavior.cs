using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Maui.Platform;
using System.Data;

namespace SmoothScrollBug {
    public class FreezeBehavior : Behavior<View> {

        //https://developer.android.com/develop/ui/views/graphics/hardware-accel
        //https://blog.danlew.net/2015/10/20/using-hardware-layers-to-improve-animation-performance/

#if ANDROID
        Android.Views.View androidView;
#endif
        bool isFrozen = false;
        public FreezeBehavior() { 

        }
        protected override void OnAttachedTo(View view) {
            base.OnAttachedTo(view);
            // Perform setup

#if ANDROID
            if (view != null && view.Handler!=null && view.Handler.MauiContext!=null)  {
                androidView = view.ToPlatform(view.Handler.MauiContext);
                updateFreezeStatus();
            }
            else {
                if (view != null) {
                    view.HandlerChanged += monitorHandler;
                }
            }
            void monitorHandler(object? sender, EventArgs e) {
                if (view != null && view.Handler != null && view.Handler.MauiContext != null) {
                    androidView = view.ToPlatform(view.Handler.MauiContext);
                    Debug.WriteLine("FREEZE MONITOR HANDLER FINISHED || IS HARDWARE ACCELERATED? " + androidView.IsHardwareAccelerated);
                    DebugWindow.Instance.addMessage("HARDWARE ACCEL ENABLED? " + androidView.IsHardwareAccelerated);
                    updateFreezeStatus();
                    view.HandlerChanged -= monitorHandler;
                }
            }
#endif

        }

        void updateFreezeStatus() {
#if ANDROID
            if (androidView != null) {
                if (isFrozen) {
                    androidView.SetLayerType(Android.Views.LayerType.Hardware, null);
                    Debug.WriteLine("FROZE THIS VIEW");
                }
                else {
                    androidView.SetLayerType(Android.Views.LayerType.None, null);
                }
            }
            else {
                Debug.WriteLine("Cannot freeze - no handler or maui context");
            }
#endif
        }

        protected override void OnDetachingFrom(View attachedView) {
            base.OnDetachingFrom(attachedView);
            // Perform clean up
            // never should detach I don't think.
            unFreezeView();
        }

        // Behavior implementation
        public void freezeView() {
#if ANDROID
            isFrozen = true;
            updateFreezeStatus();
#endif
        }

        public void unFreezeView() {
#if ANDROID
            isFrozen = false;
            updateFreezeStatus();
#endif
        }
    }
}
