using System;
using GameManager.iframework.core;
using GameManager.ios;

namespace GameManager.ctr_commons
{
    internal class CTRApp : Application
    {
        public override void dealloc()
        {
            throw new NotImplementedException();
        }

        public virtual void applicationWillTerminate(UIApplication application)
        {
            sharedPreferences().savePreferences();
        }

        public virtual void applicationDidReceiveMemoryWarning(UIApplication application)
        {
            throw new NotImplementedException();
        }

        public virtual void challengeStartedWithGameConfig(NSString gameConfig)
        {
            throw new NotImplementedException();
        }

        public virtual void applicationWillResignActive(UIApplication application)
        {
            sharedPreferences().savePreferences();
            if (root != null && !root.isSuspended())
            {
                root.suspend();
            }
        }

        public virtual void applicationDidBecomeActive(UIApplication application)
        {
            if (root != null && root.isSuspended())
            {
                root.resume();
            }
        }
    }
}
