using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal interface IPressable
    {
        void Press(MouseDownEvent evt);
    }
}