using FFXIVClientStructs.FFXIV.Client.Game;

namespace Xperimental.Common;
internal class ActionManagerInstance
{
    private unsafe static ActionManager* instanceActionManager;

    public unsafe static ActionManager* InstanceActionManager
    {
        get
        {
            if (instanceActionManager == null)
            {
                instanceActionManager = ActionManager.Instance();
                if (instanceActionManager == null)
                {
                    throw new Exception("Could not create an instanceActionManager of the ActionManager.");
                }
            }

            return instanceActionManager;
        }
    }
}
