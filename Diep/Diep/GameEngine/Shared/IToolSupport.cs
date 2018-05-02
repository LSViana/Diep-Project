using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public interface IToolSupport : IMovable, IGameObject
    {
        Single Opacity { get; set; }
        TeamColor TeamColor { get; set; }
        GameScreen Screen { get; set; }
        Single Stability { get; set; }
    }
}
