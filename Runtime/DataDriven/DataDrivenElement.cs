using System;

namespace Echo.DataDriven
{
    [Serializable]
    public abstract class DataDrivenElement
    {
        protected internal DataDrivenObject Object { get; internal set; }

        protected internal abstract void OnInitialize();
        protected internal abstract void OnDispose();
    }
}