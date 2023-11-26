using Cysharp.Threading.Tasks;
using Laboratory.Core;

public interface IWarmingLab : ILab
{
    public UniTask WarmUp();
}

