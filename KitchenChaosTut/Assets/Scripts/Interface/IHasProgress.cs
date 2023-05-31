using System;
/* Interface 可用 Event */

public interface IHasProgress
{
    /* progressBar Changed */
    // 傳遞當前切的次數(需傳遞必須自建event)
    public event EventHandler<OnProgressBarChangeEventArgs> OnProgressBarChanged;
    public class OnProgressBarChangeEventArgs : EventArgs
    {
        public float progressBarNormalized;
    }
}
