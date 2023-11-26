using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Laboratory.Core
{
    public class LabCenter
    {
        private Dictionary<Type, ILab> _labs;
        private List<ILab> _activeLabs;

        [Inject]
        public LabCenter(TypeObjFactory factory)
        {
            _labs = new Dictionary<Type, ILab>();
            _activeLabs = new List<ILab>();

            CreateAllLabs(factory);
        }

        public async UniTask SetNewContext(LabContext labContext)
        {
            List<ILab> neededLabs = FindLabsByContext(labContext);

            List<ILab> extraObjects = _activeLabs.Except(neededLabs).ToList();
            List<ILab> commonObjects = _activeLabs.Intersect(neededLabs).ToList();
            List<ILab> missingObjects = neededLabs.Except(_activeLabs).ToList();

            await PerformBreakFor(extraObjects);
            await PerformRebootFor(commonObjects);
            await PerformWorkFor(missingObjects);
        }

        public async UniTask AddMoreContecst(LabContext labContext)
        {
            List<ILab> neededLabs = FindLabsByContext(labContext);

            List<ILab> missingObjects = neededLabs.Except(_activeLabs).ToList();

            await PerformWorkFor(missingObjects);
        }

        private void CreateAllLabs(TypeObjFactory factory)
        {
            List<ILab> labs = factory.CreateAll<ILab>();
            foreach (ILab lab in labs)
            {
                _labs[lab.GetType()] = lab;
            }
        }

        private UniTask PerformBreakFor(List<ILab> extraLabs)
        {
            foreach (ILab extraLab in extraLabs)
            {
                extraLab.Break();
                _activeLabs.Remove(extraLab);
            }

            return UniTask.CompletedTask;
        }

        private UniTask PerformRebootFor(List<ILab> commonLabs)
        {
            foreach (ILab common in commonLabs)
            {
                common.Reboot();
            }

            return UniTask.CompletedTask;
        }

        private async UniTask PerformWorkFor(List<ILab> missingLabs) 
        {
            foreach (ILab missing in missingLabs)
            {
                if (missing is IWarmingLab warming)
                {
                    await warming.WarmUp();
                }
            }

            foreach (var missing in missingLabs)
            {
                missing.Work().Forget();
                _activeLabs.Add(missing);
            }
        }

        private List<ILab> FindLabsByContext(LabContext labContext)
        {
            List<Type> context = labContext.GetTypes();

            if (context == null) return default;
            if (context.Count == 0) return default;


            List<ILab> neededLabs = context.Where(c => _labs.ContainsKey(c))
                                           .Select(c => _labs[c])
                                           .ToList();

            return neededLabs;
        }
    }
}