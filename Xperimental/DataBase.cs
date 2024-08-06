using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xperimental.Common;

namespace Xperimental;
internal class DataBase
{
    public static PlayerCharacter Player => Player.Object;
    public static float DefaultGCDTotal => ActionManagerHelper.GetDefaultRecastTime();

    public static float DefaultGCDRemain =>
        ActionManagerHelper.GetDefaultRecastTime() - ActionManagerHelper.GetDefaultRecastTimeElapsed();

    public static float DefaultGCDElapsed => ActionManagerHelper.GetDefaultRecastTimeElapsed();
}
