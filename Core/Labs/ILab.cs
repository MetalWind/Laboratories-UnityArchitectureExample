using System.Collections;

namespace Laboratory.Core
{
    public interface ILab
    {
        public IEnumerator Work();
        public IEnumerator Reboot();
        public IEnumerator Break();
    }
}