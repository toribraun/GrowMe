using System.Threading;

namespace Application
{
    public class WateringTimer
    {
        private App app;
        private Timer timer;

        public WateringTimer(App app)
        {
            this.app = app;
        }
        
        public void InitTimer(int period)
        {
            var tm = new TimerCallback(app.SendNotifications);
            // timer = new Timer(tm, new object(), 0, 1000 * 3600 * 3);
            timer = new Timer(tm, new object(), 0, period);
        }
    }
}