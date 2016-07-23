using System;

namespace Magisterka.VisualEcosystem.Animation.AnimationCommands
{
    public interface IAnimationCommand
    {
        event EventHandler AnimationEnded;
        void BeginAnimation();
    }
}
