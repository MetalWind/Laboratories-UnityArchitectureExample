using System;
using System.Collections;
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

        public IEnumerator SetNewContext(LabContext labContext)
        {
            List<Type> context = labContext.GetTypes();

            if (context == null) yield return null;
            if (context.Count == 0) yield return null;


            List<ILab> neededLabs = context.Where(c => _labs.ContainsKey(c))
                                           .Select(c => _labs[c])
                                           .ToList();


            List<ILab> commonObjects = _activeLabs.Intersect(neededLabs).ToList();
            List<ILab> extraObjects = _activeLabs.Except(neededLabs).ToList();
            List<ILab> missingObjects = neededLabs.Except(_activeLabs).ToList();

            foreach (var extra in extraObjects)
            {
                yield return AutonomCoroutine.StartRoutine(extra.Break());
                _activeLabs.Remove(extra);
            }

            foreach (var missing in missingObjects)
            {
                yield return AutonomCoroutine.StartRoutine(missing.Work());
                _activeLabs.Add(missing);
            }

            foreach (var common in commonObjects)
            {
                yield return AutonomCoroutine.StartRoutine(common.Reboot());
            }
        }

        private void CreateAllLabs(TypeObjFactory factory)
        {
            List<ILab> labs = factory.CreateAll<ILab>();
            foreach (ILab lab in labs)
            {
                _labs[lab.GetType()] = lab;
            }
        }
    }
}