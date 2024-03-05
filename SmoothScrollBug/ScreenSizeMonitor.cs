using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothScrollBug {


    public class ScreenSizeMonitor {

        public ContentPage pageToMonitor;

        //SINGLETON LAZY PATTERN https://csharpindepth.com/articles/Singleton#lazy
        private static readonly Lazy<ScreenSizeMonitor> lazy = new Lazy<ScreenSizeMonitor>(() => new ScreenSizeMonitor());
        public static ScreenSizeMonitor Instance { get { return lazy.Value; } }


        //==========================
        //SCREEN CHANGE FUNCTIONS 
        //==========================
        public Size screenSize = new Size(0, 0);
        public event Action ScreenSizeChanged = null;

        private ScreenSizeMonitor() {
        }
        public void setContentPage(ContentPage contentPageToMonitor) {
            pageToMonitor = contentPageToMonitor;
            startScreenMonitor();
        }
        private void startScreenMonitor() {

            //run once in case already valid to update
            updateFunction(null, null);

            pageToMonitor.SizeChanged -= updateFunction;
            pageToMonitor.SizeChanged += updateFunction;

        }
        private void updateFunction(object sender, EventArgs e) {
            if (pageToMonitor.Width > 0 && pageToMonitor.Height > 0) {
                screenSize = new Size(pageToMonitor.Width, pageToMonitor.Height);
                ScreenSizeChanged?.Invoke();
            }
        }

    }
}
