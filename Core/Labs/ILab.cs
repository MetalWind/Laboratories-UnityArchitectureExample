using Cysharp.Threading.Tasks;

namespace Laboratory.Core
{
    public interface ILab
    {
        public UniTask Work();
        public UniTask Reboot();
        public UniTask Break();
    }
}