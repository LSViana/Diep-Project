using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diep.GameEngine.Shared
{
    public interface IGameObject : IDrawable
    {
        void Step(IEnumerable<object> data);
        long Id { get; set; }
    }
}
