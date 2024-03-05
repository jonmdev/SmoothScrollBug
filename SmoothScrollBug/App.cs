using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothScrollBug {
    public partial class App : Application {
        TestPage testPage;
        public App() {
            //InitializeComponent();

            testPage = new TestPage();
            MainPage = testPage;
            ScreenSizeMonitor.Instance.setContentPage(testPage);


        }

    }
}
