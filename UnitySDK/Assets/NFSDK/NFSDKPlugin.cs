using UnityEngine;
using System.Collections;

namespace NFSDK
{
    public class NFSDKPlugin : NFIPlugin
    {
        public NFSDKPlugin(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "NFSDKPlugin";
        }

        public override void Install()
        {
            AddModule<NFIElementModule>(new NFElementModule(mPluginManager));
            AddModule<NFIClassModule>(new NFClassModule(mPluginManager));
            AddModule<NFIKernelModule>(new NFKernelModule(mPluginManager));
            AddModule<NFIEventModule>(new NFEventModule(mPluginManager));
        }
        public override void Uninstall()
        {
			mPluginManager.RemoveModule<NFIElementModule>();
			mPluginManager.RemoveModule<NFIClassModule>();
			mPluginManager.RemoveModule<NFIKernelModule>();
			mPluginManager.RemoveModule<NFIEventModule>();
            mModules.Clear();
        }
    }
}
