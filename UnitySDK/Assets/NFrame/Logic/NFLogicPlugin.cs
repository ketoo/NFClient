using UnityEngine;
using System.Collections;
using NFSDK;

namespace NFrame
{
    public class NFLogicPlugin : NFIPlugin
    {
        public NFLogicPlugin(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "NFLogicPlugin";
        }

        public override void Install()
        {
            AddModule<NFNetModule>(new NFNetModule(mPluginManager));
            AddModule<NFLoginModule>(new NFLoginModule(mPluginManager));
			AddModule<NFHelpModule>(new NFHelpModule(mPluginManager));
			AddModule<NFLogModule>(new NFLogModule(mPluginManager));
			AddModule<NFNetEventModule>(new NFNetEventModule(mPluginManager));
			AddModule<NFNetHandlerModule>(new NFNetHandlerModule(mPluginManager));
			AddModule<NFLanguageModule>(new NFLanguageModule(mPluginManager));

        }

        public override void Uninstall()
        {
			mPluginManager.RemoveModule<NFLanguageModule>();
			mPluginManager.RemoveModule<NFNetHandlerModule>();
			mPluginManager.RemoveModule<NFNetEventModule>();
			mPluginManager.RemoveModule<NFLogModule>();
			mPluginManager.RemoveModule<NFHelpModule>();
			mPluginManager.RemoveModule<NFLoginModule>();
			mPluginManager.RemoveModule<NFNetModule>();

            mModules.Clear();
        }
    }
}
